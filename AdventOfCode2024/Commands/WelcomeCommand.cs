using System.Drawing;
using Console = Colorful.Console;

namespace AdventOfCode2024.Commands
{
    public class WelcomeCommand : ICommand, ICommandFactory
    {
        public string CommandName => "Welcome";

        public string CommandArgs => "";

        public string[] CommandAlternates => [];

        public string Description => "Displays the Welcome Message";

        public bool WithLogging { get; set; }
        public ICommand MakeCommand(string[] args)
        {
            return new WelcomeCommand();
        }

        public void Run()
        {
            Console.WriteAscii("-----------------------", Color.White);
            Console.WriteAscii("Advent Of Code 2024", Color.Yellow);
            Console.WriteAscii("-----------------------", Color.White);
            Console.WriteLine("- By Brian Wham", Color.Yellow);
            Console.WriteLine("- https://github.com/TechneWare", Color.Yellow);
            Console.WriteLine();
        }
    }
}
