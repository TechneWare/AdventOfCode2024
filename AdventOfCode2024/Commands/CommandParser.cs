namespace AdventOfCode2024.Commands
{
    internal class CommandParser(IEnumerable<ICommandFactory> commands)
    {
        private readonly IEnumerable<ICommandFactory> commands = commands;

        internal ICommand ParseCommand(string[] args)
        {
            var requestedCommand = args[0];

            var command = FindRequestedCommand(requestedCommand);
            if (command == null)
                return new NotFoundCommand { Name = requestedCommand };

            if (command is RunPuzzleCommand && double.TryParse(args[0], out double dayNum))
            {
                args = ["runpuzzle", $"{dayNum}"];
            }

            return command.MakeCommand(args);
        }

        private ICommandFactory? FindRequestedCommand(string requestedCommand)
        {
            ICommandFactory? cmd = null;
            var matched = commands.Where(c =>
                        c.CommandName.StartsWith(requestedCommand, StringComparison.CurrentCultureIgnoreCase)
                        || c.CommandAlternates.Any(ca => ca.StartsWith(requestedCommand, StringComparison.CurrentCultureIgnoreCase)));

            if (!matched.Any() && double.TryParse(requestedCommand, out double dayNum))
            {
                matched = commands.Where(c => c.CommandName.StartsWith("runpuzzle", StringComparison.CurrentCultureIgnoreCase));
            }

            if (matched.Any())
            {
                if (matched.Count() == 1)
                    cmd = matched.First();
                else
                {
                    var msg = "Please Be More Specific." +
                              "\nDid you mean one of these?";

                    foreach (var c in matched)
                        msg += $"\n=> {c.CommandName,-25}-{c.Description}";

                    cmd = new BadCommand(msg, new BadCommand(msg, new NotFoundCommand()));
                }
            }

            return cmd;
        }
    }
}
