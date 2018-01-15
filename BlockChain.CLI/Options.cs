using BlockChain.CLI.Verbs;
using CommandLine;

namespace BlockChain.CLI
{
    class Options : CommonCommands
    {
        public override string Heading => "👋  Welcome to μChain CLI";

        [VerbOption("blockchain", HelpText = "Basic blockchain commands")]
        public BlockChainOptions BlockChainVerb { get; set; }

        [VerbOption("bitcoin", HelpText = "Bitcoin commands")]
        public BitcoinOptions BitcoinVerb { get; set; }
    }
}
