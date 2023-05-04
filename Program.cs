using System;
using System.Linq;
using CommandLine;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
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
                await Bot(client, input.Channel, input.Tags.ToArray());
            }
        }
        static async Task Bot(TelegramBotClient bot, long channel, string[] tags)
        {
            var me = await bot.GetMeAsync();
            Console.WriteLine("Bot id: " + me.Id);

            await Post(bot, channel, tags);
        }
        static async Task Post(TelegramBotClient bot, long channel, string[] tags)
        {
#if DEBUG
            Console.WriteLine($"Try to post to {channel} channel");
#endif
            PostInfo s = await imageProvider.GetImageStream(tags);
            Stream final = await imageEditor.CompressImage(s.imageStream);
            var input = new Telegram.Bot.Types.InputFiles.InputOnlineFile(final);
            await bot.SendPhotoAsync(channel, input, parseMode:Telegram.Bot.Types.Enums.ParseMode.Html, caption:s.ToString());
        }
    }
}
