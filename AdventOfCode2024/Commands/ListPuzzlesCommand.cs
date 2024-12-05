using Spectre.Console;

namespace AdventOfCode2024.Commands
{
    public class ListPuzzlesCommand : ICommand, ICommandFactory
    {
        public string CommandName => "List";

        public string CommandArgs => "";

        public string[] CommandAlternates => [];

        public string Description => "Lists All Available Puzzles";
        public string ExtendedDescription => "For Example:\n" +
                                             "Day:1   Name:Historian Hysteria\n" +
                                             "Day:2   Name:Red-Nosed Reports\n" +
                                             "Day:3   Name:Mull It Over\n" +
                                             "Day:4   Name:Ceres Search\n" +
                                             "Day:5   Name:Print Queue\n" +
                                             "\nYou can run a puzzle by entering its day number";

        public bool WithLogging { get; set; } = false;

        public ICommand MakeCommand(string[] args)
        {
            return new ListPuzzlesCommand();
        }

        public void Run()
        {
            foreach (var p in Utils.GetAllPuzzles().OrderBy(p => p.DayNumber))
                AnsiConsole.MarkupInterpolated($"[green]Day:[/][bold yellow]{p.DayNumber}[/]\t[green]Name:[/][bold yellow]{p.Name}[/]\n");
        }
    }
}
