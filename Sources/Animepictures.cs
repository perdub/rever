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
#if DEBUG
                Console.WriteLine($"Post url: {posturl}");
#endif
                //попытка получения html страницы поста
                result = await Client.GetAsync(posturl);
                if (((int)result.StatusCode) == 301 || ((int)result.StatusCode) == 302)
                {
                    continue;
                }
                // открытие DOM
                IDocument dom = await context.OpenAsync(async h => h.Content(await result.Content.ReadAsStringAsync()));

                //если мы можем найти обьект с этим ид то поста не существует и мы были перенаправленны на страницу с постами
                if (dom.GetElementById("posts") != null)
                {
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

                string fileurl;
                try
                {
                    //получение ссылки на изображение
                    var download_icon = dom.GetElementsByClassName("download_icon");
                    fileurl = ((AngleSharp.Html.Dom.IHtmlAnchorElement)download_icon[0]).Href;
                }
                catch (System.ArgumentOutOfRangeException)
                {
#if DEBUG
                    Console.WriteLine("Fall to parse");
#endif
                    continue;
                }

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