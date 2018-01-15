using System;
using System.Linq;
using CommandLine;
using CommandLine.Text;

namespace BlockChain.CLI.Verbs
{
    public abstract class CommonSubOptions : ICommand
    {
        public abstract string Heading { get; }

        [Option('q', "quiet", HelpText = "Suppress summary message.")]
        public bool Quiet { get; set; }

        [Option("verbose", DefaultValue = false, HelpText = "Prints all messages to standard output.")]
        public bool Verbose { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage(string verb)
        {

            if (this.LastParserState?.Errors.Any() ?? false)
            {
                var help = new HelpText();

                var errors = help.RenderParsingErrorsText(this, 2); // indent with two spaces

                if (!string.IsNullOrEmpty(errors))
                {
                    help.AddPreOptionsLine(string.Concat(Environment.NewLine, "ERROR(S):"));
                    help.AddPreOptionsLine(errors);
                }

                return help;
            }

            var current = HelpText.AutoBuild(this);

            current.Heading = this.Heading;

            if (!string.IsNullOrWhiteSpace(verb) && verb != "help")
                current.AddPreOptionsLine($"Available commands for \"{verb}\":");
            else
                current.AddPreOptionsLine("Available commands:");

            current.AddPreOptionsLine(string.Empty);

            return current;
        }
    }
}
