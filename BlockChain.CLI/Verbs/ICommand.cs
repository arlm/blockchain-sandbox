using System;
using CommandLine;

namespace BlockChain.CLI.Verbs
{
    public interface ICommand
    {
        string Heading { get; }

        IParserState LastParserState { get; set; }

        string GetUsage(string verb);
    }
}
