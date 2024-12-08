using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2024
{
    public static class SysInfo
    {
        public static long TotalMemoryMB;
        public static int ProcessorCount;
        static SysInfo()
        {
            TotalMemoryMB = Tools.SystemInfo.GetTotalMemoryMB();
            ProcessorCount = Tools.SystemInfo.GetProcessorCount();
        }
    }
}
