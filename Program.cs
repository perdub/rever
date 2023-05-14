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
                imageProvider = new(input.UsePixiv ? input.PixivRefreshToken : null);
                imageEditor = new();

                await Bot(client, input.Channel, new SearchParams(input));
            }
        }
        static async Task Bot(TelegramBotClient bot, long channel, SearchParams search)
        {
            var me = await bot.GetMeAsync();
            Console.WriteLine("Bot id: " + me.Id);

            await Post(bot, channel, search);
        }
        static async Task Post(TelegramBotClient bot, long channel, SearchParams search)
        {
#if DEBUG
            Console.WriteLine($"Try to post to {channel} channel");
#endif
            PostInfo s = await imageProvider.GetImageStream(search);
            Stream final = await imageEditor.CompressImage(s.imageStream);
            var input = new Telegram.Bot.Types.InputFiles.InputOnlineFile(final);
            await bot.SendPhotoAsync(channel, input, parseMode:Telegram.Bot.Types.Enums.ParseMode.Html, caption:s.ToString());
        }
    }
}
