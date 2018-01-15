using CommandLine;

namespace BlockChain.CLI.Verbs
{
    public class BitcoinOptions : CommonCommands
    {
        public override string Heading => "👋  μChain Bitcoin commands";

        [VerbOption("show", HelpText = "Shows the entire Bitcoin with data")]
        public ShowOptions Show { get; set; }

        [VerbOption("mine", HelpText = "Mines a new Bitcoin block")]
        public MineOptions Mine { get; set; }

        [VerbOption("peers", HelpText = "Shows connected Bitcoin peers")]
        public PeersOptions Peers { get; set; }

        [VerbOption("connect", HelpText = "Connects to a Bitcoin peer")]
        public ConnectOptions Connect { get; set; }

        [VerbOption("discover", HelpText = "Discovers new Bitcoin peers from your current peers")]
        public DiscoverOptions Discover { get; set; }

        [VerbOption("open", HelpText = "Opens port for Bitcoin connections")]
        public OpenOptions Open { get; set; }


        public class ShowOptions : CommonSubOptions
        {
            public override string Heading => "👋  μChain Bitcoin Show commands";

            [ValueOption(1)]
            public string Command { get; set; }
        }

        public class MineOptions : CommonSubOptions
        {
            public override string Heading => "👋  μChain Bitcoin Mine commands";

            [ValueOption(1)]
            public string Command { get; set; }
        }

        public class PeersOptions : CommonSubOptions
        {
            public override string Heading => "👋  μChain Bitcoin Peers commands";

            [ValueOption(1)]
            public string Command { get; set; }
        }

        public class ConnectOptions : CommonSubOptions
        {
            public override string Heading => "👋  μChain Bitcoin Connect commands";

            [ValueOption(1)]
            public string Command { get; set; }
        }

        public class DiscoverOptions : CommonSubOptions
        {
            public override string Heading => "👋  μChain Bitcoin Discover commands";

            [ValueOption(1)]
            public string Command { get; set; }
        }

        public class OpenOptions : CommonSubOptions
        {
            public override string Heading => "👋  μChain Bitcoin Open commands";

            [ValueOption(1)]
            public string Command { get; set; }
        }
    }
}
