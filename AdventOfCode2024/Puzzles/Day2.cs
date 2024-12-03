namespace AdventOfCode2024.Puzzles
{
    /// <summary>
    /// --- Day 2: Red-Nosed Reports ---
    /// </summary>
    /// <see cref="https://adventofcode.com/2024/day/2"/>
    internal class Day2 : Puzzle
    {
        private readonly List<Report> reports = [];

        public Day2()
            : base(Name: "Red-Nosed Reports", DayNumber: 2) { }

        public override void ParseData()
        {
            reports.Clear();
            foreach (var line in DataRaw.Split("\r\n", StringSplitOptions.RemoveEmptyEntries))
                reports.Add(new Report(line));
        }

        public override void Part1(bool isTestMode)
        {
            ParseData();

            int safeReports = 0;
            var answer = "";

            foreach (var report in reports)
            {
                var isSafe = report.IsSafe();

                if (isSafe)
                    safeReports++;

                if (isTestMode)
                    answer += $"{String.Join(' ', report.Levels)} = {(isSafe ? "Safe" : "Unsafe")}\n";
            }

            answer += $"Total Safe = {safeReports}\n";

            Part1Result = answer;
        }

        public override void Part2(bool isTestMode)
        {
            ParseData();

            int safeReports = 0;
            var answer = "";

            foreach (var report in reports)
            {
                var isSafe = report
                            .UseProblemDampener()
                            .IsSafe();

                if (isSafe)
                    safeReports++;

                if (isTestMode)
                    answer += $"{String.Join(' ', report.Levels)} = {(isSafe ? "Safe" : "Unsafe")}\n";
            }

            answer += $"Total Safe = {safeReports}\n";

            Part2Result = answer;
        }

        internal class Report(string data)
        {
            private bool _useProblemDampener;
            private List<Report> Permutations { get; set; } = [];

            public List<int> Levels { get; internal set; } = data
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => int.Parse(x))
                    .ToList();

            public bool IsSafe()
            {
                var hasAllIncreasingValues = false;
                var hasAllDecreasingValues = false;
                var hasAllInRangeDiffs = false;
                var diffs = new List<int>();

                for (int i = 0; i < Levels.Count - 1; i++)
                    diffs.Add(Levels[i + 1] - Levels[i]);

                hasAllIncreasingValues = diffs.All(d => d > 0);
                hasAllDecreasingValues = diffs.All(d => d < 0);
                hasAllInRangeDiffs = diffs.All(d => Math.Abs(d) >= 1 && Math.Abs(d) <= 3);

                bool isSafe = (hasAllIncreasingValues || hasAllDecreasingValues) && hasAllInRangeDiffs;

                if (!isSafe && _useProblemDampener)
                    isSafe = Permutations.Any(p => p.IsSafe());

                return isSafe;
            }
            public Report UseProblemDampener()
            {
                Permutations.Clear();
                for (int i = 0; i < Levels.Count; i++)
                {
                    var levels = Levels.Clone();
                    levels.RemoveAt(i);
                    Permutations.Add(new Report(String.Join(' ', levels)));
                }

                _useProblemDampener = true;

                return this;
            }
        }
    }
}
