using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using BooruSharp.Booru;
using System.Net.Http;
using System.Diagnostics;
using BooruSharp.Search.Post;

namespace rever
{
    public class ImageProvider
    {
        ABooru[] boorus = new ABooru[1];
        HttpClient downloader;
        Random random;
        public ImageProvider()
        {
            downloader = new HttpClient();
            random = new();

            boorus[0] = new Yandere();
            boorus[0].HttpClient = downloader;
        }
        private string[] getrandomtags(params string[] tags)
        {
            //we can take only 3 tags
            List<string> selectedtags = new List<string>();
            selectedtags.Capacity = tags.Length;
            for(int i = 0; i<Math.Min(tags.Length, random.Next(1, 6)); i++)
            {
                selectedtags.Add(tags[random.Next(tags.Length)]);
            }
            return selectedtags.ToArray();
        }
        public async Task<PostInfo> GetImageStream(params string[] tags)
        {
#if DEBUG
            Stopwatch starttime = new Stopwatch();
            starttime.Start();
            Console.WriteLine($"Try to get image...");
#endif
            string[] finaltags = getrandomtags(tags);
#if DEBUG
            Console.WriteLine($"Selected tags: {Newtonsoft.Json.JsonConvert.SerializeObject(finaltags)}");
#endif
            SearchResult target;
            do
            {
                try
                {
                    target = await boorus[0].GetRandomPostAsync(finaltags);
                }
                catch(BooruSharp.Search.InvalidTags fall)
                {
                    //clear array from last element
                    string[] buffer = new string[finaltags.Length - 1];
                    for(int i = 0; i < buffer.Length; i++)
                    {
                        buffer[i] = finaltags[i];
                    }
                    finaltags = buffer;

#if DEBUG
                    Console.WriteLine("Remove one last tag...");
                    Console.WriteLine($"Now selected tags: {Newtonsoft.Json.JsonConvert.SerializeObject(finaltags)}");
#endif
                    continue;
                }
                catch(Exception e)
                {
                    throw e;
                }
                break;
            }
            while (true);

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
