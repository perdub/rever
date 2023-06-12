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

        public string BaseUrl => "https://vk.com/";

        public HttpClient Client { get; set; }
        public Random Random { get;set; }

        StringBuilder builder = new();
        string access_token;
        VKAlbum[] targets;
        string buildUrl(VKAlbum album, int offset = int.MaxValue){
            builder.Clear();
            builder.Append("https://api.vk.com/method/photos.get?v=5.131&count=1");
            builder.Append($"&access_token={access_token}");
            builder.Append($"&offset={offset}");
            builder.Append(album.ToString());
            return builder.ToString();
        }
        void loadOwners(){
            // todo: change path!
            var arr = File.ReadAllLines("vkowners.list");
            targets = new VKAlbum[arr.Length];
            for(int i = 0; i<arr.Length;i++){
                targets[i] = new VKAlbum(arr[i]);
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
            VKAlbum target = targets[Random.Next(targets.Length)];

            string firstreq = await (await Client.GetAsync(buildUrl(target))).Content.ReadAsStringAsync();
            int groupImageCount = (int)VkResponce.FromJson(firstreq).Response.Count;
            int offset = Random.Next(groupImageCount);

            string res = await (await Client.GetAsync(buildUrl(target, offset))).Content.ReadAsStringAsync();
            var item = VkResponce.FromJson(res).Response.Items[0];

            SourceResult result = new SourceResult{
                PostUrl = $"https://vk.com/photo{item.OwnerId}_{item.Id}",
                FileUrl = item.Sizes[^1].Url.ToString(),
                Rating = 1, //подумать над этим!!
                Tags = new string[0]
            };
            return result;
        }
    }
}