using Spectre.Console;
using Console = Colorful.Console;

namespace AdventOfCode2024.Commands
{
    public class WelcomeCommand : ICommand, ICommandFactory
    {
        public string CommandName => "Welcome";

        public string CommandArgs => "";

        public string[] CommandAlternates => [];

        public string Description => "Displays the Welcome Message";
        public string ExtendedDescription => "";
        public bool WithLogging { get; set; }

        public ICommand MakeCommand(string[] args)
        {
            return new WelcomeCommand();
        }

        public void Run()
        {
            Console.WriteAscii("-----------------------", System.Drawing.Color.White);
            Console.WriteAscii("Advent Of Code 2024", System.Drawing.Color.Yellow);
            Console.WriteAscii("-----------------------", System.Drawing.Color.White);
            
            AnsiConsole.MarkupLine("[bold green]- By Brian Wham[/]");
            AnsiConsole.MarkupLine("[underline italic dim link=https://github.com/TechneWare/AdventOfCode2024]https://github.com/TechneWare[/]");
            Console.WriteLine();
        }
    }
}
