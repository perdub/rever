using System;
using System.Linq;
using CommandLine;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using BooruSharp.Search.Post;
using Telegram.Bot;
namespace rever
{
    class Program
    {
        static ImageProvider imageProvider;
        static ImageEditor imageEditor;
        async static Task Main(string[] args)
        {
            var par = Parser.Default.ParseArguments<Options>(args);
            TelegramBotClient client;
            if (!par.Errors.Any())
            {
                Options input = par.Value;
                client = new(input.Token);
                imageProvider = new();
                imageEditor = new();

                Rating target;
                try
                {
                    target = (Rating)Enum.Parse(typeof(Rating), input.Rating);
                }
                catch (System.ArgumentException)
                {
                    target = Rating.Safe;
                }

                await Bot(client, input.Channel, input.Tags.ToArray(), target);
            }
        }
        static async Task Bot(TelegramBotClient bot, long channel, string[] tags, Rating target)
        {
            var me = await bot.GetMeAsync();
            Console.WriteLine("Bot id: " + me.Id);

            await Post(bot, channel, tags, target);
        }
        static async Task Post(TelegramBotClient bot, long channel, string[] tags, Rating target)
        {
#if DEBUG
            Console.WriteLine($"Try to post to {channel} channel");
#endif
            PostInfo s = await imageProvider.GetImageStream(target, tags);
            Stream final = await imageEditor.CompressImage(s.imageStream);
            var input = new Telegram.Bot.Types.InputFiles.InputOnlineFile(final);
            await bot.SendPhotoAsync(channel, input, parseMode:Telegram.Bot.Types.Enums.ParseMode.Html, caption:s.ToString());
        }
    }
}
