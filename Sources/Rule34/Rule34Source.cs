// в теории, в Boorusource должен быть класс для рботы с этим апи
// но он не работает из-за того что сайт поменял его
// поэтому как только в библиотеке будет его исправленая версия, я заменю ей это

using System.Text;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace rever
{
    public class Rule34Source : ISource
    {
        public bool UseTags => true;

        public bool NoMoreThanTwoTags => false;

        public string BaseUrl => "https://rule34.xxx/";

        public HttpClient Client { get; set; }
        public Random Random { get; set; }

        int getrating(string rating)
        {
            // преобразование строки с рейтингом в число, которое можно использовать для сравнения
            if (rating == "explicit")
            {
                return 3;
            }
            if (rating == "questionable")
            {
                return 2;
            }
            if (rating == "safe")
            {
                return 1;
            }
            return 3;
        }
        StringBuilder sb = new();

        string buildApiUrl(int pid, int length, params string[] tags)
        {
            // сборка запроса к апи с параметрами
            sb.Clear();
            sb.Append("https://api.rule34.xxx/index.php?page=dapi&s=post&q=index&json=1&pid=");
            sb.Append(pid);
            sb.Append("&limit=");
            sb.Append(length);
            sb.Append("&tags=");

            try
            {
                sb.Append(tags[0]);
                int i = 1;
                do
                {
                    sb.Append('+');
                    sb.Append(tags[i]);
                    i++;
                }
                while (i < tags.Length);
            }
            catch
            {
#if DEBUG
                Console.WriteLine("Fall to complete tags to rule34 api.");
#endif
            }
            return sb.ToString();
        }

        public async Task<SourceResult> GetApiResult(params string[] tags)
        {
            //началльня генерация значений
            int pid = Random.Next(201);
            int length = Random.Next(1001);

            int i = 0;

            do
            // бесконечный цикл с запрочсами к апи
            {
                // сам запрос и чтение ответа
                var apiResult = await Client.GetAsync(buildApiUrl(pid,length,tags));
                string raw = await apiResult.Content.ReadAsStringAsync();

                //так как апи сделанно не очень хорошо, оно может вернуть и ничего, и пустой массив
                // в этом случае мы уменьшаем одно из значений
                if (raw == "" || raw == "[]")
                {
                    clearreq();
                    i++;
                    continue;
                }
                
                //десериализация массива и получение случайного обьекта
                var json = Responce.FromJson(await apiResult.Content.ReadAsStringAsync());
                var result = json[Random.Next(json.Length)];

                // генерация ответа
                SourceResult r = new SourceResult
                {
                    PostUrl = $"https://rule34.xxx/index.php?page=post&s=view&id=" + result.Id,
                    FileUrl = result.FileUrl.ToString(),
                    Tags = result.Tags.Split(),
                    Rating = getrating(result.Rating)
                };
                return r;
            }
            while (true);

            void clearreq(){
                //эта функция уменьшает значения для увеличения теоретического числа постов, которые могут попасть в выборку
                // при каждом вызове уменьшается одно из трёх значениц поочерёдно
                switch (i%3)
                {
                    case 0:
                        //уменьшение страницы с постами
                        pid = Convert.ToInt32(Math.Sqrt(pid));
                    break;
                    case 1:
                        //кменьшение количества постов в ответе
                        length = Convert.ToInt32(Math.Sqrt(length));
                    break;
                    //оба значения уменьшаются путём нахождения их квадратного корня и округлением его до целочисленного

                    case 2:
                    //убирает последний элемент из массива
                        string[] buffer = new string[tags.Length - 1];
                    for (int i = 0; i < buffer.Length; i++)
                    {
                        buffer[i] = tags[i];
                    }
                    tags = buffer;
                    break;
                }
            }
        }
    }
}