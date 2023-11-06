using System;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using BCrypt.Net;
using Cosmos.System.ExtendedASCII;
using Zaphyros.Plugs;
//using Console = System.Console;

namespace Zaphyros
{
    public class Logger<T>
    {
        private readonly Type Type = typeof(T);
        //private readonly unsafe int syze = sizeof(T);
        public void TraceMessage(string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            Console.WriteLine("message: " + message);

            Console.WriteLine("member Type AssemblyQualifiedName: " + Type.AssemblyQualifiedName);
            Console.WriteLine("member Type Name: " + Type.Name);
            Console.WriteLine("member Type Namespace: " + Type.Namespace);
            //Console.WriteLine("member Type Size: " + syze);

            Console.WriteLine("member name: " + memberName);
            Console.WriteLine("source file path: " + sourceFilePath);
            Console.WriteLine("source line number: " + sourceLineNumber);
        }
    }

    public sealed class Kernel : Sys.Kernel
    {
        private Core.Kernel kernel;

        internal static string[] welcomeMessages = new string[]
        {
            "Welcome aboard! Prepare for a stellar computing experience.",
            "Yeah, we did it.",
            "Greetings, space traveler! Get ready to explore the frontiers of technology.",
            "Welcome to the future of computing. Buckle up and enjoy the ride!",
            "Initializing your journey into the digital cosmos. Welcome to a world of infinite possibilities.",
            "Welcome to the realm of innovation and discovery. Your adventure begins now.",
            "Step into the unknown. Welcome to a universe of boundless potential.",
            "Welcome, fellow explorer! Unleash your imagination and push the boundaries of what's possible.",
            "Systems online. Welcome to a world where imagination meets reality.",
            "Welcome to the nexus of creativity and functionality. Let's embark on a revolutionary computing experience.",
            "Prepare for liftoff! Welcome to a seamless fusion of technology and user-centric design."
        };

        protected override void BeforeRun()
        {
            // Kernel Initialization
            kernel = new();

            Encoding.RegisterProvider(CosmosEncodingProvider.Instance);

            //Console.ReadLine();

            Console.WriteLine("Setting SeedProvider");
            BCryptImpl.SeedProvider = kernel.SeedProvider;

            kernel.BeforeRun();

            Console.Clear();
            Console.WriteLine("Zaphyros booted successfully.");

            // Random Welcome Message
            var messageIndex = kernel.Random.Next(welcomeMessages.Length);
            Console.WriteLine(welcomeMessages[messageIndex]);
        }

        protected override void Run() => kernel.Run();
    }
}
