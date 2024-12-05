using Spectre.Console;

namespace AdventOfCode2024.Commands
{
    public class RunPuzzleCommand : ICommand, ICommandFactory
    {
        public string CommandName => "RunPuzzle";

        public string CommandArgs => "[Day Number]";

        public string[] CommandAlternates => ["day", "[#]"];

        public string Description => "Runs a Puzzle";
        public string ExtendedDescription => "Runs a Puzzle, EG: 'RunPuzzle 1' will run the first puzzle\n" +
                                             "'RunPuzzle [daynumber] log' will run a specific puzzle with logging turned on\n" +
                                             "Simply typing a number will run that days puzzle if found. EG: '5' will run the Day 5 Puzzle.";

        public double DayNumber { get; set; } = 0;
        public string arg { get; set; } = "";

        public ICommand MakeCommand(string[] args)
        {
            ICommand cmd = new RunPuzzleCommand();

            try
            {
                if (args.Length > 1)
                {
                    ((RunPuzzleCommand)(cmd)).arg = args[1];
                    if (double.TryParse(args[1], out double dayNum))
                        ((RunPuzzleCommand)(cmd)).DayNumber = dayNum;
                    else if (args[1].ToLower().StartsWith('l')) //run latest puzzle
                        if (Utils.GetAllPuzzles().Any())
                            ((RunPuzzleCommand)cmd).DayNumber = (double)Utils.GetAllPuzzles().Max(p => p.DayNumber);
                        else
                            cmd = new BadCommand("No Puzzles have been created yet, go make one!", this);
                }
                else
                    cmd = new BadCommand("Usage: day [Day Number | last]", this);
            }
            catch (Exception)
            {
                cmd = new BadCommand("Unable to locate any puzzles", this);
            }


            return cmd;
        }

        public void Run()
        {
            if (DayNumber != 0)
            {
                var puzzle = Utils.GetAllPuzzles()
                                  .Where(p => p.DayNumber == DayNumber)
                                  .SingleOrDefault();

                if (puzzle != null)
                    puzzle.Run();
                else
                    AnsiConsole.MarkupLineInterpolated($"[bold yellow]Unkown Day Number[/] [bold yellow slowblink]{DayNumber}[/] [bold yellow]no puzzle for this day was found[/]");
            }
            else
                AnsiConsole.MarkupLineInterpolated($"[bold yellow]Invalid Day Number[/] [bold yellow slowblink]{arg}[/] [bold yellow]must be 1 or higher[/]");
        }
    }
}
