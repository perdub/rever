using System;
using System.Linq;
using CommandLine;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using BooruSharp.Search.Post;
using Telegram.Bot;
using Telegram.Bot.Types;
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
                ActiveSources a = new ActiveSources{
                    Pixiv=input.UsePixiv,
                    Yandere=input.UseYandere,
                    Gelbooru=input.UseGelbooru,
                    DanbooruDonmai=input.UseDanbooruDonmai,
                    SankakuComplex=input.UseSankakuComplex,
                    Sakugabooru=input.UseSakugabooru,
                    Lolibooru=input.UseLolibooru
                };
                imageProvider = new(a, input.UsePixiv ? input.PixivRefreshToken : null);
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
            Chat chat = await bot.GetChatAsync(channel);
#if DEBUG
            Console.WriteLine($"Try to post to {channel} channel");
            Console.WriteLine($"Invite link: {chat.InviteLink}");
#endif
            PostInfo s = await imageProvider.GetImageStream(search);
            s.telegramChannelLink = chat.InviteLink;
            s.telegramChannelName = chat.Title;
            Stream final = await imageEditor.CompressImage(s.imageStream);
            var input = new Telegram.Bot.Types.InputFiles.InputOnlineFile(final);

            string caption = s.ToString();
            int messagesCount = (caption.Length / 4096) + 1;
            string[] messages = SplitByLength(caption, 4096);

            var message = await bot.SendPhotoAsync(channel, input, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html, caption: messages[0]);
            for (int i = 1; i < messagesCount; i++)
            {
                await bot.SendTextMessageAsync(channel, messages[i], Telegram.Bot.Types.Enums.ParseMode.Html, replyToMessageId: message.MessageId);
            }
        }
        static string[] SplitByLength(string text, int maxLength)
        {
            if (text == null) throw new ArgumentNullException("text");
            if (maxLength <= 0) throw new ArgumentOutOfRangeException("maxLength");
            List<string> lines = new List<string>();
            string[] words = text.Split(' ');
            StringBuilder currentLine = new StringBuilder();
            foreach (string word in words)
            {
                if (currentLine.Length == 0)
                {
                    currentLine.Append(word);
                }
                else if (currentLine.Length + 1 + word.Length <= maxLength)
                {
                    currentLine.Append(' ');
                    currentLine.Append(word);
                }
                else
                {
                    lines.Add(currentLine.ToString());
                    currentLine.Clear();
                    currentLine.Append(word);
                }
            }
            if (currentLine.Length > 0)
            {
                lines.Add(currentLine.ToString());
            }
            return lines.ToArray();
        }
    }
}
