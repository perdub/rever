using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
namespace rever
{
    //класс для парсинга входных данных через CommandLine
    public class Options
    {
        [Option("token", Required = true, HelpText = "Token to telegram bot.")]
        public string Token { get; set; }
        [Option('c', Required = true, HelpText = "Channel id.")]
        public long Channel { get; set; }

        //we use % symbol as separator because part of tags contains : symbol
        [Option("tags", Separator = '%', Required = false, HelpText = "Tags to find post. Note that we take only part of tags to find.")]
        public IEnumerable<string> Tags { get; set; }

        [Option("bannedtags", Separator = '%', Required = false, HelpText = "You don`t get output post with this tags.")]
        public IEnumerable<string> BannedTags { get; set; }

        [Option("rating", Required = false, Default = "Questionable", HelpText = "Rating to post.\nCan be General, Safe, Questionable and Explicit. By default - Safe.")]
        public string Rating { get; set; }

        [Option("maxiumtags", Required = false, Default = 10)]
        // максимальное количество тегов в запросе к конкретному апи
        public int MaxTagsToRequest {get;set;} 


        [Option("pixivrefresh", Required = false, HelpText = "Pixiv api refresh token. Need only if you want to use pixiv as source.")]
        public string PixivRefreshToken { set; get; }

        [Option("usepixiv", Default = false, HelpText = "true if ypu want to use pixiv as source. If true, you need to set --pixivrefresh. ")]
        public bool UsePixiv { get; set; }
        [Option("useyandere", Default = false )]
        public bool UseYandere { get; set; }
        [Option("usegelbooru", Default = false)]
        public bool UseGelbooru { get; set; }
        [Option("usedanboorudonmai", Default = false)]
        public bool UseDanbooruDonmai { get; set; }
        [Option("usesankakucomplex", Default = false)]
        public bool UseSankakuComplex { get; set; }
        [Option("usesakugabooru", Default = false, HelpText = "DON`T RECCOMEND TO USE")]
        public bool UseSakugabooru { get; set; }
        [Option("uselolibooru", Default = false)]
        public bool UseLolibooru { get; set; }

        [Option("useanimepictures", Default = false)]
        public bool UseAnimePictures{get;set;}

        [Option("mintagscount", Required =false, Default = 0, HelpText = "Minimal tags count in post")]
        public int MinTagsInPost {get;set;}

        [Option("grabtags", Required = false, Default =false)]
        public bool GrabTags{get;set;}

        [Option("disablecaptions", Required = false, Default = false)]
        public bool DisableCaptions {get;set;}
    }
}
