namespace AdventOfCode2024.Commands
{
    public class SettingsCommand : ICommand, ICommandFactory
    {
        public string CommandName => "Set";

        public string CommandArgs => "[Log] | [PuzzleText]";

        public string[] CommandAlternates => [];

        private string setting = "";

        public string Description => "Toggles Settings:\n\t[Log] Show/Hide detailed Logging\n\t[PuzzleText] Show/Hide puzzle description";
        public ICommand MakeCommand(string[] args)
        {
            ICommand cmd = new SettingsCommand();

            if (args.Length < 2)
                cmd = new BadCommand("Usage: [Set Log] | [Set PuzzleText]");
            else
            {
                ((SettingsCommand)cmd).setting = args[1].ToLower();
            }

            return cmd;
        }

        public void Run()
        {
            if (setting == "log")
                Settings.ShowLog ^= true;

            if (setting == "puzzletext")
                Settings.ShowPuzzleText ^= true;
        }
    }
}
