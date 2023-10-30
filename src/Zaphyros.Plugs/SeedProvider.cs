using Cosmos.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zaphyros.Core.Security;
using XSharp;
using XSharp.Assembler.x86;

namespace Zaphyros.Plugs
{
    internal class SeedProvider : ISeedProvider
    {
        public int GetSeed()
        {
            // Get the current timestamp (number of milliseconds since January 1, 1970 UTC)
            ulong timestamp = (ulong)(DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond);

            // Get the hash code of the current thread object
            ulong upTime = CPU.GetCPUUptime();

            // Combine the timestamp and thread hash code to create a unique seed
            int seed = (int)(timestamp ^ upTime);

            return seed;
        }
    }
}
