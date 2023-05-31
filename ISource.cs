using System;
using System.Threading.Tasks;

namespace rever
{
    public interface ISource
    {
        bool UseTags{get;}
        bool NoMoreThanTwoTags {get;}
        Task<SourceResult> GetApiResult(params string[] tags);
        string BaseUrl {get;}
    }
}