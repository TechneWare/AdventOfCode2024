using Spectre.Console;

namespace AdventOfCode2024.Commands
{
    public class QuitCommand : ICommand, ICommandFactory
    {
        public string CommandName => "Quit";
        public string CommandArgs => "";
        public string[] CommandAlternates => new string[] { "exit" };
        public string Description => "Ends the program";
        public string ExtendedDescription => "";
        public bool WithLogging { get; set; } = false;
        public ICommand MakeCommand(string[] args)
        {
            return new QuitCommand();
        }

        public void Run()
        {
            AnsiConsole.MarkupLine("[bold yellow]Laterz[/]");
        }
    }
}
