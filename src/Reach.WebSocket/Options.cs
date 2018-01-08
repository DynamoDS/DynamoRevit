using CommandLine;
using CommandLine.Text;

namespace Reach.WebSocket
{
    public class Options
    {
        [Option('v', "verbose", DefaultValue = false,
            HelpText = "Prints all messages to standard output.")]
        public bool Verbose { get; set; }

        [Option('x', "autoexit", DefaultValue = false,
            HelpText = "Whether to exit the executable when the first session terminates")]
        public bool AutoExit { get; set; }

        [Option('t', "token", DefaultValue = false,
            HelpText = "Disable token validation")]
        public bool DisableTokenValidator { get; set; }

        [Option('a', "tokenaddress", DefaultValue = null,
            HelpText = "Override token validator address provided from the config file.")]
        public string TokenValidatorAddress { get; set; }

        [Option('g', "geometry", DefaultValue = false,
            HelpText = "Disable searching for shape manager binaries from installed applications. Load the shape manager binaries from the root directory instead.")]
        public bool LoadShapeManagerFromRoot { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
                (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
