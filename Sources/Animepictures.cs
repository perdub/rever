using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using AngleSharp;
using AngleSharp.Dom;

namespace rever
{
    // источник с сайта anime-pictures.net, представляет собой парсер
    public class AnimePictures : ISource
    {
        public bool UseTags => false;

        public bool NoMoreThanTwoTags => false;

        public string BaseUrl => "https://anime-pictures.net/";

        public HttpClient Client { get; set; }
        public Random Random { get; set; }
        //логин не работает поэтому часть постов не будет видно
        async Task Login()
        {
            Client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/113.0.0.0 Safari/537.36");
            var logindata = new StringContent("\"{\"login\":\"perdub\",\"password\":\"myijijijiplpis123456ppppoppooopoppoopopopopn\"}\"");
            var resp = await Client.PostAsync("https://anime-pictures.net/api/v3/auth", logindata);
            Client.DefaultRequestHeaders.Remove("asian_server");
            Client.DefaultRequestHeaders.Add("asian_server", Newtonsoft.Json.JsonConvert.DeserializeObject<LoginResult>(await resp.Content.ReadAsStringAsync()).token);
        }
        record class LoginResult(bool success, string token);

        //получение результата
        public async Task<SourceResult> GetApiResult(params string[] tags)
        {
            //теги игнорируются ибо мне лень парсить поиск с ними :((
            int postid;
            string posturl;
            HttpResponseMessage result;                                                     //todo: отформатировать код                    
            Client.DefaultRequestHeaders.Add("asian_server", "fa603d8489144c94840f2874e54f61e4");
            var context = BrowsingContext.New(Configuration.Default);
            do
            {
                //получение случайного поста
                postid = Random.Next(799689);
                posturl = $"https://anime-pictures.net/posts/{postid}?lang=en";
                //попытка получения html страницы поста
                result = await Client.GetAsync(posturl);
                if (((int)result.StatusCode) == 301 || ((int)result.StatusCode) == 302)
                {
                    continue;
                }
                // открытие DOM
                IDocument dom = await context.OpenAsync(async h => h.Content(await result.Content.ReadAsStringAsync()));

                //если мы можем найти обьект с этим ид то поста не существует и мы были перенаправленны на страницу с постами
                if(dom.GetElementById("posts")!=null){
                    continue;
                }

                //парсинг тегов
                var w = dom.GetElementsByClassName("svelte-bnip2f");
                var ptags = new List<string>();
                foreach (var i in w)
                {
                    if (i.GetType().Name == "HtmlListItemElement")
                    {
                        ptags.Add(i.TextContent.Split("\n")[0]);
                    }
                }
                //получение ссылки на изображение
                var download_icon = dom.GetElementsByClassName("download_icon");
                var fileurl = ((AngleSharp.Html.Dom.IHtmlAnchorElement)download_icon[0]).Href;

                //создание обьекта результата
                SourceResult r = new SourceResult
                {
                    Tags = ptags.ToArray(),
                    Rating = 1,
                    PostUrl = posturl,
                    FileUrl = fileurl
                };
                return r;
            }
            while (true);
        }
    }
}