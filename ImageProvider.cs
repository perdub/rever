using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using BooruSharp.Booru;
using System.Net.Http;
using System.Diagnostics;

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
        public async Task<PostInfo> GetImageStream()
        {
#if DEBUG
            Stopwatch starttime = new Stopwatch();
            starttime.Start();
            Console.WriteLine($"Try to get image...");
#endif
            var target = await boorus[0].GetRandomPostAsync();
            PostInfo result = new PostInfo();
            Stream raw = await downloader.GetStreamAsync(target.FileUrl);
            result.imageStream = raw;
            result.postLink = target.PostUrl;
            result.tags = target.Tags.Select((x) => { return x; }).ToArray();
#if DEBUG
            starttime.Stop();
            Console.WriteLine($"Image load, elapsed time - {starttime.Elapsed}");
#endif
            return result;
        }
    }
}
