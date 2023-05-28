using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats;

namespace rever
{
    public class ImageEditor
    {
        const int StreamMaxSize = 10 * 1024 * 1024;

        public async Task<Stream> CompressImage(Stream image)
        {
            using MemoryStream buffer = new MemoryStream();
            await image.CopyToAsync(buffer);
            buffer.Position = 0;

#if DEBUG
            Console.WriteLine($"Stream length - {buffer.Length}");
#endif

            using Image raw = (await Image.LoadAsync(buffer));

            if (raw.Width > 10000)
            {
                //compress by horizontaly
                raw.Mutate(x => x.Resize(10000, 0));
            }

            if (raw.Height > 10000)
            {
                //compress by verticaly
                raw.Mutate(x => x.Resize(0, 10000));
            }

            MemoryStream output = new MemoryStream();
            PngEncoder encoder = new PngEncoder { CompressionLevel = PngCompressionLevel.BestCompression };
            raw.Save(output, encoder);
            while (output.Length >= StreamMaxSize)
            {
                output.Position = 0;
                int newwidth = Convert.ToInt32(Math.Round(raw.Width * 0.8, MidpointRounding.ToEven));
                raw.Mutate(x => x.Resize(newwidth, 0));
                output.SetLength(0);
                raw.Save(output, encoder);
#if DEBUG
                Console.WriteLine($"New stream length - {output.Length}");
#endif
            }

            output.Position = 0;
            return output;
        }
    }
}
