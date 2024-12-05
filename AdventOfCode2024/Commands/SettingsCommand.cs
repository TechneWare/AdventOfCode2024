namespace AdventOfCode2024.Commands
{
    public class SettingsCommand : ICommand, ICommandFactory
    {
        public string CommandName => "Set";

        public string CommandArgs => "[Log] | [PuzzleText]";

        public string[] CommandAlternates => [];

        private string setting = "";

        public string Description => "Toggles A Setting";
        public string ExtendedDescription => "'Set Log' Enable/Disable Log output (may become depricated)\n" +
                                             "'Set PuzzleText' Enable/Disable showing puzzle description when it runs\n" +
                                             "- EG: If PuzzleText is ON when the 'RunAll' command is used\n" +
                                             "      each days puzzle text will be displayed for part 1 & 2\n" +
                                             "      as it is executed.";

        public ICommand MakeCommand(string[] args)
        {
            ICommand cmd = new SettingsCommand();

            if (args.Length < 2)
                cmd = new BadCommand("Usage: [Set Log] | [Set PuzzleText]", this);
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
