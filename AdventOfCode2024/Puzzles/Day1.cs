namespace AdventOfCode2024.Puzzles
{
    /// <summary>
    /// --- Day 1: Historian Hysteria ---
    /// </summary>
    /// <see cref="https://adventofcode.com/2024/day/1"/>
    internal class Day1 : Puzzle
    {
        private List<int> list1 = [];
        private List<int> list2 = [];
        private readonly List<int> distances = [];
        private readonly List<int> scores = [];

        public Day1()
            : base(Name: "Historian Hysteria", DayNumber: 1) { }

        public override void ParseData()
        {
            list1.Clear();
            list2.Clear();
            distances.Clear();
            scores.Clear();

            foreach (var line in DataRaw.Replace("\r","").Split("\n"))
            {
                var nums = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                list1.Add(int.Parse(nums[0]));
                list2.Add(int.Parse(nums[1]));
            }
        }

        public override void Part1(bool isTestMode)
        {
            ParseData();

            list1 = [.. list1.OrderBy(n => n)];
            list2 = [.. list2.OrderBy(n => n)];

            for (int i = 0; i < list1.Count; i++)
                distances.Add(Math.Abs(list1[i] - list2[i]));

            Part1Result = $"Distance = {distances.Sum()}";
        }

        public override void Part2(bool isTestMode)
        {
            ParseData();

            for (int i = 0; i < list1.Count; i++)
                scores.Add(list1[i] * list2.Where(n => n == list1[i]).Count());

            Part2Result = $"Similarity = {scores.Sum()}";
        }
    }
}
