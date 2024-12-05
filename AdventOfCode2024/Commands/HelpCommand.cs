using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2024.Commands
{
    public class HelpCommand : ICommand, ICommandFactory
    {
        private readonly string[] args;

        public string CommandName => "Help";

        public string CommandArgs => "commandName";

        public string[] CommandAlternates => ["h", "?"];

        public string Description => "Displays detailed help for a command";

        public string ExtendedDescription => "Specify a command name to get help on that command.\n" +
                                             "EG: Help [CommandName] will display the details of a command.";

        public HelpCommand()
        {
            args = [];
        }

        public HelpCommand(string[] args)
        {
            //Save all args that do not point at this command
            this.args = args.Skip(1).ToArray();
        }

        public ICommand MakeCommand(string[] args)
        {
            if (args.Length == 0)
                return new HelpCommand();
            else
                return new HelpCommand(args);
        }

        public void Run()
        {
            if (this.args.Length == 0)
                Utils.PrintUsage(Utils.GetAvailableCommands());
            else
            {
                var parser = new CommandParser(Utils.GetAvailableCommands());
                var cmd = parser.ParseCommand(this.args);

                if (cmd is BadCommand command)
                    cmd = command.SourceCommand;

                if (cmd is BadCommand || cmd is NotFoundCommand)
                    cmd.Run();
                else if (cmd != null)
                {
                    Utils.PrintCommandUsage((ICommandFactory)cmd);
                }
            }
        }
    }
}
