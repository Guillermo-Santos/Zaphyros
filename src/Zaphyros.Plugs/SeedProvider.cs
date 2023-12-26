using Cosmos.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zaphyros.Core.Security;
using Cosmos.System.Helpers;
using Cosmos.System.Network.IPv4.TCP;
using Cosmos.System.Network.IPv4;
using Cosmos.System.Network;
using IL2CPU.API.Attribs;
using System.Net.Sockets;
using System.Net;
using System.Runtime.CompilerServices;

namespace Zaphyros.Plugs
{
    internal class SeedProvider : ISeedProvider
    {
        public int GetSeed()
        {
            // Get the current timestamp (number of milliseconds since January 1, 1970 UTC)
            ulong timestamp = (ulong)(DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond);

            return (int)timestamp;
        }
    }

}
