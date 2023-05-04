using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace rever
{
    public class ImageEditor
    {
        const int StreamMaxSize = 10 * 1024 * 1024;

        public async Task<Stream> CompressImage(Stream image)
        {
            MemoryStream buffer = new MemoryStream();

            await image.CopyToAsync(buffer);
            buffer.Position = 0;

#if DEBUG
            Console.WriteLine($"Stream length - {buffer.Length}");
#endif

            var info = Image.Identify(buffer);
            buffer.Position = 0;
            Image raw = Image.Load(buffer);

            if (info.Width > 10000)
            {
                //compress by horizontaly
                raw.Mutate(x => x.Resize(10000, 0));
            }

            if (info.Height > 10000)
            {
                //compress by verticaly
                raw.Mutate(x => x.Resize(0, 10000));
            }
            MemoryStream output = new MemoryStream();
            raw.Save(output, new PngEncoder());
            while (output.Length >= StreamMaxSize)
            {
                output.Position = 0;
                int newwidth = Convert.ToInt32(Math.Round(raw.Width * 0.8, MidpointRounding.ToEven));
                raw.Mutate(x => x.Resize(newwidth, 0));
                raw.Save(output, new PngEncoder());
#if DEBUG
                Console.WriteLine($"New stream length - {buffer.Length}");
#endif
            }

            output.Position = 0;
            raw.Dispose();

            return output;
        }
    }
}
