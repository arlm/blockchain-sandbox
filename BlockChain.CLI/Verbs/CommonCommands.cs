using System;
using System.Diagnostics;
using System.Linq;
using CommandLine;
using CommandLine.Text;

namespace BlockChain.CLI.Verbs
{
    public abstract class CommonCommands : ICommand
    {
        public abstract string Heading { get; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        [HelpVerbOption]
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

            HelpText current;

            if (string.IsNullOrWhiteSpace(verb))
            {
                return current = DefaultHelp(verb);
            }

            (var property, var attribute) = this.RetrieveOptionProperty<VerbOptionAttribute>(verb);

            var verbObj = property?.GetValue(this) as ICommand;

            if (verbObj == null)
            {
                if (verb == "help")
                {
                    verb = null;
                }

                return current = DefaultHelp(verb);
            }

            current = HelpText.AutoBuild(verbObj);
            current.Heading = verbObj.Heading;

            if (!string.IsNullOrWhiteSpace(verb) && verb != "help")
                current.AddPreOptionsLine($"Available commands for \"{verb}\":");
            else
                current.AddPreOptionsLine("Available commands:");

            current.AddPreOptionsLine(string.Empty);

            return current;
        }

        private HelpText DefaultHelp(string verb)
        {
            HelpText current = HelpText.AutoBuild(this, verb);
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
