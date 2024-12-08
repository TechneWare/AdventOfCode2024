using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2024.Commands
{
    public class SysInfoCommand : ICommand, ICommandFactory
    {
        public string CommandName => "Sysinfo";

        public string CommandArgs => "";

        public string[] CommandAlternates => [];

        public string Description => "Displays System Information";
        public string ExtendedDescription => "System information is used to detect systems that cannot handle heavy puzzles." +
                                             "This allows a way to skip sections that might crash on low power systems.";
        public bool WithLogging { get; set; }

        public ICommand MakeCommand(string[] args)
        {
            return new SysInfoCommand();
        }
        public void Run()
        {
            Console.WriteLine();
            AnsiConsole.MarkupLineInterpolated($"Procssors: {SysInfo.ProcessorCount} Memory: {SysInfo.TotalMemoryMB} MB");
            Console.WriteLine();
        }
    }
}
