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

namespace Zaphyros.Core
{
    public sealed class Kernel
    {
        public Random Random => new(SeedProvider.GetSeed());

        private ISeedProvider? _seedProvider;
        public ISeedProvider SeedProvider => _seedProvider ??= new BCryptSeedProvider();

        internal static CosmosVFS Vfs { get; private set; }
        private static CommandHandler commandHandler;
        internal static TaskManager TaskManager { get; private set; }

        public void BeforeRun()
        {
            RAT.MinFreePages = 2;
            //Start Filesystem
            Vfs = new();
            commandHandler = new();
            _Prompt = @"0:\";

            VFSManager.RegisterVFS(Vfs);

            if (!File.Exists($@"0:\System\Greetings.txt"))
            {
                Console.WriteLine("Adding Greetings File...");
                if (!Directory.Exists($@"0:\System"))
                {
                    Directory.CreateDirectory($@"0:\System");
                }
                File.Create($@"0:\System").Close();
                File.WriteAllBytes($@"0:\System", SysFiles.Greetings);
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

            if (!File.Exists(SysFiles.USER_FILE))
            {
                Console.WriteLine("Adding users...");
                if (!Directory.Exists($@"0:\System"))
                {
                    Directory.CreateDirectory($@"0:\System");
                }
                File.Create(SysFiles.USER_FILE).Close();
                File.WriteAllBytes(SysFiles.USER_FILE, File.ReadAllBytes("1:\\System\\users.sys"));
                Console.WriteLine("users File Added Successfully...");
            }

            //Console.WriteLine(Encoding.ASCII.GetString(SysFiles.usrFile));
            //Console.WriteLine(File.ReadAllText("2:\\users"));
            //Console.ReadKey();

            TaskManager = new TaskManager();
            Console.WriteLine($"Registering Service {nameof(WorkFactorCalculatorService)}");
            TaskManager.RegisterService(new WorkFactorCalculatorService());
            Console.WriteLine($"Registered Service {nameof(WorkFactorCalculatorService)}");
        }

        delegate uint HashPrototype(byte[] InputText);
        public void Run()
        {
            TaskManager.Run();

            Console.ReadKey(true);
            Console.WriteLine("Let's Start:");
            foreach (var encoding in new List<Encoding>
            {
                Encoding.UTF8,
                Encoding.Unicode,
                Encoding.BigEndianUnicode,
                Encoding.ASCII,
                CosmosEncodingProvider.Instance.GetEncoding(437)!,
                CosmosEncodingProvider.Instance.GetEncoding(858)!
            })
            {
                Console.OutputEncoding = encoding;
                Console.WriteLine($"Ecoding: {encoding.BodyName}");
                Console.WriteLine("Line One");
                Console.WriteLine("Line Two");
                Console.WriteLine("Line Three");
                Thread.Sleep(1000);
            }
            Console.WriteLine($"{_Prompt}>");
            var input = Console.ReadLine();

            Console.WriteLine(input);

            commandHandler.ExecuteCommand(input);
            Heap.Collect();
        }

    }
}
