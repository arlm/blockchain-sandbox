using CommandLine;

namespace BlockChain.CLI.Verbs
{
    public class BlockChainOptions : CommonCommands
    {
        public override string Heading => "👋  μChain Blockchain commands";

        [VerbOption("show", HelpText = "Shows the entire blockchain with data")]
        public ShowOptions Show { get; set; }

        [VerbOption("mine", HelpText = "Mines a new block")]
        public MineOptions Mine { get; set; }

        [VerbOption("peers", HelpText = "Shows connected peers")]
        public PeersOptions Peers { get; set; }

        [VerbOption("connect", HelpText = "Connects to a peer")]
        public ConnectOptions Connect { get; set; }

        [VerbOption("discover", HelpText = "Discovers new peers from your current peers")]
        public DiscoverOptions Discover { get; set; }

        [VerbOption("open", HelpText = "Opens port for connections")]
        public OpenOptions Open { get; set; }

        public class ShowOptions : CommonSubOptions
        {
            public override string Heading => "👋  μChain Blockchain Show command";

            [ValueOption(1)]
            public string Command { get; set; }
        }

        public class MineOptions : CommonSubOptions
        {
            public override string Heading => "👋  μChain Blockchain Mine command";

            [ValueOption(1)]
            public string Command { get; set; }
        }

        public class PeersOptions : CommonSubOptions
        {
            public override string Heading => "👋  μChain Blockchain Peers command";

            [ValueOption(1)]
            public string Command { get; set; }
        }

        public class ConnectOptions : CommonSubOptions
        {
            public override string Heading => "👋  μChain Blockchain Connect command";

            [ValueOption(1)]
            public string Command { get; set; }
        }

        public class DiscoverOptions : CommonSubOptions
        {
            public override string Heading => "👋  μChain Blockchain Discover command";

            [ValueOption(1)]
            public string Command { get; set; }
        }

        public class OpenOptions : CommonSubOptions
        {
            public override string Heading => "👋  μChain Blockchain Open command";
           
            [ValueOption(1)]
            public string Command { get; set; }
        }
    }
}
