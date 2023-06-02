using System.Linq;
using System;
using BooruSharp.Booru;
using BooruSharp.Search.Post;
using BooruSharp.Others;
using System.Threading.Tasks;
using System.Net.Http;

namespace rever
{
    //кастомная реализация BooruSource для pixiv`а
    public class PixivSource : ISource
    {
        readonly ABooru _booru;
        public PixivSource(ABooru booru)
        {
            _booru=booru;   
        }
        //эти свойства зависят от конкретной реализации _booru
        public bool UseTags { get {return true;} }
        public bool NoMoreThanTwoTags {get{return _booru.NoMoreThanTwoTags;}}
        public string BaseUrl {get{return _booru.BaseUrl.ToString();}}

        public Pixiv GetPixiv{get{return (Pixiv)_booru;}}
        SearchResult r;
        public SearchResult PixivResult {get{return r;}}

        //это не используется в этой реализации
        HttpClient ISource.Client { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        Random ISource.Random { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public async Task<SourceResult> GetApiResult(params string[] tags)
        {
            r = await _booru.GetRandomPostAsync(tags);
            return new SourceResult{
                FileUrl = r.FileUrl.ToString(),
                PostUrl = r.PostUrl.ToString(),
                Tags = r.Tags.ToArray(),
                Rating = (int)r.Rating
            };
        }
    }
}