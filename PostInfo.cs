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


        public override string ToString()
        {
            StringBuilder sb = new();
            sb.Append($"<a href=\"{postLink.AbsoluteUri}\">Источник</a>");
            sb.AppendLine(); sb.AppendLine();
            foreach(var tag in tags)
            {
                sb.Append('#');
                sb.Append(tag);
                sb.Append(' ');
            }
            return sb.ToString();
        }
    }
}
