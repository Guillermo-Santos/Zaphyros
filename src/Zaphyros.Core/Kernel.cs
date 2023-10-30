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

namespace Zaphyros.Core
{
    public sealed class Kernel
    {
        public Random Random => new(SeedProvider.GetSeed());

        private ISeedProvider? _seedProvider;
        public ISeedProvider SeedProvider => _seedProvider ??= new BCryptSeedProvider();

        private string? _Prompt;
        internal static CosmosVFS Vfs { get; private set; }
        private static CommandHandler commandHandler;

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
                if (!Directory.Exists($@"{_Prompt}System"))
                {
                    Directory.CreateDirectory($@"{_Prompt}System");
                }
                File.Create($@"{_Prompt}System\Greetings.txt").Close();
                File.WriteAllBytes($@"{_Prompt}System\Greetings.txt", SysFiles.Greetings);
                Console.WriteLine("Greetings File Added Successfully...");
            }

            if (!File.Exists($@"0:\System\System.Private.CoreLib.dll"))
            {
                Console.WriteLine("Adding System.Private.CoreLib.dll...");
                if (!Directory.Exists($@"{_Prompt}System"))
                {
                    Directory.CreateDirectory($@"{_Prompt}System");
                }
                File.Create($@"{_Prompt}System\System.Private.CoreLib.dll").Close();
                File.WriteAllBytes($@"{_Prompt}System\System.Private.CoreLib.dll", SysFiles.CorLib);
                Console.WriteLine("System.Private.CoreLib.dll File Added Successfully...");
            }

        }

        delegate uint HashPrototype(byte[] InputText);
        public void Run()
        {
            Console.WriteLine(BigInteger.Parse("2323333333333333333333333333232323232"));

            /*
            var opt  = SourceGenerationContext.Default.Options;
            var user2 = (User)JsonSerializer.Deserialize(jsonString, typeof(User), opt)!;
            Console.WriteLine($"Name={user2.Name}");*/

            /*
            var hash = BCrypt.Net.BCrypt.EnhancedHashPassword("Hello World!", 15, HashType.SHA256);
            Console.WriteLine(hash);
            Console.WriteLine(BCrypt.Net.BCrypt.EnhancedVerify("Hello World!", hash, HashType.SHA256));
            */



            //var bi = new BigInteger(0);
            //Console.WriteLine($"BigInt: {bi}");

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
