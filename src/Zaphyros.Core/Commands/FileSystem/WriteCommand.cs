using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zaphyros.Core.Commands.FileSystem
{
    [Command("write", "Write on a file or create a new file.")]
    internal partial class WriteCommand : CommandBase
    {
        public override CommandResult Execute(string[] args, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
