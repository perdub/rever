using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using BooruSharp.Booru;
using System.Net.Http;
using System.Diagnostics;
using BooruSharp.Search.Post;
using BooruSharp.Others;

namespace rever
{
    public class ImageProvider
    {
        List<ABooru> boorus = new List<ABooru>();
        HttpClient downloader;
        Random random;

        int _maxtags;
        public ImageProvider(ActiveSources a, string pixivrefresh, int maxtags)
        {
            _maxtags = maxtags;
            downloader = new HttpClient();
            random = new();

            //сборка необходимых booru api
            boorus = a.Build(downloader, pixivrefresh);
        }
        //этот метод возвращает случайные теги из всех возможных. количество тегов будет равно 
        // минимальному значению из следующих: количество всех тегов, --maxiumtags флагу и случайному целому числу от 1 до 5
        private string[] getrandomtags(params string[] tags)
        {
            List<string> selectedtags = new List<string>();
            selectedtags.Capacity = tags.Length;
            for (int i = 0; i < Math.Min(Math.Min(tags.Length, _maxtags), random.Next(1, 6)); i++)
            {
                selectedtags.Add(tags[random.Next(tags.Length)]);
            }
            return selectedtags.ToArray();
        }
        public async Task<PostInfo> GetImageStream(SearchParams search)
        {
#if DEBUG
            Stopwatch starttime = new Stopwatch();
            starttime.Start();
            Console.WriteLine($"Try to get image...");
#endif
            //выбор тегов для запроса
            string[] finaltags = getrandomtags(search.tags);
#if DEBUG
            Console.WriteLine($"Selected tags: {Newtonsoft.Json.JsonConvert.SerializeObject(finaltags)}");
#endif  
            //обьявление поля для результата и счётчика плохих запросов
            SearchResult target;
            int ratingbadresult = 0;

            //выбор случайного апи
            ABooru source = boorus[random.Next(boorus.Count)];
#if DEBUG
            Console.WriteLine($"Booru source base url: {source.BaseUrl}");
#endif
            //тут начинается самое веселое :((
            do
            {
                //бесконечный цикл с запросами к апи, мы выйдем из него как только все условия будут выполнены
                try
                {
                    //если в выбраном апи есть ограничение на только два тега для поиска, то мы усекаем массив
                    if (source.NoMoreThanTwoTags)
                    {
                        if (finaltags.Length != 0)
                        {
                            //почему тут только один тег? потому что с двумя оно почемуто не работает
                            string[] buffer = new string[1];
                            buffer[0] = finaltags[0];
                            finaltags = buffer;
                        }
                    }

                    //запрос к апи!!!
                    target = await source.GetRandomPostAsync(finaltags);



                    //если установлен флаг для определения минимального количества тегов в посте и в посте меньше
                    if (target.Tags.Count < search.mintags)
                    {
                        //возвращение в начало цикла
                        continue;
                    }

                    //проверка на забаненные теги с использованием флага banned
                    // забаненный тег - пост, который содержит хотя бы один такой тег, никогда не может быть результатьм вызова
                    bool banned = false;
                    for (int i = 0; i < search.bannedtags.Length; i++)
                    {
                        if (target.Tags.Contains(search.bannedtags[i]))
                        {
                            banned = true;
                            break;
                        }
                    }
                    if (banned)
                    {
                        continue;
                    }

                    //сверка рейтинга поста и целевого(пока что нет чёткой проверки, только на более)
                    if ((int)target.Rating > (int)search.rating)
                    {
                        //увеличение флага
                        ratingbadresult++;

                        if (ratingbadresult > 9)
                        {
                            //если флаг был установлен 10 раз, то мы генерируем теги заново(ибо мы не сможем получить пост с нужным рейтингом)
                            finaltags = getrandomtags(search.tags);
#if DEBUG
                            Console.WriteLine($"Edit selected tags: {Newtonsoft.Json.JsonConvert.SerializeObject(finaltags)}");
#endif
                            //сброс флага
                            ratingbadresult = 0;
                        }
#if DEBUG
                        Console.WriteLine("Censored!");
#endif
                        //возвращение к началу циккла
                        continue;
                    }
                }
                //если booru api ничего не может найти, то генерируется BooruSharp.Search.InvalidTags
                catch (BooruSharp.Search.InvalidTags)
                {
                    //и мы чистим массив от последнего элемента
                    //Array.Clear не используется так как он не может удалить единственный элемент из массива
                    string[] buffer = new string[finaltags.Length - 1];
                    for (int i = 0; i < buffer.Length; i++)
                    {
                        buffer[i] = finaltags[i];
                    }
                    finaltags = buffer;

#if DEBUG
                    Console.WriteLine("Remove one last tag...");
                    Console.WriteLine($"Now selected tags: {Newtonsoft.Json.JsonConvert.SerializeObject(finaltags)}");
#endif
                    continue;
                }
                //если запрос закончился сетевым исключением, то апи недоступно
                catch (System.Net.Http.HttpRequestException e)
                {
#if DEBUG
                    //уведомление в консоль
                    Console.WriteLine($"Fall to get {source.BaseUrl}: {e.StatusCode} returned.");
#endif
                    //смена апи
                    source = boorus[random.Next(boorus.Count)];
                    //возвращение в начало цикла
                    continue;
                }
                //если исключение неизвестно, мы пропускаем его дальше
                catch (Exception)
                {
                    throw;
                }
                //если код дошёл до сюда, то у нас есть пост, который подходит под все требования, и мы выходим из цикла
                break;
            }
            while (true);

            PostInfo result = new PostInfo();

            //скачивание файла
            Stream raw;
#if DEBUG
            Console.WriteLine(target.FileUrl);
#endif
            if (source is Pixiv)
            {
                raw = new MemoryStream(await ((Pixiv)source).ImageToByteArrayAsync(target));
            }
            else
            {
                raw = await downloader.GetStreamAsync(target.FileUrl);
            }

            //присваинвание результата выходному обьекту
            result.imageStream = raw;
            result.fileLink = target.FileUrl;
            result.postLink = target.PostUrl;
            result.tags = target.Tags.ToArray();
#if DEBUG
            starttime.Stop();
            Console.WriteLine($"Image load, elapsed time - {starttime.Elapsed}");
#endif
            return result;
        }
    }
}
