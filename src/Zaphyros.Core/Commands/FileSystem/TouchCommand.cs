using Zaphyros.Core.Commands;

namespace Zaphyros.Core.Commands.FileSystem
{
    [Command("touch", "Create a file without content")]
    internal partial class TouchCommand : CommandBase
    {
        public override CommandResult Execute(string[] args, CancellationToken cancellationToken)
        {
            if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
            {
                Console.WriteLine("Error: no file name provided");
                return CommandResult.Failure;
            }

            File.Create(args[0]).Close();

            return CommandResult.Success;
        }
    }
}
