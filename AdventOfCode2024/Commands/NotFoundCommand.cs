namespace AdventOfCode2024.Commands
{
    internal class NotFoundCommand : ICommand
    {
        public bool WithLogging { get; set; } = false;
        public string Name { get; set; }
        public void Run()
        {
            Console.WriteLine($"Command Not Found: {Name}");
        }
    }
}
