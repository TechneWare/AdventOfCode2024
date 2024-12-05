using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2024.Puzzles
{
    /// <summary>
    /// --- Day 5: Print Queue ---
    /// </summary>
    /// <see cref="https://adventofcode.com/2024/day/5"/>
    internal class Day5 : Puzzle
    {
        private List<(int X, int Y)> rules = [];
        private List<List<int>> updates = [];

        public Day5()
            : base(Name: "Print Queue", DayNumber: 5) { }

        public override void ParseData()
        {
            var data = DataRaw.Replace("\r", "").Split("\n");

            rules = [];
            updates = [];

            bool isParsingRules = true;

            foreach (var line in data)
            {
                if (string.IsNullOrWhiteSpace(line))
                    isParsingRules = false;
                else if (isParsingRules)
                {
                    var parts = line.Split('|');
                    rules.Add((int.Parse(parts[0]), int.Parse(parts[1])));
                }
                else
                {
                    updates.Add(line.Split(',').Select(int.Parse).ToList());
                }
            }
        }
        public override void Part1(bool isTestMode)
        {
            ParseData();

            int total = 0;
            foreach (var update in updates)
            {
                if (IsValidUpdate(update, rules))
                {
                    total += GetMidPage(update);
                }
            }

            Part1Result = $"Valid Middle Page Sum: {total}";
        }
        public override void Part2(bool isTestMode)
        {
            ParseData();

            int total = 0;
            var InvalidUpdates = GetInvalidUpdates();

            foreach (var update in InvalidUpdates)
            {
                var sorted = SortUpdate(update, rules);
                total += GetMidPage(sorted);
            }

            Part2Result = $"Sorted Middle Page Sum: {total}";
        }

        private List<List<int>> GetInvalidUpdates() => updates.Where(u => !IsValidUpdate(u, rules)).ToList();
        private static bool IsValidUpdate(List<int> update, List<(int X, int Y)> rules)
        {
            // map the positions of each page
            var positions = update
                .Select((p, i) => (page: p, index: i))
                .ToDictionary(p => p.page, p => p.index);

            // check the rules
            return rules.Where(r => positions.ContainsKey(r.X) && positions.ContainsKey(r.Y))
                        .All(r => positions[r.X] < positions[r.Y]);
        }
        private static List<int> SortUpdate(List<int> update, List<(int X, int Y)> rules)
        {
            var (pages, weights) = GetSortingData(update, rules);

            var result = new List<int>();
            // select first sorted value
            var minValues = new Queue<int>(weights.Where(w => w.Value == 0).Select(w => w.Key));

            // Kahn's topological sort
            while (minValues.Count > 0)
            {
                var curItem = minValues.Dequeue();
                result.Add(curItem);

                foreach (var relatedPage in pages[curItem])
                {
                    weights[relatedPage]--;
                    if (weights[relatedPage] == 0)      //as related pages float to the top
                        minValues.Enqueue(relatedPage); //que them for the result
                }
            }

            if (result.Count != update.Count)
                throw new InvalidOperationException("Invalid Rule Detected");

            return result;
        }
        private static (Dictionary<int, HashSet<int>>, Dictionary<int, int>) GetSortingData(List<int> update, List<(int X, int Y)> rules)
        {
            // make page graph and position weights
            var pages = new Dictionary<int, HashSet<int>>();
            var posWeights = new Dictionary<int, int>();
            foreach (var page in update)
            {
                pages[page] = [];
                posWeights[page] = 0;
            }

            // for each rule that applies
            foreach (var (X, Y) in rules
                .Where(r => update.Contains(r.X) && update.Contains(r.Y)))
            {
                pages[X].Add(Y);    //map the related page
                posWeights[Y]++;    //increase its positional weight
            }

            return (pages, posWeights);
        }
        private static int GetMidPage(List<int> update)
        {
            return update[update.Count / 2];
        }
    }
}
