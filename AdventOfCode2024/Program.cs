using System.Diagnostics;
using AdventOfCode2024.Commands;
using Spectre.Console;
using ICommand = AdventOfCode2024.Commands.ICommand;

namespace AdventOfCode2024
{
    internal class Program
    {
        static void Main(string[] args)
        {

            var availableCommands = Utils.GetAvailableCommands();
            var parser = new CommandParser(availableCommands);

            parser.ParseCommand(["Cls"]).Run();
            parser.ParseCommand(["Sysinfo"]).Run();
            parser.ParseCommand(["Welcome"]).Run();

            Settings.ShowPuzzleText = !Debugger.IsAttached;
            parser.ParseCommand(["RunPuzzle", "Last"]).Run();
            Settings.ShowPuzzleText = false;


            ICommand? lastCommand = null;
            do
            {
                args = GetInput().Split(' ');

                if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
                    AnsiConsole.MarkupLine("[green]Type '[/][bold yellow]HELP[/][green]' to get help with available commands[/]");
                else
                {
                    ICommand? command = parser.ParseCommand(args);
                    if (command != null)
                    {
                        lastCommand = command;
                        command.Run();
                    }
                }
            } while (lastCommand == null || lastCommand.GetType() != typeof(QuitCommand));
        }

        static string GetInput()
        {
            AnsiConsole.MarkupLineInterpolated($"\n[dim italic]Logging:[/] {(Settings.ShowLog ? "On" : "Off")} [dim italic]Puzzle Text:[/]{(Settings.ShowPuzzleText ? "On" : "Off")}");
            var commandInput = AnsiConsole.Prompt(new TextPrompt<string>("[bold yellow]$>[/] ").AllowEmpty());

            return commandInput;
        }
    }
}

