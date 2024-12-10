using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace AdventOfCode2024.Puzzles
{
    /// <summary>
    /// --- Day 10: Hoof It ---
    /// </summary>
    /// <see cref="https://adventofcode.com/2024/day/10"/>
    internal class Day10 : Puzzle
    {
        private List<int[,]> Maps = [];
        public Day10()
            : base(Name: "Hoof It", DayNumber: 10) { }

        public override void ParseData()
        {
            Maps.Clear();
            var data = DataRaw.Replace("\r", "").Split("\n", StringSplitOptions.TrimEntries);
            List<List<string>> mapLInes = [];
            var nextMapLines = new List<string>();
            foreach (var line in data)
            {
                if (line == "")
                {
                    mapLInes.Add(new List<string>(nextMapLines));
                    nextMapLines = [];
                    continue;
                }

                nextMapLines.Add(line);
            }
            mapLInes.Add(nextMapLines);

            foreach (var mapSet in mapLInes)
            {
                int h = mapSet.Count;
                int w = mapSet[0].Length;
                int[,] map = new int[h, w];
                int curRow = 0;

                foreach (var mapLine in mapSet)
                {
                    for (int col = 0; col < w; col++)
                    {
                        map[curRow, col] = mapLine[col] == '.'
                            ? -1
                            : mapLine[col] - '0';
                    }
                    curRow++;
                }

                Maps.Add(map);
            }
        }
        public override void Part1(bool isTestMode)
        {
            ParseData();
            var answer = "";
            int trHeadMarker = 0;
            int trEndMarker = 9;
            int trSloap = 1;

            foreach(var map in Maps)
            {
                var (score, pathPoints) = GetMapScore(map, trHeadMarker, trEndMarker, trSloap);
                answer += $"{GetMapDisplay(map, pathPoints, 0, 9)}";
                answer += $"[green1]Map Score =[/] [yellow]{score}[/]\n\n";
            }

            Part1Result = answer;
        }
        public override void Part2(bool isTestMode)
        {
            ParseData();
            var answer = "";
            int trHeadMarker = 0;
            int trEndMarker = 9;
            int trSloap = 1;

            foreach (var map in Maps)
            {
                var (score, pathPoints) = GetMapTrailScore(map, trHeadMarker, trEndMarker, trSloap);
                answer += $"{GetMapDisplay(map, pathPoints, 0, 9)}";
                answer += $"[green1]Map Score =[/] [yellow]{score}[/]\n\n";
            }

            Part2Result = answer;
        }

        private static (int mapScore, List<(int row, int col)> pathPoints) GetMapTrailScore(int[,] map, int trHeadMarker, int trEndMarker, int trSloap)
        {
            var allPaths = GetPaths(map, trHead: trHeadMarker, trEnd: trEndMarker, sloap: trSloap);
            var trailHeads = GetTrailHeads(map, trHead: trHeadMarker);
            var pathPoints = trailHeads.SelectMany(th => GetPathPoints(allPaths, th, trHeadMarker, trEndMarker).ToList()).ToList();

            return (allPaths.Count, pathPoints);
        }
        private static (int mapScore, List<(int row, int col)> pathPoints) GetMapScore(int[,] map, int trHeadMarker, int trEndMarker, int trSloap)
        {
            var allPaths = GetPaths(map, trHead: trHeadMarker, trEnd: trEndMarker, sloap: trSloap);
            var trailHeads = GetTrailHeads(map, trHead: trHeadMarker);
            var score = trailHeads.Sum(th => GetPathScore(allPaths, th, trHeadMarker, trEndMarker));
            var pathPoints = trailHeads.SelectMany(th => GetPathPoints(allPaths, th, trHeadMarker, trEndMarker).ToList()).ToList();

            return (score, pathPoints);
        }
        private static int GetPathScore(List<List<(int row, int col)>> AllPaths, (int row, int col) tHead, int theadIdx, int tendIdx)
        {
            return AllPaths.Where(p => p[theadIdx].row == tHead.row && p[theadIdx].col == tHead.col)
                            .Select(p => (p[tendIdx].row, p[tendIdx].col))
                            .Distinct().Count();
        }
        private static List<(int row, int col)> GetPathPoints(List<List<(int row, int col)>> AllPaths, (int row, int col) tHead, int theadIdx, int tendIdx)
        {
            return AllPaths.Where(p => p[theadIdx].row == tHead.row && p[theadIdx].col == tHead.col)
                            .SelectMany(p => p)
                            .Distinct().ToList();
        }
        private static List<(int row, int col)> GetTrailHeads(int[,] map, int trHead)
        {
            int h = map.GetLength(0);
            int w = map.GetLength(1);
            return (from row in Enumerable.Range(0, h)
                    from col in Enumerable.Range(0, w)
                    where map[row, col] == trHead
                    select (row, col)).ToList();
        }
        private static List<List<(int row, int col)>> GetPaths(int[,] map, int trHead = 0, int trEnd = 9, int sloap = 1)
        {
            var trailHeads = GetTrailHeads(map, trHead);

            var allPaths = new List<List<(int row, int col)>>();
            var visited = new HashSet<(int row, int col)>();
            foreach (var (thRow, thCol) in trailHeads)
            {
                var curPath = new List<(int row, int col)>();
                FindAllPaths(map, thRow, thCol,
                    trHead, trEnd, sloap,
                    curPath, visited, allPaths);
            }

            return allPaths;
        }
        private static void FindAllPaths(int[,] map, int startRow, int startCol,
                                         int currentValue, int targetValue, int sloap,
                                         List<(int row, int col)> currentPath,
                                         HashSet<(int row, int col)> visited,
                                         List<List<(int row, int col)>> allPaths)
        {
            if (startRow < 0 || startRow >= map.GetLength(0) ||
                startCol < 0 || startCol >= map.GetLength(1))
                return;

            if (visited.Contains((startRow, startCol)))
                return;

            if (map[startRow, startCol] != currentValue)
                return;

            currentPath.Add((startRow, startCol));
            visited.Add((startRow, startCol));

            if (currentValue == targetValue)
            {
                allPaths.Add(new List<(int row, int col)>(currentPath));
            }
            else
            {
                int[] dRow = { -1, 1, 0, 0 };
                int[] dCol = { 0, 0, -1, 1 };

                for (int i = 0; i < 4; i++)
                {
                    FindAllPaths(map, startRow + dRow[i], startCol + dCol[i],
                                currentValue + sloap, targetValue, sloap,
                                currentPath, visited, allPaths);
                }
            }

            currentPath.RemoveAt(currentPath.Count - 1);
            visited.Remove((startRow, startCol));
        }

        private static string GetMapDisplay(int[,] map, List<(int row, int col)> PathPoints, int trHead, int trEnd)
        {
            var result = new StringBuilder();

            int h = map.GetLength(0);
            int w = map.GetLength(1);

            for (int row = 0; row < h; row++)
            {
                for (int col = 0; col < w; col++)
                {
                    bool isPathPoint = PathPoints.Any(p => p.row == row && p.col == col);

                    result.Append(GetTopoStyle(map[row, col], isPathPoint));

                    if (!isPathPoint || map[row, col] < 0)
                        result.Append(' ');
                    else
                        result.Append(map[row, col]);

                    result.Append("[/]");
                }
                result.Append('\n');
            }

            return result.ToString();
        }
        private static string GetTopoStyle(int topoHeight, bool isPathElement)
        {
            if (topoHeight < 0)
                return "[#252525 on #252525]";

            string[] colorMap = [
                $"[#969696 on #252525{(!isPathElement ? " invert dim" : "")}]", // 0
                $"[#fff7ec on #252525{(!isPathElement ? " invert dim" : "")}]", // 1
                $"[#fee8c8 on #252525{(!isPathElement ? " invert dim" : "")}]", // 2
                $"[#fdd49e on #252525{(!isPathElement ? " invert dim" : "")}]", // 3
                $"[#fdbb84 on #252525{(!isPathElement ? " invert dim" : "")}]", // 4
                $"[#fc8d59 on #252525{(!isPathElement ? " invert dim" : "")}]", // 5
                $"[#ef6548 on #252525{(!isPathElement ? " invert dim" : "")}]", // 6
                $"[#d7301f on #252525{(!isPathElement ? " invert dim" : "")}]", // 7
                $"[#b30000 on #252525{(!isPathElement ? " invert dim" : "")}]", // 8
                $"[#7f0000 on #252525{(!isPathElement ? " invert dim" : "")}]", // 9
            ];

            return colorMap[topoHeight];
        }
    }
}
