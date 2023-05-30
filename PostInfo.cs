﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace rever
{
    public class PostInfo
    {
        public Stream imageStream { get; set; }
        public Uri postLink { get; set; }
        public Uri fileLink { get; set; }
        public string[] tags { get; set; }

        public string telegramChannelName { get; set; }
        public string telegramChannelLink { get; set; }

        private static string clear(string tag) 
        // todo : add check if all symbols is numbers 
        {
            string tt;
            //this function clear tag  from symbols like ( ) - ! and ets
            //thiis need because telegram dont add this symbols to tags
            //please, change this only IF YOU REALLY NEED THIS
            if(tag[0]=='_')
            {
                tt = 't'+tag;
            }
            else{
                tt = tag;
            }
            tt = tt.Replace("&", "and").Replace("`","").Replace("・","").Replace("'","");
            return Regex.Replace(tt, @"[-():;>~+^<!./@]", "_");
        }

        public override string ToString()
        {
            StringBuilder sb = new();
            sb.Append($"<a href=\"{postLink.AbsoluteUri}\">Источник</a>|<a href=\"{fileLink.AbsoluteUri}\">Файл</a>\n\n<a href=\"{telegramChannelLink}\">{telegramChannelName}</a>".Replace("_", "%5F"));
            sb.AppendLine(); sb.AppendLine();
            foreach(var tag in tags)
            {
                sb.Append('#');
                sb.Append(clear(tag));
                sb.Append(' ');
            }
            return sb.ToString();
        }
    }
}
