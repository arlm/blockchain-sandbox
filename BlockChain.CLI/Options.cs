using CommandLine;
using CommandLine.Text;

namespace BlockChain.CLI
{
    class Options
    {
        [ValueOption(1)]
        public string Command { get; set; }

        [ValueOption(2)]
        public string Data { get; set; }

        [Option("verbose", DefaultValue = true, HelpText = "Prints all messages to standard output.")]
        public bool Verbose { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, (HelpText current) =>
              {
                  current.Heading = "Blockchain CLI";
                  current.AddPreOptionsLine("Command-line verbs:");
                  current.AddPreOptionsLine(string.Empty);
                  current.AddPreOptionsLine("  blockchain\t\tShows the entire blockchain with data");
                  current.AddPreOptionsLine("  mine \"block data\"\t\tMines a new block");
                  current.AddPreOptionsLine("  peers\t\t\tShows connected peers");
                  current.AddPreOptionsLine("  connect\t\tConnects to a peer");
                  current.AddPreOptionsLine("  discover\t\tDiscovers new peers");
                  current.AddPreOptionsLine("  open\t\t\tOpens port for connections");
                  current.AddPreOptionsLine(string.Empty);
                  current.AddPreOptionsLine("Command-line options:");
              });
        }
    }
}
