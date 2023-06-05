using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using System.IO;

namespace rever{
    public class VKSource : ISource
    {
        public bool UseTags => false;

        public bool NoMoreThanTwoTags => false;

        public string BaseUrl => throw new NotImplementedException();

        public HttpClient Client { get; set; }
        public Random Random { get;set; }

        StringBuilder builder = new();
        string access_token;
        long[] owner_ids;
        string buildUrl(long id, int offset = int.MaxValue){
            builder.Clear();
            builder.Append("https://api.vk.com/method/photos.get?album_id=wall&v=5.131&count=1");
            builder.Append($"&access_token={access_token}");
            builder.Append($"&owner_id={id}");
            builder.Append($"&offset={offset}");
            return builder.ToString();
        }
        void loadOwners(){
            // todo: change path!
            var arr = File.ReadAllLines("bin\\Debug\\net7.0\\vkowners.list");
            owner_ids = new long[arr.Length];
            for(int i = 0; i<arr.Length;i++){
                owner_ids[i] = Convert.ToInt64(arr[i]);
            }
        }
        public VKSource(string accessToken)
        {
            access_token=accessToken;
            loadOwners();
        }
        public async Task<SourceResult> GetApiResult(params string[] tags)
        {
            //получение паблика для работы с ним
            long target = owner_ids[Random.Next(owner_ids.Length)];

            string firstreq = await (await Client.GetAsync(buildUrl(target))).Content.ReadAsStringAsync();
            int groupImageCount = (int)VkResponce.FromJson(firstreq).Response.Count;
            int offset = Random.Next(groupImageCount);

            string res = await (await Client.GetAsync(buildUrl(target, offset))).Content.ReadAsStringAsync();
            var item = VkResponce.FromJson(res).Response.Items[0];

            SourceResult result = new SourceResult{
                PostUrl = "https://vk.com/thenekotopia?w=wall-175384626_"+item.PostId,
                FileUrl = item.Sizes[^1].Url.ToString(),
                Rating = 1 //подумать над этим!!
                
            };
            return result;
        }
    }
}