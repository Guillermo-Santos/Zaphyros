using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zaphyros.Core.Security
{
    public interface ISeedProvider
    {
        public int GetSeed();
    }
}
