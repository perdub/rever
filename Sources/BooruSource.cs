using System.Linq;
using System;
using BooruSharp.Booru;
using BooruSharp.Search.Post;
using BooruSharp.Others;
using System.Threading.Tasks;
using System.Net.Http;

namespace rever
{
    //класс который обьединяет abooru и isource
    public class BooruSource : ISource
    {
        readonly ABooru _booru;
        public BooruSource(ABooru booru)
        {
            _booru=booru;   
        }
        //эти свойства зависят от конкретной реализации _booru
        public bool UseTags { get {return true;} }
        public bool NoMoreThanTwoTags {get{return _booru.NoMoreThanTwoTags;}}
        public string BaseUrl {get{return _booru.BaseUrl.ToString();}}

        //это не используется в этой реализации
        HttpClient ISource.Client { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        Random ISource.Random { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public async Task<SourceResult> GetApiResult(params string[] tags)
        {
            var r = await _booru.GetRandomPostAsync(tags);
            return new SourceResult{
                FileUrl = r.FileUrl.ToString(),
                PostUrl = r.PostUrl.ToString(),
                Tags = r.Tags.ToArray(),
                Rating = (int)r.Rating
            };
        }
    }
}