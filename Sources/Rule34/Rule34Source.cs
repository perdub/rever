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
        public Random Random { get ; set ; }

        int getrating(string rating){
            if(rating == "explicit"){
                return 3;
            }
            if(rating == "questionable"){
                return 2;
            }
            if (rating == "safe"){
                return 1;
            }
            return 3;
        }

        public async Task<SourceResult> GetApiResult(params string[] tags)
        {
            int pid = Random.Next(201);
            StringBuilder sb = new StringBuilder();
            sb.Append("https://api.rule34.xxx/index.php?page=dapi&s=post&q=index&json=1&limit=1000&pid=");
            sb.Append(pid);

            sb.Append("&tags=");

            try{
                sb.Append(tags[0]);
                int i = 1;
                do{
                    sb.Append('+');
                    sb.Append(tags[i]);
                    i++;
                }
                while(i<tags.Length);
            }
            catch{
#if DEBUG
    Console.WriteLine("Fall to complete tags to rule34 api.");
#endif
            }

            var apiResult = await Client.GetAsync(sb.ToString());
            var json = Responce.FromJson(await apiResult.Content.ReadAsStringAsync());
            var result = json[Random.Next(json.Length)];

            SourceResult r = new SourceResult
            {
                PostUrl = $"https://rule34.xxx/index.php?page=post&s=view&id="+result.Id,
                FileUrl = result.FileUrl.ToString(),
                Tags = result.Tags.Split(),
                Rating = getrating(result.Rating)
            };
            return r;
        }
    }
}