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
        public ImageProvider(ActiveSources a, string pixivrefresh)
        {
            downloader = new HttpClient();
            random = new();

            boorus = a.Build(downloader, pixivrefresh);
        }
        private string[] getrandomtags(params string[] tags)
        {
            List<string> selectedtags = new List<string>();
            selectedtags.Capacity = tags.Length;
            for(int i = 0; i<Math.Min(tags.Length, random.Next(1, 6)); i++)
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
            string[] finaltags = getrandomtags(search.tags);
#if DEBUG
            Console.WriteLine($"Selected tags: {Newtonsoft.Json.JsonConvert.SerializeObject(finaltags)}");
#endif
            SearchResult target;
            int ratingbadresult = 0;
            ABooru source = boorus[random.Next(boorus.Count)];
#if DEBUG
            Console.WriteLine($"Booru source base url: {source.BaseUrl}");
#endif
            do
            {
                try
                {
                    if (source.NoMoreThanTwoTags)
                    {
                        if (finaltags.Length != 0)
                        {
                            string[] buffer = new string[1];
                            buffer[0] = finaltags[0];
                            finaltags = buffer;
                        }
                    }
                    target = await source.GetRandomPostAsync(finaltags);

                    bool banned = false;
                    for(int i = 0; i< search.bannedtags.Length; i++)//check to banned tags
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

                    if ((int)target.Rating > (int)search.rating)
                    {
                        ratingbadresult++;
                        if (ratingbadresult > 9)
                        {
                            finaltags = getrandomtags(search.tags);
#if DEBUG
                            Console.WriteLine($"Edit selected tags: {Newtonsoft.Json.JsonConvert.SerializeObject(finaltags)}");
#endif
                            ratingbadresult = 0;
                        }
#if DEBUG
                        Console.WriteLine("Censored!");
#endif
                        continue;
                    }
                }
                catch(BooruSharp.Search.InvalidTags fall)
                {
                    //clear array from last element
                    string[] buffer = new string[finaltags.Length - 1];
                    for(int i = 0; i < buffer.Length; i++)
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
                catch(Exception e)
                {
                    throw;
                }
                break;
            }
            while (true);

            PostInfo result = new PostInfo();

            Stream raw;
            if (source is Pixiv)
            {
                raw = new MemoryStream(await ((Pixiv)source).ImageToByteArrayAsync(target));
            }
            else
            {
                raw = await downloader.GetStreamAsync(target.FileUrl);
            }

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
