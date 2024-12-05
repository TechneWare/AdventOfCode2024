using Spectre.Console;

namespace AdventOfCode2024.Commands
{
    internal class BadCommand : ICommand, ICommandFactory
    {
        public string Message { get; set; }
        public ICommand SourceCommand { get; }

        public string CommandName => "BadCommand";

        public string CommandArgs => "";

        public string[] CommandAlternates => [];

        public string Description => "Internal: Used for bad commands";
        public bool WithLogging { get; set; } = false;

        public string ExtendedDescription => "";

        public BadCommand() { Message = ""; SourceCommand = null; }
        public BadCommand(string message, ICommand sourceCommand)
        {
            Message = message;
            SourceCommand = sourceCommand;
        }

        public void Run()
        {
            AnsiConsole.MarkupLine("[bold red rapidblink]===> Bad Command <===[/]");
            AnsiConsole.MarkupLineInterpolated($"[italic bold yellow]{Message}[/]");
        }

        public ICommand MakeCommand(string[] args)
        {
            return this;
        }
    }
}
