namespace AdventOfCode2024.Commands
{
    internal interface ICommandFactory
    {
        string CommandName { get; }
        string CommandArgs { get; }
        string[] CommandAlternates { get; }
        string Description { get; }
        ICommand MakeCommand(string[] args);
    }
}
