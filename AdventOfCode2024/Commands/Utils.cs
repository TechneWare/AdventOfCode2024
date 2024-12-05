using AdventOfCode2024.Puzzles;
using Spectre.Console;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace AdventOfCode2024.Commands
{
    internal static class Utils
    {
        /// <summary>
        /// </summary>
        /// <returns>All command objects that are marked public</returns>
        public static IEnumerable<ICommandFactory> GetAvailableCommands()
        {
            var type = typeof(ICommandFactory);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && p.IsClass && p.IsPublic)
                .Select(t => Activator.CreateInstance(type: t) as ICommandFactory);

            return types != null ? (IEnumerable<ICommandFactory>)types : [];
        }

        /// <summary>
        /// </summary>
        /// <returns>All Puzzles</returns>
        public static IEnumerable<IPuzzle> GetAllPuzzles()
        {
            var type = typeof(Puzzle);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && p.Name != type.Name)
                .Select(t => Activator.CreateInstance(type: t) as IPuzzle);

            return types != null ? (IEnumerable<IPuzzle>)types : [];
        }
        public static void PrintUsage(IEnumerable<ICommandFactory> availableCommands)
        {
            AnsiConsole.MarkupLine("\n[bold yellow]Usage:[/] [italic][[Command Name]] [[Arguments]][/]");

            var t = new Table()
            .Title($"[bold yellow on blue]Available Commands[/]")
            .AddColumns(
                "[bold yellow]Command[/]",
                "[bold yellow]Alternates[/]",
                "[bold yellow]Arguments[/]",
                "[bold yellow]Description[/]")
            .RoundedBorder()
            .BorderColor(Color.LightSlateBlue);

            foreach (var command in availableCommands)
            {
                string alts = "";
                if (command.CommandAlternates.Length != 0)
                    alts = string.Join(" | ", command.CommandAlternates);

                var args = command.CommandArgs.Replace("[", "[[").Replace("]", "]]");
                var desc = command.Description.Replace("[", "[[").Replace("]", "]]");
                alts = alts.Replace("[", "[[").Replace("]", "]]");

                t.AddRow(command.CommandName, alts, args, desc);
            }
            AnsiConsole.Write(t);
            AnsiConsole.MarkupLine("[dim italic]You can also run a command by typeing the first letter(s) that uniquely identify that command[/]");
            AnsiConsole.MarkupLine("[dim italic]EG: 'L' will match 'List', 'Q' will match 'Quit'[/]");
        }
        public static void PrintCommandUsage(ICommandFactory command)
        {
            var t = new Table()
            .AddColumns(
                "[bold yellow]Command[/]",
                "[bold yellow]Alternates[/]",
                "[bold yellow]Arguments[/]",
                "[bold yellow]Description[/]")
            .RoundedBorder()
            .BorderColor(Color.LightSlateBlue);

            string alts = "";
            string ext = "";

            if (command.CommandAlternates.Length != 0)
                alts = string.Join(" | ", command.CommandAlternates);

            if (!string.IsNullOrEmpty(command.ExtendedDescription))
                ext = $"\n{command.ExtendedDescription}";

            var args = command.CommandArgs.Replace("[", "[[").Replace("]", "]]");
            var desc = $"{command.Description}{ext}".Replace("[", "[[").Replace("]", "]]");
            alts = alts.Replace("[", "[[").Replace("]", "]]");

            t.AddRow(command.CommandName, alts, args, desc);

            AnsiConsole.Write(t);
        }
    }
}
