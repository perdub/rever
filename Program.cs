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
            args = "--rating Questionable --token=5967832290:AAGzkcZdYyjBt0NHcSRYEX8Dqw4-hK-RCsc -c -1001251996661 --tags=genshin_impact%ass%pussy%open_shirt%pantsu%girls_und_panzer%animal_ears%loli%trap%wet%naked%bottomless%seifuku%undressing%uniform%bunny_ears%bunny_girl%nekomimi%neko%hatsune_miku%re_zero_kara_hajimeru_isekai_seikatsu%maid%honkai:_star_rail%wallpaper".Split();
            var par = Parser.Default.ParseArguments<Options>(args);
            TelegramBotClient client;
            if (!par.Errors.Any())
            {
                Options input = par.Value;
                client = new(input.Token);
                imageProvider = new();
                imageEditor = new();

                Rating target;
                if (!Enum.TryParse<Rating>(input.Rating, out target))
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
