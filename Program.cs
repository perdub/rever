using System;
using System.Linq;
using CommandLine;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
namespace rever
{
    class Program
    {
        static ImageProvider imageProvider;
        static ImageEditor imageEditor;
        static bool _disablecaptions;
        async static Task Main(string[] args)
        {
            //парсинг параметров
            var par = Parser.Default.ParseArguments<Options>(args);
            TelegramBotClient client;

            if (!par.Errors.Any())
            {
                //создание вспомогательных обьектов и вызов Bot()
                Options input = par.Value;
                client = new(input.Token);
                ActiveSources a = new ActiveSources{
                    Pixiv=input.UsePixiv,
                    Yandere=input.UseYandere,
                    Gelbooru=input.UseGelbooru,
                    DanbooruDonmai=input.UseDanbooruDonmai,
                    SankakuComplex=input.UseSankakuComplex,
                    Sakugabooru=input.UseSakugabooru,
                    Lolibooru=input.UseLolibooru,

                    AnimePictures = input.UseAnimePictures,
                    Rule34 = input.UseRule34
                };
                imageProvider = new(a, input.UsePixiv ? input.PixivRefreshToken : null, input.MaxTagsToRequest);
                imageEditor = new();
                _disablecaptions = input.DisableCaptions;

                await Bot(client, input.Channel, new SearchParams(input));

            }
        }
        static async Task Bot(TelegramBotClient bot, long channel, SearchParams search)
        {
            //метод-обёртка(честно говоря я не знаю зачем он нужен)
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
            //запрос к imageProvider для получения поста
            PostInfo s = await imageProvider.GetImageStream(search);

            //добавление данных о тгк в postinfo
            s.telegramChannelLink = chat.InviteLink;
            s.telegramChannelName = chat.Title;

            //сжатие изображения(если надо)
            Stream final = await imageEditor.CompressImage(s.imageStream);
            
            //подготовка входного файла
            var input = InputFile.FromStream(final);

            //получение и разбивка текста поста на части(для того что бы апи телеграмма не ругалось)
            string caption = _disablecaptions ? "" : s.ToString();
            int messagesCount = (caption.Length / 1024 ) + 1;
            string[] messages = SplitByLength(caption, 1024);

            //отправление фото, а затем лополнительных частей текста(если весь текст не влазит в один пост)
            var message = await bot.SendPhotoAsync(channel, input, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html, caption: _disablecaptions ? "" :messages[0]);
            for (int i = 1; i < messagesCount; i++)
            {
                await bot.SendTextMessageAsync(channel, messages[i], parseMode: Telegram.Bot.Types.Enums.ParseMode.Html, replyToMessageId: message.MessageId);
            }
        }

        //split one string by length
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
