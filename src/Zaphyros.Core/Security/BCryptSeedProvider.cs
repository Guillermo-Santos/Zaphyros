using Cosmos.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zaphyros.Core.Security
{
    internal class BCryptSeedProvider : ISeedProvider
    {
        public int GetSeed()
        {
            unchecked
            {
                // TODO: Advance to the unreachable code as the SO Development Progress.
                long ticks = DateTime.UtcNow.Ticks;
                ulong cpuTime = CPU.GetCPUUptime();
                uint kernelEnd = CPU.GetEndOfKernel();
                uint stackStart = CPU.GetStackStart();
                int vendorName = CPU.GetCPUVendorName().GetHashCode();
                var diskSize = Kernel.Vfs.GetTotalSize("0:\\");
                var diskFreeSpace = Kernel.Vfs.GetAvailableFreeSpace("0:\\");
                var diskFillSpace = diskSize - diskFreeSpace;
                var guid = Guid.NewGuid().GetHashCode();

                object[] seeds =
                {
                    DateTime.UtcNow.Ticks,
                    cpuTime,
                    kernelEnd,
                    kernelEnd,
                    diskSize,
                    diskFreeSpace,
                    diskFillSpace,
                    guid
                };


                // Combine the timestamp and thread hash code to create a unique seed
                int Seed = (int)(cpuTime ^ kernelEnd ^ stackStart) ^ vendorName ^ (int)ticks;
                var operationRandom = new Random(Seed);
                Seed ^= operationRandom.Next();
                Seed = Seed << 17 | Seed >> 47;

                foreach (var seed in seeds)
                {
                    Seed = (int)PerformOperation(PerformOperation((ulong)Seed) ^ PerformOperation((ulong)seed));
                }
                Seed ^= operationRandom.Next();
                Seed = Seed << 23 | Seed >> 41;
                /*
                Seed = (int)PerformOperation((PerformOperation(cpuTime)
                             ^ PerformOperation(kernelEnd)
                             ^ PerformOperation(stackStart)
                             ^ PerformOperation((ulong)vendorName)
                             ^ PerformOperation((ulong)ticks)
                             ^ PerformOperation((ulong)operationRandom.Next())) ^ (ulong)operationRandom.Next());
                */
                Console.WriteLine($"Seed: " + Seed);

                return Seed;
            }

        }

        #region Helper Methods
        private static ulong PerformOperation(ulong data)
        {
            var random = new Random((int)data);
            int operation = random.Next(13);
            ulong operand = (ulong)random.Next(); // Get a random operand for arithmetic operations

            switch (operation)
            {
                case 0:
                    /* Do Nothing */
                    break;
                case 1:
                    data += operand;
                    break;
                case 2:
                    data -= operand;
                    break;
                case 3:
                    data *= operand;
                    break;
                case 4:
                    data /= operand;
                    break;
                case 5:
                    data ^= operand;
                    break;
                case 6:
                    data |= operand;
                    break;
                case 7:
                    data &= operand;
                    break;
                case 8:
                    data <<= (int)(operand % 64);
                    break;
                case 9:
                    data >>= (int)(operand % 64);
                    break;
                case 10:
                    data = (ulong)((long)data - (long)data);
                    break;
                case 11:
                    data = ~data;
                    break;
                case 12:
                    if (operand != 0) data %= operand;
                    break;
            }
            return data;
        }
        #endregion
    }
}
