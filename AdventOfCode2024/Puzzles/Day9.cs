using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Numerics;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2024.Puzzles
{
    /// <summary>
    /// --- Day 9: Disk Fragmenter ---
    /// </summary>
    /// <see cref="https://adventofcode.com/2024/day/9"/>
    internal class Day9 : Puzzle
    {
        private List<string> DiskMaps = [];
        public Day9()
            : base(Name: "Disk Fragmenter", DayNumber: 9) { }

        public override void ParseData()
        {
            DiskMaps.Clear();
            DiskMaps = [.. DataRaw.Replace("\r", "").Split("\n", StringSplitOptions.RemoveEmptyEntries)];
        }
        public override void Part1(bool isTestMode)
        {
            ParseData();

            string answer = "";

            List<Disk> disks = [];
            foreach (var map in DiskMaps)
            {
                if (isTestMode || WithLogging)
                {
                    answer += $"[bold]Disk Map:[/][teal]{map}[/]\n";
                    answer += $"[yellow underline]Defraging Disk[/]\n\n";
                }

                var d = new Disk(map);

                var defragger = d.Defrag().GetEnumerator();

                if (isTestMode || WithLogging) answer += $"{d}\n";

                while (defragger.MoveNext())
                {
                    if (isTestMode || WithLogging)
                        answer += $"{d}\n";

                    if (defragger.Current) //true when defrag complete
                        break;
                }

                if (isTestMode || WithLogging)
                    answer += $"\n";

                disks.Add(d);
            }

            for (int d = 0; d < disks.Count; d++)
            {
                answer += $"[green1]Disk[/] [yellow bold]{d}[/] [green1]Checksum([/][yellow bold]{disks[d].CheckSum}[/][green1])[/]\n";
                if (isTestMode) answer += $"=> [teal]{disks[d]}[/]\n";
            }

            Part1Result = answer;
        }
        public override void Part2(bool isTestMode)
        {
            ParseData();

            string answer = "";

            List<Disk> disks = [];
            foreach (var map in DiskMaps)
            {
                if (isTestMode || WithLogging)
                {
                    answer += $"[bold]Disk Map:[/][teal]{map}[/]\n";
                    answer += $"[yellow underline]Defraging Disk[/]\n\n";
                }

                var d = new Disk(map);

                var defragger = d.DefragBlockMove().GetEnumerator();

                if (isTestMode || WithLogging) answer += $"{d}\n";

                while (defragger.MoveNext())
                {
                    if (!defragger.Current && (isTestMode || WithLogging))
                        answer += $"{d}\n";

                    if (defragger.Current) //true when defrag complete
                        break;
                }

                if (isTestMode || WithLogging)
                    answer += $"\n";

                disks.Add(d);
            }

            for (int d = 0; d < disks.Count; d++)
            {
                answer += $"[green1]Disk[/] [yellow bold]{d}[/] [green1]Checksum([/][yellow bold]{disks[d].CheckSum}[/][green1])[/]\n";
                if (isTestMode) answer += $"=> [teal]{disks[d]}[/]\n";
            }

            Part2Result = answer;
        }
        public class Disk
        {
            public int Length => Blocks.Length;
            public string Map { get; private set; }
            public int[] Blocks { get; private set; } = [];
            public List<(int Id, List<int> BlockIds)> FAT { get; set; } = [];
            public long CheckSum => CalcChecksum();
            public Disk(string map)
            {
                Map = map;

                GenerateDisk();
            }
            private void GenerateDisk()
            {
                int fileId = 0;
                int diskptr = 0;
                int mapptr = 0;
                var blocks = new List<int>();

                while (mapptr < Map.Length)
                {
                    var numBlocks = int.Parse(Map[mapptr++].ToString());
                    var emptyBlocks = mapptr < Map.Length ? int.Parse(Map[mapptr++].ToString()) : 0;

                    var fileBlocks = new List<int>();
                    for (int b = 0; b < numBlocks; b++)
                    {
                        blocks.Add(fileId);
                        fileBlocks.Add(diskptr++);
                    }

                    FAT.Add((fileId++, fileBlocks));

                    for (int b = 0; b < emptyBlocks; b++)
                    {
                        blocks.Add(-1);
                        diskptr++;
                    }
                }

                Blocks = [.. blocks];
            }
            public IEnumerable<bool> Defrag()
            {
                bool isDefragDone = false;
                var diskEnum = GetNextEmptyBlock().GetEnumerator();

                var freePtr = 0;
                while (!isDefragDone)
                {
                    var fid = GetLastFileId();
                    var (FileId, FatIds) = FAT.Where(f => f.Id == fid).Single();
                    int fileIdx = FatIds.IndexOf(FatIds.Max());

                    if (!diskEnum.MoveNext())
                        Debugger.Break();

                    freePtr = diskEnum.Current;

                    Blocks[freePtr] = FileId;
                    Blocks[FatIds[fileIdx]] = -1;
                    FatIds[fileIdx] = freePtr;

                    var maxUsedBlockId = GetLastFileBlock();
                    var firstEmptyBlock = GetFirstEmptyBlock();
                    isDefragDone = maxUsedBlockId < firstEmptyBlock;

                    yield return isDefragDone;
                }

                yield return false;
            }
            public IEnumerable<bool> DefragBlockMove()
            {
                bool isDefragDone = false;

                var fileId = GetLastFileId();
                while (!isDefragDone)
                {
                    (int _, List<int> FatIds) = FAT.Where(f => f.Id == fileId).Single();
                    var (blockFound, startBlock) = GetEmptyBlock(FatIds.Count, FatIds.Min());

                    if (blockFound)
                    {
                        int freePtr = startBlock;
                        for (int fbIdx = 0; fbIdx < FatIds.Count; fbIdx++)
                        {
                            Blocks[freePtr] = fileId;
                            Blocks[FatIds[fbIdx]] = -1;
                            FatIds[fbIdx] = freePtr;

                            freePtr++;
                        }
                    }

                    fileId--;
                    isDefragDone = fileId < 0;

                    if (blockFound || isDefragDone)
                        yield return isDefragDone;
                }

                yield return true;
            }
            private long CalcChecksum()
            {

                return Blocks
                        .Select((file, i) => i * (file >= 0 ? file : 0L))
                        .Sum();
            }
            private (bool blockFound, int startBlock) GetEmptyBlock(int targetSize, int endBlockId)
            {
                bool blockFound = false;

                int diskPtr = 0;
                int startPtr = 0;

                while (diskPtr < endBlockId)
                {
                    int blockSize = diskPtr - startPtr;
                    if (blockSize == targetSize)
                    {
                        blockFound = true;
                        break;
                    }

                    bool inEmptyBlock = Blocks[diskPtr++] == -1;
                    if (!inEmptyBlock) startPtr = diskPtr;
                }

                return (blockFound, startPtr);
            }
            private IEnumerable<int> GetNextEmptyBlock()
            {
                for (int diskptr = 0; diskptr < Length; diskptr++)
                {
                    if (Blocks[diskptr] == -1)
                        yield return diskptr;
                }

                throw new Exception("Unable to locate an empty block");
            }
            private int GetFirstEmptyBlock()
            {
                for (int diskptr = 0; diskptr < Length; diskptr++)
                {
                    if (Blocks[diskptr] == -1)
                        return diskptr;
                }

                throw new Exception("Unable to find empty block");
            }
            private int GetLastFileId()
            {
                for (int i = Length - 1; i >= 0; i--)
                {
                    if (Blocks[i] != -1)
                    {
                        return Blocks[i];
                    }
                }

                throw new Exception("Unable to locate last file id");
            }
            private int GetLastFileBlock()
            {
                for (int i = Length - 1; i > 0; i--)
                {
                    if (Blocks[i] != -1)
                    {
                        return i;
                    }
                }

                throw new Exception("Unable to locate last file block");
            }
            public override string? ToString()
            {
                var result = new StringBuilder();
                for (int i = 0; i < Length; i++)
                {
                    if (Blocks[i] == -1)
                        result.Append('.');
                    else if (Blocks[i] < 10)
                        result.Append(Blocks[i]);
                    else
                        result.Append('#');
                }

                return result.ToString();
            }

        }
    }
}
