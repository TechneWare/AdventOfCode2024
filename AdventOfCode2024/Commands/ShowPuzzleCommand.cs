using Spectre.Console;

namespace AdventOfCode2024.Commands
{
    public class ShowPuzzleCommand : ICommand, ICommandFactory
    {
        public string CommandName => "ShowPuzzle";

        public string CommandArgs => "[Day Number] [Part Number [1-2]]";

        public string[] CommandAlternates => ["show"];

        public string Description => "Show a Puzzle's Description";
        public string ExtendedDescription => "'show 1' will display the day 1 part 1 description\n" +
                                             "'show 1 2' will display the day 1 part 2 description";

        public double DayNumber { get; set; } = 0;
        private int PartNumber { get; set; } = 1;
        public string arg { get; set; } = "";


        public ICommand MakeCommand(string[] args)
        {
            ICommand cmd = new ShowPuzzleCommand();

            try
            {
                if (args.Length > 1)
                {
                    ((ShowPuzzleCommand)(cmd)).arg = args[1];
                    if (double.TryParse(args[1], out double dayNum))
                        ((ShowPuzzleCommand)(cmd)).DayNumber = dayNum;
                    else if (args[1].ToLower().StartsWith('l')) //run latest puzzle
                        if (Utils.GetAllPuzzles().Any())
                            ((ShowPuzzleCommand)cmd).DayNumber = (double)Utils.GetAllPuzzles().Max(p => p.DayNumber);
                        else
                            cmd = new BadCommand("No Puzzles have been created yet, go make one!", this);
                }
                else
                    cmd = new BadCommand("Usage: day [Day Number | last]", this);

                if (cmd is ShowPuzzleCommand)
                {
                    ((ShowPuzzleCommand)cmd).PartNumber = 1;

                    if (args.Length > 2)
                    {
                        if (int.TryParse(args[2], out int partNum))
                        {
                            if (partNum == 1 || partNum == 2)
                                ((ShowPuzzleCommand)cmd).PartNumber = partNum;
                        }
                    }
                }
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
                    puzzle.ShowPuzzleText(PartNumber);
                else
                    AnsiConsole.MarkupLineInterpolated($"[bold yellow]Unkown Day Number[/] [bold yellow slowblink]{DayNumber}[/] [bold yellow]no puzzle for this day was found[/]");
            }
            else
                AnsiConsole.MarkupLineInterpolated($"[bold yellow]Invalid Day Number[/] [bold yellow slowblink]{arg}[/] [bold yellow]must be 1 or higher[/]");
        }
    }
}
