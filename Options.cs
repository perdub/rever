using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
namespace rever
{
    public class Options
    {
        [Option("token", Required = true, HelpText = "Token to telegram bot")]
        public string Token { get; set; }
        [Option('c', Separator = ':', Required = false, HelpText = "Channels ids")]
        public IEnumerable<long> channels { get; set; }
    }
}
