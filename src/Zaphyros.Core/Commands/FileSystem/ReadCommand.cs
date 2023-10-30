using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zaphyros.Core.Commands.FileSystem
{
    [Command("read", "Reads a file Content.")]
    internal partial class ReadCommand : CommandBase
    {
        public override CommandResult Execute(string[] args, CancellationToken cancellationToken)
        {
            try
            {
                Console.WriteLine(File.ReadAllText(args[0]));
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"File \"{args[0]}\" does not exists...");
                return CommandResult.Failure;
            }

            return CommandResult.Success;
        }
    }
}
