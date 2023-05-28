﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rever
{
    public class SearchParams
    {
        public SearchParams(BooruSharp.Search.Post.Rating rating, string[] tags, string[] bannedtags)
        {
            // todo: add in this constructor MintagsInPost
            this.rating = rating;
            this.tags = tags;
            this.bannedtags = bannedtags;
        }
        public SearchParams(Options s)
        {
            BooruSharp.Search.Post.Rating r;
            if (!Enum.TryParse<BooruSharp.Search.Post.Rating>(s.Rating, out r))
            {
                r = BooruSharp.Search.Post.Rating.Safe;
            }
            this.rating = r;
            this.tags = s.Tags.ToArray();
            this.bannedtags = s.BannedTags.ToArray();
            this.mintags = s.MinTagsInPost;
        }
        public BooruSharp.Search.Post.Rating rating { get; private set; }
        public string[] tags {get;private set;}
        public string[] bannedtags { get; private set; }
        public int mintags {get;set;}
    }
}
