using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static AdventOfCode2024.Puzzles.Day7.Calibration;

namespace AdventOfCode2024.Puzzles
{
    /// <summary>
    /// Bridge Repair
    /// </summary>
    /// <see cref="https://adventofcode.com/2024/day/7"/>
    internal class Day7 : Puzzle
    {
        private List<Calibration> Calibrations { get; set; } = [];
        public Day7()
            : base(Name: "Bridge Repair", DayNumber: 7) { }

        public override void ParseData()
        {
            Calibrations.Clear();

            var data = DataRaw.Replace("\r", "").Split("\n", StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in data)
            {
                var d = line.Split(':', StringSplitOptions.TrimEntries);
                var value = long.Parse(d[0]);
                var nums = d[1].Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                .Select(i => int.Parse(i))
                                .ToList();

                Calibrations.Add(new Calibration(value, nums));
            }
        }
        public override void Part1(bool isTestMode)
        {
            ParseData();
            foreach (var c in Calibrations)
            {
                c.GenerateSolutions([
                    Calibration.Operator.Add,
                    Calibration.Operator.Multiply]);
            }

            Part1Result = GenerateAnswer(isTestMode);
        }
        public override void Part2(bool isTestMode)
        {
            ParseData();

            foreach (var c in Calibrations)
            {
                c.GenerateSolutionsThreaded([
                    Calibration.Operator.Add,
                    Calibration.Operator.Multiply,
                    Calibration.Operator.Concat]);
            }

            Part2Result = GenerateAnswer(isTestMode);
        }
        private string GenerateAnswer(bool isTestMode)
        {
            var vc = Calibrations.Where(c => c.IsValid).ToList();
            var answer = new StringBuilder();
            if (isTestMode || WithLogging)
            {
                foreach (var c in vc)
                {
                    answer.Append($"{c.TestValue} Solutions:\n");
                    foreach (var s in c.Solutions)
                        answer.Append($" => {s}\n");
                    answer.Append('\n');
                }
            }

            var sum = vc.Sum(c => c.TestValue);
            answer.Append($"Total Calibration = {sum}\n");
            return answer.ToString();
        }
        internal class Calibration
        {
            public enum Operator
            {
                None,
                Add,
                Multiply,
                Concat
            }
            public long TestValue { get; private set; }
            public List<int> Numbers { get; private set; }
            public List<string> Solutions { get; private set; } = [];
            public bool IsValid => Solutions.Count > 0;
            public Calibration(long testValue, List<int> numbers)
            {
                TestValue = testValue;
                Numbers = numbers;
            }
            internal void GenerateSolutions(List<Operator> operators)
            {
                var possibleEquations = GenerateEquations(Numbers, operators);
                var validSolutions = new List<List<(int, Operator)>>();
                foreach (var pe in possibleEquations)
                {
                    var value = RunEquation(pe);
                    if (value == TestValue)
                        validSolutions.Add(pe);
                }

                SaveSolutions(validSolutions);
            }
            internal void GenerateSolutionsThreaded(List<Operator> operators)
            {
                var possibleEquations = GenerateEquations(Numbers, operators);
                var validSolutions = new ConcurrentStack<List<(int, Operator)>>();
                Parallel.ForEach(possibleEquations, (pe) =>
                {
                    var value = RunEquation(pe);
                    if (value == TestValue)
                        validSolutions.Push(pe);
                });

                SaveSolutions([.. validSolutions]);
            }
            private void SaveSolutions(List<List<(int, Operator)>> validSolutions)
            {
                foreach (var vs in validSolutions)
                {
                    string solution = "";
                    Operator nextOp = Operator.None;
                    for (int i = 0; i < vs.Count; i++)
                    {
                        var (num, op) = vs[i];

                        if (i == 0)
                        {
                            solution = $"{num}";
                        }
                        else
                        {
                            switch (nextOp)
                            {
                                case Operator.Add:
                                    solution += $" + {num}";
                                    break;
                                case Operator.Multiply:
                                    solution += $" * {num}";
                                    break;
                                case Operator.Concat:
                                    solution += $" || {num}";
                                    break;
                                case Operator.None:
                                default:
                                    break;
                            }
                        }
                        nextOp = op;
                    }

                    Solutions.Add(solution);
                }
            }
            private static long RunEquation(List<(int num, Operator op)> operations)
            {
                long result = 0;
                Operator nextOp = Operator.None;

                for (int i = 0; i < operations.Count; i++)
                {
                    var (num, op) = operations[i];

                    if (i == 0)
                    {
                        result = num;
                    }
                    else
                    {
                        switch (nextOp)
                        {
                            case Operator.Add:
                                result += num;
                                break;
                            case Operator.Multiply:
                                result *= num;
                                break;
                            case Operator.Concat:
                                //Equivelent to => result = long.Parse($"{result}{num}");
                                //but faster
                                int numDigits = (int)Math.Log10(num) + 1;
                                result = result * (long)Math.Pow(10, numDigits) + num;
                                break;
                            case Operator.None:
                            default:
                                break;
                        }
                    }
                    nextOp = op;
                }

                return result;
            }
            internal static List<List<(int, Operator)>> GenerateEquations(List<int> numbers, List<Operator> operators)
            {
                var results = new List<List<(int, Operator)>>();
                GetPossibleEquations(numbers, operators, [], results);
                return results;
            }
            internal static void GetPossibleEquations(
                                    List<int> numbers,
                                    List<Operator> operators,
                                    List<(int, Operator)> current,
                                    List<List<(int, Operator)>> results)
            {
                int index = current.Count;

                if (index == numbers.Count)
                {
                    results.Add(new List<(int, Operator)>(current));
                    return;
                }

                if (index == numbers.Count - 1)
                {
                    // The last number always has a `None` operator.
                    current.Add((numbers[index], Operator.None));
                    GetPossibleEquations(numbers, operators, current, results);
                    current.RemoveAt(current.Count - 1);
                }
                else
                {
                    foreach (var op in operators)
                    {
                        current.Add((numbers[index], op));
                        GetPossibleEquations(numbers, operators, current, results);
                        current.RemoveAt(current.Count - 1);
                    }
                }
            }
        }
    }
}
