using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zaphyros.Core.Commands.FileSystem
{
    [Command("list", "List al items on a directory.")]
    internal partial class ListCommand : CommandBase
    {
        public override CommandResult Execute(string[] args, CancellationToken cancellationToken)
        {
            string path;

            path = args.Length > 0 ? args[0] : Kernel.Session.CurrentDirectory;

            if (string.IsNullOrEmpty(path)) return CommandResult.Failure;

            Console.WriteLine(path);
            Console.WriteLine(new string('-', Console.WindowWidth));
            Console.SetCursorPosition(0, Console.CursorTop - 1);

            foreach (var item in Directory.GetDirectories(path))
            {
                Console.WriteLine($"d -\t{item}");
            }

            foreach (var item in Directory.GetFiles(path))
            {
                Console.WriteLine($"f -\t{item}");
            }

            Console.WriteLine();

            return CommandResult.Success;
        }
    }
}
