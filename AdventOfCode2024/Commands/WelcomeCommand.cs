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
            var table = new Table()
                .AddColumn(new TableColumn(""))
                .HideHeaders()
                .Expand()
                .Border(TableBorder.None);

            var panel = new Panel(new FigletText("Advent Of Code 2024")
                                 .Centered().Color(Spectre.Console.Color.Teal))
                .SquareBorder()
                .Expand()
                .BorderColor(Spectre.Console.Color.LightSkyBlue1);

            table.AddRow(panel);
            table.AddRow(new Markup("[green]-- By Brian Wham[/]"));
            table.AddRow(new Markup("[green]--[/] [italic dim link=https://github.com/TechneWare/AdventOfCode2024]https://github.com/TechneWare[/]"));
            AnsiConsole.Write(table);
            Console.WriteLine();
        }
    }
}
