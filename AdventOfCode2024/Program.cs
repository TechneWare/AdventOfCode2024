using System.Diagnostics;
using AdventOfCode2024.Commands;
using ICommand = AdventOfCode2024.Commands.ICommand;

namespace AdventOfCode2024
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var availableCommands = Utils.GetAvailableCommands();
            var parser = new CommandParser(availableCommands);

            parser.ParseCommand(new string[] { "Cls" }).Run();
            parser.ParseCommand(new string[] { "Welcome" }).Run();

            Settings.ShowPuzzleText = !Debugger.IsAttached;
            parser.ParseCommand(new string[] { "RunPuzzle", "Last" }).Run();
            Settings.ShowPuzzleText = false;

            ICommand? lastCommand = null;
            do
            {
                args = GetInput().Split(' ');

                if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
                    Utils.PrintUsage(availableCommands);
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
            Console.WriteLine($"\nLogging: {(Settings.ShowLog ? "On" : "Off")}\t Puzzle Text:{(Settings.ShowPuzzleText ? "On" : "Off")}");
            Console.Write("$> ");
            string commandInput = Console.ReadLine();
            return commandInput;
        }
    }
}

