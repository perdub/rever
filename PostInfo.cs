using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace rever
{
    public class PostInfo
    {
        public Stream imageStream { get; set; }
        public Uri postLink { get; set; }
        public string[] tags { get; set; }

        private static string clear(string tag)
        {
            //this function clear tag  from symbols like ( ) - ! and ets
            //thiis need because telegram dont add this symbols to tags
            //please, change this only IF YOU REALLY NEED THIS
            return tag.Replace('-', '_').Replace("(", "").Replace(")", "").Replace("&", "and").Replace(":", "").Replace("!", "").Replace('.','_');
        }

        public override string ToString()
        {
            StringBuilder sb = new();
            sb.Append($"<a href=\"{postLink.AbsoluteUri}\">Источник</a>");
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
