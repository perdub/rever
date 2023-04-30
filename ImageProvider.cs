using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using BooruSharp.Booru;
using System.Net.Http;

namespace rever
{
    public class ImageProvider
    {
        ABooru[] boorus = new ABooru[1];
        HttpClient downloader;
        public ImageProvider()
        {
            downloader = new HttpClient();

            boorus[0] = new Yandere();
            boorus[0].HttpClient = downloader;
        }
        public async Task<Stream> GetImageStream()
        {
            var target = await boorus[0].GetRandomPostAsync();
            Stream raw = await downloader.GetStreamAsync(target.FileUrl);
       /*     if (raw.Length > 10 * 1024 * 1024)
            {
                //todo: compress image
            }//*/
            return raw;
        }
    }
}
