using Spectre.Console;

namespace AdventOfCode2024.Commands
{
    internal class NotFoundCommand : ICommand
    {
        public bool WithLogging { get; set; } = false;
        public string Name { get; set; }
        public void Run()
        {
            AnsiConsole.MarkupLineInterpolated($"[bold red]Command Not Found => [/][bold yellow]{Name}[/]");
        }
    }
}
