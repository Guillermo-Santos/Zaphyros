using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zaphyros.Core.Commands
{

    public interface ICommand
    {
        public CommandInfo Info { get; }
        public CommandResult Execute(string[] args, CancellationToken cancellationToken);
    }
}
