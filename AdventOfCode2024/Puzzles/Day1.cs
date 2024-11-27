using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2024.Puzzles
{
    //Placeholder for day1, this is just a copy of day1 from 2023 for now
    internal class Day1 : Puzzle
    {
        private List<string> calDoc = [];
        private List<List<int>> calValues = [];
        private List<int> calNums = [];

        public Day1()
            : base(Name: "Trebuchet", DayNumber: 1) { }

        public override void ParseData()
        {
            calDoc = DataRaw.ToLines();
            calValues = [];
            foreach (var line in calDoc)
            {
                calValues.Add(line.Where(c => char.IsDigit(c)).Select(c => int.Parse(c.ToString())).ToList());
            }

        }

        public override void Part1(bool TestMode)
        {
            LoadData(isTestMode: TestMode, partNum: 1);
            ParseData();
            int sum = SumCalNums();

            Part1Result = $"Sum = {sum}";
        }

        private int SumCalNums()
        {
            calNums = [];
            foreach (var nums in calValues)
            {
                var firstNum = nums.First();
                var lastNum = nums.Last();

                calNums.Add(int.Parse($"{firstNum}{lastNum}"));
            }

            var sum = calNums.Sum();
            return sum;
        }

        public override void Part2(bool TestMode)
        {
            LoadData(isTestMode: TestMode, partNum: 2);
            DataRaw = DataRaw.WordsToNums();
            ParseData();
            int sum = SumCalNums();

            Part2Result = $"Sum = {sum}";
        }
    }

    internal static class Day1Extensions
    {
        public static string WordsToNums(this string data)
        {
            string result = "";

            var dict = new Dictionary<string, string>
            {
                { "one", "1" },
                { "two", "2" },
                { "three", "3"},
                { "four", "4" },
                { "five", "5" },
                { "six", "6" },
                { "seven", "7" },
                { "eight", "8"},
                { "nine", "9" },
                { "zero", "0" }
            };

            for (int i = 0; i < data.Length; i++)
            {
                var line = data.Substring(i, data.Length - i);
                if (line[0] == '\n')
                    result += "\n";
                else if (char.IsDigit(line[0]))
                    result += line[0];
                else
                    foreach (var item in dict)
                    {
                        if (line.StartsWith(item.Key))
                        {
                            result += item.Value;
                            break;
                        }
                    }
            }

            return result;
        }
    }
}
