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
            int pid = Random.Next(201);
            int length = Random.Next(1001);

            int i = 0;

            do
            {
                var apiResult = await Client.GetAsync(buildApiUrl(pid,length,tags));
                string raw = await apiResult.Content.ReadAsStringAsync();

                if (raw == "" || raw == "[]")
                {
                    clearreq();
                    i++;
                    continue;
                }

                var json = Responce.FromJson(await apiResult.Content.ReadAsStringAsync());
                var result = json[Random.Next(json.Length)];

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
                switch (i%3)
                {
                    case 0:
                        pid = Convert.ToInt32(Math.Sqrt(pid));
                    break;
                    case 1:
                        length = Convert.ToInt32(Math.Sqrt(length));
                    break;

                    case 2:
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