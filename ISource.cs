using System;
using System.Threading.Tasks;

namespace rever
{
    public interface ISource
    {
        bool UseTags{get;}
        Task<SourceResult> GetImageStream(params string[] tags);
        
    }
}