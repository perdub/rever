using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rever
{
    //класс, описывающий параметры поиска в апи
    public class SearchParams
    {
        public SearchParams(Options s)
        {
            //парсинг рейтинга
            BooruSharp.Search.Post.Rating r;
            if (!Enum.TryParse<BooruSharp.Search.Post.Rating>(s.Rating, out r))
            {
                r = BooruSharp.Search.Post.Rating.Safe;
            }
            //присваивание
            this.rating = r;
            this.tags = s.Tags.ToArray();
            this.bannedtags = s.BannedTags.ToArray();
            this.mintags = s.MinTagsInPost;

            //включен ли параметр для загрузки этих тегов?
            if(s.GrabTags){
                grabTags();
            }

            lowcase();
        }
        //приводит все теги к маленьким буквам
        void lowcase(){
            tags = tags.Select(x => x.ToLower()).ToArray();
            bannedtags = bannedtags.Select(x => x.ToLower()).ToArray();
        }

        //загрузка и добавление статичных тегов из tags.tags и bannedtags.tags
        void grabTags(){
            tags = tags.Concat(System.IO.File.ReadLines("bin/Debug/net7.0/tags.tags")).ToArray();
            bannedtags = bannedtags.Concat(System.IO.File.ReadLines("bin/Debug/net7.0/bannedtags.tags")).ToArray();
        }

        //допустимый рейтинг
        public BooruSharp.Search.Post.Rating rating { get; private set; }
        //массив тегов для поиска
        public string[] tags {get;private set;}
        //массив запрещённых тегов                  подробнее про теги будет написанно в ImageProvider
        public string[] bannedtags { get; private set; }
        // минимальное количество тегов у поста для того что бы он мог быть результатом вызова(если у него меньше, мы ищем другой пост)
        public int mintags {get;set;}
    }
}
