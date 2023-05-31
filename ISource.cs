using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace rever
{
    public interface ISource
    {
        bool UseTags{get;}
        bool NoMoreThanTwoTags {get;}
        Task<SourceResult> GetApiResult(params string[] tags);
        string BaseUrl {get;}
        HttpClient Client {get;set;}
        Random Random {get;set;}
    }
}