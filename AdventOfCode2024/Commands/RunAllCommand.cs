namespace AdventOfCode2024.Commands
{
    public class RunAllCommand : ICommand, ICommandFactory
    {
        public string CommandName => "RunAll";

        public string CommandArgs => "";

        public string[] CommandAlternates => new string[] { "all" };

        public string Description => "Run All Puzzles";
        public ICommand MakeCommand(string[] args)
        {
            return new RunAllCommand();
        }

        public void Run()
        {
            var allPuzzles = Utils.GetAllPuzzles().OrderBy(p => p.DayNumber);
            foreach (var puzzle in allPuzzles)
                puzzle.Run();
        }
    }
}
