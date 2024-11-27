using AdventOfCode2024.Commands;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2024.Puzzles
{
    public abstract class Puzzle : IPuzzle
    {
        private readonly string name;
        private readonly double dayNumber;
        public bool WithLogging = false;
        public double DayNumber => dayNumber;
        public string Name => name;
        public string Part1Result { get; set; } = "Not Run";
        public string Part2Result { get; set; } = "Not Run";

        public string DataRaw { get; set; } = "";

        public Puzzle()
        {
            name = "No Name";
            dayNumber = 0;
        }

        public Puzzle(string Name, double DayNumber)
        {
            name = Name;
            dayNumber = DayNumber;
        }
        public void Run()
        {
            DisplayHeading();

            if (Settings.ShowPuzzleText)
                ShowPuzzleText(partNum: 1);
            foreach (var mode in new bool[] { true, false })
            {
                WithLogging = Settings.ShowLog && mode;

                var start = DateTime.Now;
                Part1(mode);
                var duration = (DateTime.Now - start).TotalSeconds;
                Console.WriteLine($"{(mode ? "Test" : "Actual"),-7}Day {DayNumber,-4} Part1: {Part1Result,-60}{duration:F8} Seconds");
            }

            if (Settings.ShowPuzzleText)
                ShowPuzzleText(partNum: 2);
            foreach (var mode in new bool[] { true, false })
            {
                WithLogging = Settings.ShowLog && mode;

                var start = DateTime.Now;
                Part2(mode);
                var duration = (DateTime.Now - start).TotalSeconds;
                Console.WriteLine($"{(mode ? "Test" : "Actual"),-7}Day {dayNumber,-4} Part2: {Part2Result,-60}{duration:F8} Seconds");
            }
        }

        private void DisplayHeading()
        {
            var clr = Console.ForegroundColor;
            var bkc = Console.BackgroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.BackgroundColor = ConsoleColor.Blue;
            var title = $"--- Day {DayNumber} {name} ---";
            var padding = (Console.WindowWidth / 2) + title.Length / 2;
            Console.Write($"{title.PadLeft(padding)}");
            ClearLineFromCursorToEnd();
            Console.ForegroundColor = clr;
            Console.BackgroundColor = bkc;
            Console.WriteLine("\n");
        }

        public void LoadData(bool isTestMode, int partNum)
        {
            var fileName = GetInputFileName(isTestMode, partNum);
            if (File.Exists(fileName))
                DataRaw = File.ReadAllText(fileName);
            else
                Console.WriteLine($"Required Input File Missing:\n{fileName}");
        }

        public abstract void ParseData();
        public abstract void Part1(bool TestMode);
        public abstract void Part2(bool TestMode);

        private string GetInputFileName(bool TestMode, int partNum)
        {
            if (TestMode)
                return $@"{Environment.CurrentDirectory}/Puzzles/Data/Day{DayNumber}/InputTest{partNum}.txt";
            else
                return $@"{Environment.CurrentDirectory}/Puzzles/Data/Day{DayNumber}/Input.txt";
        }
        private string GetPuzzleFileName(int partNum)
        {
            return $@"{Environment.CurrentDirectory}/Puzzles/Data/Day{DayNumber}/Part{partNum}Puzzle.txt";
        }
        public void Log(string Message)
        {
            Log("", Message);
        }
        public void Log(string Title, string Message)
        {
            if (WithLogging && (!string.IsNullOrEmpty(Title) || !string.IsNullOrEmpty(Message)))
            {
                var forColor = Console.ForegroundColor;
                var bakColor = Console.BackgroundColor;

                Console.ForegroundColor = ConsoleColor.Green;
                Console.BackgroundColor = ConsoleColor.Blue;
                if (!string.IsNullOrEmpty(Title))
                    Console.WriteLine($"\n--> {Title}");
                if (!string.IsNullOrEmpty(Message))
                    DisplayWithMoreCommand(Message);
                Console.BackgroundColor = bakColor;
                Console.ForegroundColor = ConsoleColor.Yellow;

                Console.WriteLine("\nPress any key to Continue or Q to stop logging\n");
                Console.ForegroundColor = forColor;

                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.Q)
                    WithLogging = false;
            }
        }
        public void ShowPuzzleText(int partNum)
        {
            var fname = GetPuzzleFileName(partNum);
            if (File.Exists(fname))
            {
                var puzzleText = File.ReadAllText(fname);
                DisplayWithMoreCommand(puzzleText);
            }
        }
        private void DisplayWithMoreCommand(string text)
        {
            int consoleWidth = Console.WindowWidth;
            int consoleHeight = Console.WindowHeight - 3;
            var bakColor = Console.BackgroundColor;
            var forColor = Console.ForegroundColor;
            
            // Wrap text to fit the console window width
            string wrappedText = WrapText(text, consoleWidth);

            // Split the wrapped text into lines
            string[] lines = wrappedText.Split("\n");

            int currentPage = 0;
            int totalPages = (int)Math.Ceiling(lines.Length / (double)consoleHeight);
            int charCount = 0;

            while (currentPage < totalPages)
            {
                bool slowMode = true;
                Console.ForegroundColor = ConsoleColor.Green;
                for (int i = 0; i < consoleHeight && currentPage * consoleHeight + i < lines.Length; i++)
                {
                    var line = lines[currentPage * consoleHeight + i];

                    if (Console.KeyAvailable)
                    {
                        slowMode = false;
                        Console.ReadKey();
                    }

                    if (slowMode)
                    {
                        foreach (var ch in line)
                        {
                            charCount++;
                            Console.Write(ch);
                            if (!Console.KeyAvailable)
                                Thread.Sleep(5);
                        }
                        Console.WriteLine();
                        if (line == string.Empty)
                        {
                            Thread.Sleep(charCount * 20);
                            charCount = 0;
                        }
                    }
                    else
                        Console.WriteLine(line, Color.WhiteSmoke);
                }
                Console.ForegroundColor = forColor;

                // Check if there's more content to display
                if (currentPage < totalPages - 1 && !slowMode)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.Write("\nPress any key to see more...");
                    Console.BackgroundColor = bakColor;
                    Console.ForegroundColor = forColor;

                    Console.ReadKey(); // Wait for user input

                    Console.SetCursorPosition(0, Console.CursorTop);
                    for (int i = 0; i < 2; i++)
                    {
                        ClearLineFromCursorToEnd();
                        Console.SetCursorPosition(0, Console.CursorTop - 1);
                    }
                    Console.SetCursorPosition(0, Console.CursorTop + 1);
                }

                currentPage++;
            }
        }
        private void ClearLineFromCursorToEnd()
        {
            int currentLineCursor = Console.CursorTop;
            int currentColumnCursor = Console.CursorLeft;

            int widthToClear = Console.WindowWidth - currentColumnCursor;
            Console.Write(new string(' ', widthToClear));

            Console.SetCursorPosition(currentColumnCursor, currentLineCursor);
        }
        private string WrapText(string text, int consoleWidth)
        {
            string[] words = text.Replace("\r", "").Replace("\n", " [NEW_LINE] ").Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string result = "";
            string line = "";

            foreach (string word in words)
            {
                if (word == "[NEW_LINE]" || line.Length + word.Length + 1 > consoleWidth)
                {
                    result += line + "\n";
                    line = "";
                }

                if (word != "[NEW_LINE]")
                    line += word + " ";
            }

            if (line.Length > 0)
            {
                result += line; // Add the last line
            }

            return result.TrimEnd();
        }
    }
}
