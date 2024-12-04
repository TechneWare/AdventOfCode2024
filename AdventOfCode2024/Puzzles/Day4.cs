using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2024.Puzzles
{
    /// <summary>
    /// --- Day 4: Ceres Search ---
    /// </summary>
    /// <see cref="https://adventofcode.com/2024/day/4"/>
    internal class Day4 : Puzzle
    {
        private char[,] board = new char[1, 1];
        private int width = 0;
        private int height = 0;

        public Day4()
            : base(Name: "Ceres Search", DayNumber: 4) { }

        public override void ParseData()
        {
            var lines = DataRaw.Replace("\r", "").Split('\n', StringSplitOptions.RemoveEmptyEntries);
            width = lines[0].Length;
            height = lines.Length;

            board = new char[width, height];
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    board[row, col] = lines[row][col];
                }
            }
        }

        public override void Part1(bool isTestMode)
        {
            ParseData();

            string pattern = "XMAS";
            int numFound = 0;
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    numFound += GetVectorsFrom(row, col, pattern)
                               .Count(v => v == pattern);
                }
            }

            Part1Result = $"XMAS Count = {numFound}\n";
        }

        public override void Part2(bool isTestMode)
        {
            ParseData();

            string pattern = "MAS";
            int numFound = 0;
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (GetDiagonalsFrom(row, col, pattern)
                       .Count(v => v == pattern) == 2)
                            numFound++;
                }
            }

            Part2Result = $"X-MAS Count = {numFound}\n";
        }

        private enum Directions
        {
            North,
            NorthEast,
            East,
            SouthEast,
            South,
            SouthWest,
            West,
            NorthWest,
        }
        private List<string> GetVectorsFrom(int row, int col, string pattern)
        {
            var result = new List<string>();

            //skip if first char does not match pattern
            if (board[row, col] != pattern[0])
                return result;

            foreach (var dir in Enum.GetValues(typeof(Directions)))
            {
                int r = row;
                int c = col;

                var vect = new StringBuilder();
                for (int i = 0; i < pattern.Length; i++)
                {
                    vect.Append(board[r, c]);
                    switch (dir)
                    {
                        case Directions.North:
                            r--;
                            break;
                        case Directions.NorthEast:
                            r--;
                            c++;
                            break;
                        case Directions.East:
                            c++;
                            break;
                        case Directions.SouthEast:
                            r++;
                            c++;
                            break;
                        case Directions.South:
                            r++;
                            break;
                        case Directions.SouthWest:
                            r++;
                            c--;
                            break;
                        case Directions.West:
                            c--;
                            break;
                        case Directions.NorthWest:
                            r--;
                            c--;
                            break;
                    }

                    //end if off the board
                    if (r < 0 || r >= height || c < 0 || c >= width)
                        break;
                }
                
                //only keep vectors that match in length
                if (vect.Length == pattern.Length)
                    result.Add(vect.ToString());
            }

            return result;
        }
        private enum Diagonals
        {
            NorthEast,
            SouthEast,
            SouthWest,
            NorthWest,
        }
        private List<string> GetDiagonalsFrom(int row, int col, string pattern)
        {
            var result = new List<string>();
            
            //skip if center char does not match pattern
            var l = 1 - pattern.Length % 2;
            var center = pattern.Length / 2 - l;
            if (board[row, col] != pattern[center])
                return result;

            foreach (var dir in Enum.GetValues(typeof(Diagonals)))
            {
                int r = row;
                int c = col;

                //move to start position
                var distance = (pattern.Length - 1) / 2;
                for (int i = 0; i < distance; i++)
                {
                    switch (dir)
                    {
                        case Diagonals.NorthEast:
                            r++;
                            c--;
                            break;
                        case Diagonals.SouthEast:
                            r--;
                            c--;
                            break;
                        case Diagonals.SouthWest:
                            r--;
                            c++;
                            break;
                        case Diagonals.NorthWest:
                            r++;
                            c++;
                            break;
                    }
                }

                //skip if off the board
                if (r < 0 || r >= height || c < 0 || c >= width)
                    continue;

                //skip if first char does not match pattern
                if (board[r, c] != pattern[0])
                    continue;

                var vect = new StringBuilder();
                for (int i = 0; i < pattern.Length; i++)
                {
                    vect.Append(board[r, c]);
                    switch (dir)
                    {
                        case Diagonals.NorthEast:
                            r--;
                            c++;
                            break;
                        case Diagonals.SouthEast:
                            r++;
                            c++;
                            break;
                        case Diagonals.SouthWest:
                            r++;
                            c--;
                            break;
                        case Diagonals.NorthWest:
                            r--;
                            c--;
                            break;
                    }

                    //end if off the board
                    if (r < 0 || r >= height || c < 0 || c >= width)
                        break;
                }

                //only keep vectors that match in length
                if (vect.Length == pattern.Length)
                    result.Add(vect.ToString());
            }

            return result;
        }
    }
}
