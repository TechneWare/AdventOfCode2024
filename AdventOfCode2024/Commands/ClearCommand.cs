namespace AdventOfCode2024.Commands
{
    public class ClearCommand : ICommand, ICommandFactory
    {
        public string CommandName => "Clear";

        public string CommandArgs => "";

        public string[] CommandAlternates => new string[] { "cls" };

        public string Description => "Clears the screen";

        public ICommand MakeCommand(string[] args)
        {
            return new ClearCommand();
        }

        public void Run()
        {
            Console.Clear();
        }
    }
}
