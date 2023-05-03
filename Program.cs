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
                await Bot(client, input.channels.GetEnumerator());
            }
        }
        static async Task Bot(TelegramBotClient bot, IEnumerator<long> channels)
        {
            var me = await bot.GetMeAsync();
            Console.WriteLine("Bot id: " + me.Id);

            List<Task> postTasks = new();
            while (channels.MoveNext())
            {
                postTasks.Add(Post(bot, channels.Current));
            }
            Task.WaitAll(postTasks.ToArray());
        }
        static async Task Post(TelegramBotClient bot, long channel)
        {
            PostInfo s = await imageProvider.GetImageStream();
            Stream final = await imageEditor.CompressImage(s.imageStream);
            var input = new Telegram.Bot.Types.InputFiles.InputOnlineFile(final);
            await bot.SendPhotoAsync(channel, input, parseMode:Telegram.Bot.Types.Enums.ParseMode.Html, caption:s.ToString());
        }
    }
}
