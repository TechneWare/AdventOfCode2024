using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2024.Puzzles
{
    /// <summary>
    /// --- Day 8: Resonant Collinearity ---
    /// </summary>
    /// <see cref="https://adventofcode.com/2024/day/8"/>
    internal class Day8 : Puzzle
    {
        private Location[,] Map = new Location[1,1];
        private readonly HashSet<(int row, int col)> antiNodes = [];
        public Day8()
            : base(Name: "Resonant Collinearity", DayNumber: 8) { }

        public override void ParseData()
        {
            var data = DataRaw.Replace("\r", "").Split("\n", StringSplitOptions.RemoveEmptyEntries).ToArray();
            int height = data.Length;
            int width = data[0].Length;

            antiNodes.Clear();
            Map = new Location[height, width];

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    var newLocation = new Location();
                    if (data[row][col] != '.')
                    {
                        newLocation.Antenna = new Antenna() { Id = data[row][col] };
                    }

                    Map[row, col] = newLocation;
                }
            }
        }
        public override void Part1(bool isTestMode)
        {
            ParseData();

            FindAntiNodes(Map);
            
            string answer = GetMapDisplay(Map) + '\n';
            answer += $"Antinode Locations = {antiNodes.Count}\n";

            Part1Result = answer;
        }
        public override void Part2(bool isTestMode)
        {
            ParseData();

            FindAntiNodeHarmonics(Map);
            
            string answer = GetMapDisplay(Map) + '\n';
            answer += $"Antinode Locations = {antiNodes.Count}\n";

            Part2Result = answer;
        }
        private void FindAntiNodes(Location[,] map)
        {
            int height = map.GetLength(0);
            int width = map.GetLength(1);

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (!map[row, col].HasAntenna) continue;
                    ProcessOtherAntennas(row, col, map);
                }
            }
        }
        private void ProcessOtherAntennas(int antennaRow, int antennaCol, Location[,] map)
        {
            var otherAntennas = GetMatchingAntennas(antennaRow, antennaCol, map);
            foreach (var (otherRow, otherCol) in otherAntennas)
            {
                var (antiRow, antiCol) = GetAntiNode(antennaRow, antennaCol, otherRow, otherCol);

                if (IsWithinBounds(antiRow, antiCol, map))
                    antiNodes.Add((antiRow, antiCol));
            }
        }
        private void FindAntiNodeHarmonics(Location[,] map)
        {
            int height = map.GetLength(0);
            int width = map.GetLength(1);

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (!map[row, col].HasAntenna) continue;

                    ProcessAntennaHarmonics(row, col, map);
                }
            }
        }
        private void ProcessAntennaHarmonics(int row, int col, Location[,] map)
        {
            var otherAntennas = GetMatchingAntennas(row, col, map);

            if (otherAntennas.Count > 0)
                antiNodes.Add((row, col));

            foreach (var (otherRow, otherCol) in otherAntennas)
            {
                antiNodes.Add((otherRow, otherCol));
                AddAntiNodesAlongLine(row, col, otherRow, otherCol, map);
            }
        }
        private void AddAntiNodesAlongLine(int row1, int col1, int row2, int col2, Location[,] map)
        {
            var (nextRow, nextCol) = GetAntiNode(row1, col1, row2, col2);

            while (IsWithinBounds(nextRow, nextCol, map))
            {
                antiNodes.Add((nextRow, nextCol));

                (row1, col1) = (row2, col2);
                (row2, col2) = (nextRow, nextCol);
                (nextRow, nextCol) = GetAntiNode(row1, col1, row2, col2);
            }
        }
        private static bool IsWithinBounds(int row, int col, Location[,] map)
        {
            int height = map.GetLength(0);
            int width = map.GetLength(1);
            return row >= 0 && row < height && col >= 0 && col < width;
        }
        private static (int row, int col) GetAntiNode(int row1, int col1, int row2, int col2)
        {
            var rDist = -(row1 - row2);
            var cDist = -(col1 - col2);

            var antiRow = row2 + rDist;
            var antiCol = col2 + cDist;

            return (antiRow, antiCol);
        }
        private static List<(int row, int col)> GetMatchingAntennas(int startRow, int startCol, Location[,] map)
        {
            var matchingAntennas = new List<(int row, int col)>();
            int height = map.GetLength(0);
            int width = map.GetLength(1);
            var sourceAntenna = map[startRow, startCol]?.Antenna;

            if (sourceAntenna == null)
                return matchingAntennas;

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if ((row == startRow && col == startCol) || !map[row, col].HasAntenna)
                        continue;

                    if (map[row, col].Antenna?.Id == sourceAntenna.Id)
                        matchingAntennas.Add((row, col));
                }
            }

            return matchingAntennas;
        }
        private string GetMapDisplay(Location[,] map)
        {
            var result = new StringBuilder();

            int height = map.GetLength(0);
            int width = map.GetLength(1);

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    var hasAntinode = antiNodes.Contains((row, col));
                    var antenna = map[row, col].Antenna;

                    if (antenna != null)
                        result.Append($"[teal]{antenna.Id}[/]");
                    else if (hasAntinode)
                        result.Append("[yellow]#[/]");
                    else
                        result.Append("[dim].[/]");
                }
                result.Append('\n');
            }

            return result.ToString();
        }
        internal class Location
        {
            public Antenna? Antenna { get; set; }
            public bool HasAntenna => Antenna != null;
        }
        internal class Antenna
        {
            public char Id { get; set; }
        }
    }
}
