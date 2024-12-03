namespace AdventOfCode2024.Puzzles
{
    internal interface IPuzzle
    {
        string Name { get; }
        double DayNumber { get; }
        public void Run();
        public void ShowPuzzleText(int partNum);
        public void Part1(bool isTestMode);
        public void Part2(bool isTestMode);

    }
}
