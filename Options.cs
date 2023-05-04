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
        [Option('c', Required = true, HelpText = "Channel id")]
        public long Channel { get; set; }
    }
}
