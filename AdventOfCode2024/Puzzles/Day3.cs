using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2024.Puzzles
{
    /// <summary>
    /// --- Day 3: Mull It Over ---
    /// </summary>
    /// <see cref="https://adventofcode.com/2024/day/3"/>
    internal partial class Day3 : Puzzle
    {
        [GeneratedRegex(@"mul\(\d{1,3},\d{1,3}\)")]
        private partial Regex RegExMuls();

        private List<(int x, int y)> multipliers = [];
        private bool useDoAndDont = false;

        public Day3()
            : base(Name: "Mull It Over", DayNumber: 3) { }

        public override void ParseData()
        {
            multipliers.Clear();

            var data = DataRaw.Replace("\r\n", "");
            string validData = GetValidData(data);

            var muls = RegExMuls().Matches(validData);
            foreach (var mul in muls.Where(m => m != null))
            {
                var nums = mul.ToString()
                    .Replace("mul(", "")
                    .Replace(")", "")
                    .Split(',', StringSplitOptions.RemoveEmptyEntries);

                multipliers.Add((int.Parse(nums[0]), int.Parse(nums[1])));
            }
        }

        public override void Part1(bool isTestMode)
        {
            ParseData();

            Part1Result = GetAnswer(isTestMode);
        }

        public override void Part2(bool isTestMode)
        {
            useDoAndDont = true;
            ParseData();

            Part2Result = GetAnswer(isTestMode); ;
        }

        private string GetAnswer(bool isTestMode)
        {
            var total = multipliers.Select(m => m.x * m.y).Sum();
            var answer = $"";

            if (isTestMode)
            {
                foreach (var (x, y) in multipliers)
                {
                    answer += $"{x}*{y} + ";
                }

                answer = answer.Substring(0, answer.Length - 2);
                answer += $"= {total}\n";
            }
            else
                answer += $"Total = {total}\n";

            return answer;
        }
        private string GetValidData(string data)
        {
            var validData = useDoAndDont ? "" : data;

            if (useDoAndDont)
            {
                var enabled = true;
                int idx;
                do
                {
                    idx = enabled
                        ? data.IndexOf(@"don't()") //look for don't toggle
                        : data.IndexOf(@"do()");   //look for do toggle

                    int padding = enabled
                        ? 7     //pad to end of don't()
                        : 4;    //pad to end of do()

                    if (idx == -1)
                    {
                        //no toggle found so must be end of data
                        //setup to extract final section
                        idx = 0;
                        padding = data.Length;
                    }

                    var d = data[..(idx + padding)]; //Extract the section
                    data = data.Replace(d, "");      //Remove the section from the data

                    if (enabled)
                        validData += d;             //If currently processing muls, then keep the section

                    enabled = idx >= 0 && (enabled ^ true); //Toggle the enabled flag

                } while (idx > 0); //Until no more toggles are found
            }

            return validData;
        }
    }
}
