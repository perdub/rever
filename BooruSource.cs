using System.Linq;
using System;
using BooruSharp.Booru;
using System.Threading.Tasks;
namespace rever
{
    public class BooruSource : ISource
    {
        readonly ABooru _booru;
        public BooruSource(ABooru booru)
        {
            _booru=booru;   
        }
        public bool UseTags { get {return true;} }

        public async Task<SourceResult> GetImageStream(params string[] tags)
        {
            var r = await _booru.GetRandomPostAsync(tags);
            return new SourceResult{
                FileUrl = r.FileUrl.ToString(),
                PostUrl = r.PostUrl.ToString(),
                Tags = r.Tags.ToArray()
            };
        }
    }
}