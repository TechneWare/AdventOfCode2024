using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2024.Tools
{
    public static class SystemInfo
    {
        public static long GetTotalMemoryMB()
        {
            if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
                return GetLinuxTotalMemory();
            else
                return GC.GetGCMemoryInfo().TotalAvailableMemoryBytes / (1024 * 1024);
        }
        public static int GetProcessorCount()
        {
            return Environment.ProcessorCount;
        }
        private static long GetLinuxTotalMemory()
        {
            try
            {
                var lines = File.ReadAllLines("/proc/meminfo");
                foreach (var line in lines)
                {
                    if (line.StartsWith("MemTotal"))
                    {
                        var parts = line.Split(':')[1].Trim().Split(' ')[0];
                        return long.Parse(parts) / 1024; // Convert from KB to MB
                    }
                }
            }
            catch
            {
                return -1;
            }
            return 0;
        }
    }
}
