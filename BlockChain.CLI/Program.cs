using System;
using BlockChain.Core;
using CommandLine;

namespace BlockChain.CLI
{
    class Program
    {
        private static Chain blockchain = new Chain();

        static void Main(string[] args)
        {
            var options = new Options();

            if (Parser.Default.ParseArguments(args, options))
            {
                if (options.Verbose && !string.IsNullOrWhiteSpace(options.Command))
                {
                    Console.Out.WriteLine($"Running {options.Command} verb [{options.Data}]\n");

                }

                if (options.Command == "mine" ^ !string.IsNullOrWhiteSpace(options.Data))
                {
                    Console.Out.WriteLine(options.GetUsage());
                    return;
                }

                switch (options.Command)
                {
                    case "blockchain":
                        Console.Out.WriteLine(blockchain.Dump());
                        break;
                    case "mine":
                        var newBlock = blockchain.GenerateNextBlock(options.Data);
                        blockchain.Add(newBlock);
                        Console.Out.WriteLine($"Successfully added {options.Data} to the blockchain");
                        break;
                    case "peers":
                    case "connect":
                    case "discover":
                    case "open":
                        Console.Out.WriteLine("Not implemented yet!");
                        break;
                    default:
                        Console.Out.WriteLine(options.GetUsage());
                        break;
                }
            }
        }
    }
}
