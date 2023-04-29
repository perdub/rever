using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BooruSharp.Booru;
namespace rever
{
    public class ImageProvider
    {
        ABooru[] boorus = new ABooru[1];
        
        public ImageProvider()
        {
            boorus[0] = new Yandere();
        }
        public async Task<string> GetUrl()
        {
            return boorus[0].GetRandomPostAsync("genshin_impact").Result.FileUrl.AbsoluteUri;
        }
    }
}
