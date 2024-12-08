using Spectre.Console;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2024.Puzzles
{
    /// <summary>
    /// --- Day 6: Guard Gallivant ---
    /// </summary>
    /// <see cref="https://adventofcode.com/2024/day/6"/>
    internal class Day6 : Puzzle
    {
        private int width = 0;
        private int height = 0;
        private char[,] grid = new char[1, 1];
        private int startRow = 0;
        private int startCol = 0;

        public Day6()
            : base(Name: "Guard Gallivant", DayNumber: 6) { }

        public override void ParseData()
        {
            var data = DataRaw.Replace("\r", "").Split("\n", StringSplitOptions.RemoveEmptyEntries);

            width = data[0].Length;
            height = data.Length;
            grid = new char[width, height];

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    grid[row, col] = data[row][col];
                    if (grid[row, col] == '^')
                    {
                        startRow = row;
                        startCol = col;
                    }
                }
            }
        }

        public override void Part1(bool isTestMode)
        {
            var answer = "";
            ParseData();

            var g = TracePathForward(startRow, startCol, CopyGrid(grid));

            if (isTestMode)
                answer += GetGridDisplayWithExit(g);

            int visited = GetVisitedPositionsCount(g);
            answer += $"[dim]Total Visted[/] = [bold yellow]{visited}[/]\n";

            Part1Result = answer;
        }

        public override void Part2(bool isTestMode)
        {
            if (!isTestMode && (SysInfo.ProcessorCount < 4 || SysInfo.TotalMemoryMB < 16384))
            {
                Part2Result =
                    $"Part2 Actual Skippped\n" +
                    $"Your system does not have at least 4 CPUs and 16Gig of ram\n" +
                    $"You have {SysInfo.ProcessorCount} CPUs and {SysInfo.TotalMemoryMB} MB of Ram";
            }
            else
            {

                var answer = "";
                ParseData();

                var loops = GetLoops(startRow, startCol, CopyGrid(grid));

                if (isTestMode)
                {
                    foreach (var loop in loops)
                        answer += GetGridDisplayWithLoop(loop.objRow, loop.objCol, loop.grid);
                }

                var numLoops = loops.Count;
                answer += $"[dim]Objstruction Positions[/] = [bold yellow]{numLoops}[/]\n";

                Part2Result = answer;
            }
        }
        private static int GetVisitedPositionsCount(char[,] g)
        {
            return GetVisitedPositions(g).Count;
        }
        private static List<(int row, int col)> GetVisitedPositions(char[,] g)
        {
            List<(int row, int col)> result = [];
            List<char> validChars = ['<', '>', 'v', '^', 'X'];
            int h = g.GetLength(0);
            int w = g.GetLength(1);

            for (int row = 0; row < h; row++)
            {
                for (int col = 0; col < w; col++)
                {
                    if (validChars.Contains(g[row, col]))
                        result.Add((row, col));
                }
            }

            return result;
        }
        private static char[,] TracePathForward(int startRow, int startCol, char[,] g)
        {
            var curRow = startRow;
            var curCol = startCol;
            var curDir = g[curRow, curCol];

            var (nextRow, nextCol, nextSymb) = GetNext(curRow, curCol, g);
            while (IsInGrid(nextRow, nextCol, g))
            {
                if (nextSymb == '#')
                {
                    g[curRow, curCol] = Turn90Deg(curDir);
                    curDir = g[curRow, curCol];
                }
                else
                {
                    g[curRow, curCol] = 'X';
                    g[nextRow, nextCol] = curDir;
                    curRow = nextRow;
                    curCol = nextCol;
                }

                (nextRow, nextCol, nextSymb) = GetNext(curRow, curCol, g);
            }

            return g;
        }
        private static (List<(int row, int col)> elims, char[,] grid) TracePath(int startRow, int startCol, char[,] g)
        {
            var curRow = startRow;
            var curCol = startCol;
            var curDir = g[curRow, curCol];
            var rejected = new List<(int row, int col)>();

            var (nextRow, nextCol, nextSymb) = GetNext(curRow, curCol, g);
            while (IsInGrid(nextRow, nextCol, g))
            {
                if (nextSymb == '#')
                {
                    g[curRow, curCol] = Turn90Deg(curDir);
                    curDir = g[curRow, curCol];
                }
                else
                {
                    if (CanEliminate(nextRow, nextCol, g))
                        rejected.Add((nextRow, nextCol));

                    g[curRow, curCol] = 'X';
                    g[nextRow, nextCol] = curDir;
                    curRow = nextRow;
                    curCol = nextCol;
                }

                (nextRow, nextCol, nextSymb) = GetNext(curRow, curCol, g);
            }

            return (rejected, g);
        }
        private static bool CanEliminate(int row, int col, char[,] g)
        {
            int h = g.GetLength(0);
            int w = g.GetLength(1);
            List<(int row, int col, char dir)> checks = [
                    new(row+1,col,'^'),
                    new(row, col-1,'>'),
                    new(row-1, col, 'v'),
                    new(row, col+1,'<'),
                ];

            List<bool> results = [];
            foreach (var check in checks.Where(c => c.row >= 0 && c.row < h && c.col >= 0 && c.col < w))
            {
                results.Add(CanBeEliminated(check.row, check.col, check.dir, g));
            }

            return results.All(x => x == true);

        }
        private static bool CanBeEliminated(int row, int col, char dir, char[,] g)
        {
            int h = g.GetLength(0);
            int w = g.GetLength(1);

            switch (dir)
            {
                case '^':
                    for (int c = col; c < w; c++)
                        if (g[row, c] == '#') return false;
                    break;
                case '>':
                    for (int r = row; r < h; r++)
                        if (g[r, col] == '#') return false;
                    break;
                case 'v':
                    for (int c = col; c > 0; c--)
                        if (g[row, c] == '#') return false;
                    break;
                case '<':
                    for (int r = row; r > 0; r--)
                        if (g[r, col] == '#') return false;
                    break;
                default:
                    throw new Exception("Invalid Direction Detected");
            }

            return true;
        }
        private static List<(int objRow, int objCol, char[,] grid)> GetLoops(int startRow, int startCol, char[,] g)
        {
            var gOrig = CopyGrid(g);
            var rStack = new ConcurrentStack<(int objRow, int objCol, char[,] grid)>();
            var result = new List<(int objRow, int objCol, char[,] grid)>();
            List<(int row, int col)> rejects;
            (rejects, g) = TracePath(startRow, startCol, g); //get initial positions
            var initPositions = GetVisitedPositions(g);
            var checkPositions = initPositions.Except(rejects).ToList();

            //place an object along the visited positions and detect the loops
            var parallelOpts = new ParallelOptions() { MaxDegreeOfParallelism = checkPositions.Count };
            Parallel.For(0, checkPositions.Count, parallelOpts, (i) =>
            {
                var (row, col) = checkPositions[i];
                var trace = TracePathLoop(startRow, startCol, row, col, CopyGrid(gOrig));
                if (trace.isLoop)
                {
                    rStack.Push((trace.objRow, trace.objCol, trace.grid));
                }
            });

            while (rStack.TryPop(out (int objRow, int objCol, char[,] grid) r))
            {
                result.Add(r);
            }

            return result;
        }
        private static (bool isLoop, int objRow, int objCol, char[,] grid) TracePathLoop(int startRow, int startCol, int objRow, int objCol, char[,] g)
        {
            var isLoop = false;
            var curRow = startRow;
            var curCol = startCol;
            var curDir = g[curRow, curCol];
            List<(int row, int col)> p1 = [];
            List<(int row, int col)> p2 = [];

            //if obstruction is at the starting location then its not a loop
            if (curRow == objRow && curCol == objCol)
                return (false, objRow, objCol, g);

            //place obstruction
            g[objRow, objCol] = '#';

            var (nextRow, nextCol, nextSymb) = GetNext(curRow, curCol, g);
            while (IsInGrid(nextRow, nextCol, g) && !isLoop)
            {
                if (nextSymb == '#')
                {
                    g[curRow, curCol] = Turn90Deg(curDir);
                    curDir = g[curRow, curCol];
                }
                else
                {
                    g[curRow, curCol] = 'X';
                    g[nextRow, nextCol] = curDir;
                    curRow = nextRow;
                    curCol = nextCol;

                    isLoop = (p2.Count == 0 && p1.Contains((curRow, curCol)))
                           || (p2.Count > 0 && p1.Count > p2.Count);

                    if (isLoop)
                    {
                        isLoop = p1.Count > p2.Count && new HashSet<(int row, int col)>(p2).IsSupersetOf(p1);
                        if (!isLoop)
                        {
                            p2 = new List<(int row, int col)>(p1);
                            p1.Clear();
                        }
                        else
                            break;
                    }

                    p1.Add((curRow, curCol));
                }

                (nextRow, nextCol, nextSymb) = GetNext(curRow, curCol, g);
            }

            return (isLoop, objRow, objCol, CopyGrid(g));
        }
        private static char Turn90Deg(char curSymb)
        {
            return curSymb switch
            {
                '^' => '>',
                '>' => 'v',
                'v' => '<',
                '<' => '^',
                _ => curSymb,
            };
        }
        private static (int row, int col, char symb) GetNext(int curRow, int curCol, char[,] g)
        {
            var c = g[curRow, curCol];

            int nextRow, nextCol;
            switch (c)
            {
                case '^':
                    nextRow = curRow - 1;
                    nextCol = curCol;
                    break;
                case '>':
                    nextRow = curRow;
                    nextCol = curCol + 1;
                    break;
                case 'v':
                    nextRow = curRow + 1;
                    nextCol = curCol;
                    break;
                case '<':
                    nextRow = curRow;
                    nextCol = curCol - 1;
                    break;
                default:
                    throw new Exception("Invalid Direction detected at current location");
            }

            if (IsInGrid(nextRow, nextCol, g))
                return (nextRow, nextCol, g[nextRow, nextCol]);
            else
                return (nextRow, nextCol, '.');
        }
        private static bool IsInGrid(int row, int col, char[,] g)
        {
            int h = g.GetLength(0);
            int w = g.GetLength(1);
            return (row >= 0 && row < h && col >= 0 && col < w);
        }
        private static char[,] CopyGrid(char[,] gSource)
        {
            char[,] g = new char[gSource.GetLength(0), gSource.GetLength(1)];
            Array.Copy(gSource, g, gSource.Length);

            return g;
        }
        private static string GetGridDisplayWithExit(char[,] g)
        {
            var result = "";

            int h = g.GetLength(0);
            int w = g.GetLength(1);

            for (int row = 0; row < h; row++)
            {
                for (int col = 0; col < w; col++)
                {
                    var c = g[row, col];
                    switch (c)
                    {
                        case '.':
                            result += "[dim].[/]";
                            break;
                        case '#':
                            result += "[deepskyblue1]#[/]";
                            break;
                        case 'X':
                            result += "[yellow bold]X[/]";
                            break;
                        case '^':
                        case 'v':
                        case '>':
                        case '<':
                            result += $"[green1 bold slowblink]{g[row, col]}[/]";
                            break;
                    }
                }
                result += "\n";
            }

            result += "\n";

            return result;
        }
        private static string GetGridDisplayWithLoop(int objRow, int objCol, char[,] g)
        {
            var result = "";

            int h = g.GetLength(0);
            int w = g.GetLength(1);

            g[objRow, objCol] = 'O';

            for (int row = 0; row < h; row++)
            {
                for (int col = 0; col < w; col++)
                {
                    var c = g[row, col];
                    switch (c)
                    {
                        case 'O':
                            result += "[invert]O[/]";
                            break;
                        case '.':
                            result += "[dim].[/]";
                            break;
                        case '#':
                            result += "[deepskyblue1]#[/]";
                            break;
                        case 'X':
                            result += "[yellow bold]X[/]";
                            break;
                        case '^':
                        case 'v':
                        case '>':
                        case '<':
                            result += $"[green1 bold slowblink]{g[row, col]}[/]";
                            break;
                    }
                }
                result += "\n";
            }

            result += "\n";

            return result;
        }
    }
}
