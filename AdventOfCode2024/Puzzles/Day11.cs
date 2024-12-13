using Spectre.Console;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2024.Puzzles
{
    /// <summary>
    /// --- Day 11: Plutonian Pebbles ---
    /// </summary>
    /// <see cref="https://adventofcode.com/2024/day/11"/>
    internal class Day11 : Puzzle
    {
        private List<List<long>> Arangments = [];

        public Day11()
            : base(Name: "", DayNumber: 11) { }

        public override void ParseData()
        {
            Arangments.Clear();

            var data = DataRaw.Replace("\r", "").Split('\n', StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in data)
                Arangments.Add(line.Split(' ', StringSplitOptions.TrimEntries)
                    .Select(i => long.Parse(i))
                    .ToList());
        }

        public override void Part1(bool isTestMode)
        {
            ParseData();

            var answer = "";
            int t = 0;
            foreach (var arrangment in Arangments)
            {
                long totalStones = 0;

                answer += $"[green1]Initial arrangement:[/]\n[yellow]{string.Join(' ', arrangment)}[/]\n";
                int blinkCount = 0;

                if (isTestMode)
                {
                    blinkCount = t++ == 0 ? 1 : 6;

                    var blinks = BlinkAtStones(arrangment, blinkCount).ToList();
                    for (int b = 0; b < blinks.Count && isTestMode; b++)
                    {
                        answer += $"[dim green1]After[/] [yellow bold]{blinks[b].numBlinks}[/] [dim green1]blink(s):[/]\n" +
                                  $"[teal]{string.Join(' ', blinks[b].arrangment)}[/]\n";
                    }
                    totalStones = blinks.Last().arrangment.Count;
                }
                else
                {
                    blinkCount = 25;
                    totalStones = Blink(arrangment, blinkCount);
                }

                answer += $"[green1]Total Stones[/] [yellow bold]@{blinkCount}[/] [green1]Blinks:[/] [yellow bold]{totalStones}[/]\n\n";
            }

            Part1Result = answer;
        }
        public override void Part2(bool isTestMode)
        {
            var answer = "";

            if (isTestMode)
            {
                answer += "[dim green1]No Tests for part 2[/]\n";
            }
            else
            {
                ParseData();

                foreach (var arrangment in Arangments)
                {
                    answer += $"[green1]Initial arrangement:[/]\n[yellow]{string.Join(' ', arrangment)}[/]\n";

                    int blinkCount = 75;
                    var totalStones = Blink(arrangment, blinkCount);
                    answer += $"[green1]Total Stones[/] [yellow bold]@{blinkCount}[/] [green1]Blinks:[/] [yellow bold]{totalStones}[/]\n\n";
                }
            }

            Part2Result = answer;
        }

        private static long Blink(List<long> stones, int blinkCount)
        {
            Dictionary<long, long> oldStones =
                stones.
                GroupBy(r => r).
                ToDictionary(g => g.Key, g => (long)g.Count());

            for (int b = 0; b < blinkCount; b++)
            {
                Dictionary<long, long> newStones = [];
                foreach (var kvp in oldStones)
                {
                    long number = kvp.Key;
                    long count = kvp.Value;
                    List<long> results = Blink(number);

                    foreach (long result in results)
                    {
                        if (newStones.ContainsKey(result))
                        {
                            newStones[result] += count;
                        }
                        else
                        {
                            newStones[result] = count;
                        }
                    }
                }
                oldStones = newStones;
            }

            return oldStones.Values.Sum();
        }
        private static List<(int numBlinks, List<long> arrangment)> BlinkAtStones(List<long> stones, int numBlinks)
        {
            var result = new List<(int numBlinks, List<long> arrangment)>();

            if (numBlinks > 0)
            {
                int blink = 0;
                var blinkResult = new List<long>(stones);
                do
                {
                    blinkResult = BlinkStones(blinkResult);
                    result.Add((blink + 1, new List<long>(blinkResult)));

                    blink++;
                }
                while (blink < numBlinks);
            }

            return result;
        }
        private static List<long> BlinkStones(List<long> stones)
        {
            var newStones = new List<long>();

            foreach (var stone in stones)
            {
                newStones.AddRange(Blink(stone));
            }

            return newStones;
        }
        private static List<long> Blink(long stone)
        {
            if (stone == 0)
                return [1];
            else if (CountDigits(stone) % 2 == 0)
            {
                var (l, r) = SplitNumber(stone);
                return [l, r];
            }
            else
                return [2024 * stone];
        }
        private static int CountDigits(long n)
        {
            if (n == 0) return 1;
            return (int)Math.Floor(Math.Log10(Math.Abs(n)) + 1);
        }
        private static (long, long) SplitNumber(long n)
        {
            long digits = CountDigits(n);
            if (digits % 2 != 0) throw new ArgumentException("Number must have an even number of digits.");

            long power = (long)Math.Pow(10, digits / 2);
            return (n / power, n % power);
        }
    }
}
