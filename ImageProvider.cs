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
        ABooru[] boorus = new ABooru[3];
        HttpClient downloader;
        Random random;
        public ImageProvider()
        {
            downloader = new HttpClient();
            random = new();

            boorus[0] = new Yandere();
            boorus[0].HttpClient = downloader;
            boorus[1] = new DanbooruDonmai();
            boorus[1].HttpClient = downloader;
            boorus[2] = new Gelbooru();
            boorus[2].HttpClient = downloader;
        }
        private string[] getrandomtags(params string[] tags)
        {
            List<string> selectedtags = new List<string>();
            selectedtags.Capacity = tags.Length;
            for(int i = 0; i<Math.Min(tags.Length, random.Next(1, 6)); i++)
            {
                selectedtags.Add(tags[random.Next(tags.Length)]);
            }
            return selectedtags.ToArray();
        }
        public async Task<PostInfo> GetImageStream(Rating rating, params string[] tags)
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
            int ratingbadresult = 0;
            ABooru source = boorus[random.Next(boorus.Length)];
#if DEBUG
            Console.WriteLine($"Booru source base url: {source.BaseUrl}");
#endif
            do
            {
                try
                {
                    if (source.NoMoreThanTwoTags)
                    {
                        if (finaltags.Length != 0)
                        {
                            string[] buffer = new string[1];
                            buffer[0] = finaltags[0];
                            finaltags = buffer;
                        }
                    }
                    target = await source.GetRandomPostAsync(finaltags);
                    if ((int)target.Rating > (int)rating)
                    {
                        ratingbadresult++;
                        if (ratingbadresult > 9)
                        {
                            finaltags = getrandomtags(tags);
#if DEBUG
                            Console.WriteLine($"Edit selected tags: {Newtonsoft.Json.JsonConvert.SerializeObject(finaltags)}");
#endif
                            ratingbadresult = 0;
                        }
#if DEBUG
                        Console.WriteLine("Censored!");
#endif
                        continue;
                    }
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
                    throw;
                }
                break;
            }
            while (true);

            PostInfo result = new PostInfo();
            Stream raw = await downloader.GetStreamAsync(target.FileUrl);
            result.imageStream = raw;
            result.fileLink = target.FileUrl;
            result.postLink = target.PostUrl;
            result.tags = target.Tags.ToArray();
#if DEBUG
            starttime.Stop();
            Console.WriteLine($"Image load, elapsed time - {starttime.Elapsed}");
#endif
            return result;
        }
    }
}
