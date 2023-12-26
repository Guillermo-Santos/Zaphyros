using Cosmos.System.FileSystem.VFS;
using Cosmos.System.FileSystem;
using Cosmos.Core.Memory;
using Cosmos.System.ExtendedASCII;
using System.Runtime.InteropServices;
using System.Text;
using System.Runtime.CompilerServices;
using System.Numerics;
using BCrypt.Net;
using Cosmos.Core;
using Zaphyros.Core.Security;
using Zaphyros.Core.Users;
using System.Text.Json;
using Zaphyros.Core.Configuration;
using Zaphyros.Core.Apps;
using System.Net.WebSockets;
using Zaphyros.Core.Users.Services;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Zaphyros.Core
{
    public sealed class Kernel
    {
        public Random Random => new(SeedProvider.GetSeed());

        private ISeedProvider? _seedProvider;
        public ISeedProvider SeedProvider => _seedProvider ??= new BCryptSeedProvider();

        internal static CosmosVFS Vfs { get; private set; }
        internal static TaskManager TaskManager { get; private set; }
        internal static Session Session { get; set; }

        public void BeforeRun()
        {
            //RAT.MinFreePages = 2;
            //Start Filesystem
            Vfs = new();

            VFSManager.RegisterVFS(Vfs);

            if (!File.Exists($@"0:\System\Greetings.txt"))
            {
                Console.WriteLine("Adding Greetings File...");
                if (!Directory.Exists($@"0:\System"))
                {
                    Directory.CreateDirectory($@"0:\System");
                }
                File.Create($@"0:\System\Greetings.txt").Close();
                File.WriteAllBytes($@"0:\System\Greetings.txt", SysFiles.Greetings);
                Console.WriteLine("Greetings File Added Successfully...");
            }

            if (!File.Exists($@"0:\System\System.Private.CoreLib.dll"))
            {
                Console.WriteLine("Adding System.Private.CoreLib.dll...");
                if (!Directory.Exists($@"0:\System"))
                {
                    Directory.CreateDirectory($@"0:\System");
                }
                File.Create($@"0:\System\System.Private.CoreLib.dll").Close();
                File.WriteAllBytes($@"0:\System\System.Private.CoreLib.dll", SysFiles.CorLib);
                Console.WriteLine("System.Private.CoreLib.dll File Added Successfully...");
            }

            //if (!File.Exists(SysFiles.USER_FILE))
            {
                Console.WriteLine("Adding users...");
                if (!Directory.Exists($@"0:\System"))
                {
                    Directory.CreateDirectory($@"0:\System");
                }
                File.Create($@"0:\System\users").Close();
                File.WriteAllBytes($@"0:\System\users", File.ReadAllBytes("1:\\System\\users.sys"));
                Console.WriteLine("users File Added Successfully...");
            }

            //Console.WriteLine(Encoding.ASCII.GetString(SysFiles.usrFile));
            foreach(var file in Directory.GetFiles("1:\\","*", SearchOption.AllDirectories))
            {
                Console.WriteLine(file);
            }
            //Console.ReadKey();
            //Console.WriteLine(File.ReadAllText("1:\\pass.conf"));
            //Console.ReadKey();

            TaskManager = new TaskManager();
            Console.WriteLine($"Registering Service {nameof(WorkFactorCalculatorService)}");
            TaskManager.RegisterService(new WorkFactorCalculatorService());
            Console.WriteLine($"Registered Service {nameof(WorkFactorCalculatorService)}");

        }

        public void Run()
        {
            TaskManager.Run();
        }

    }
}
