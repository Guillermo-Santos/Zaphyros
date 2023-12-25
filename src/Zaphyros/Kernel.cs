using System.Text;
using Cosmos.System.ExtendedASCII;
using ThunderboltIoc;
using Sys = Cosmos.System;
using Zaphyros.Plugs;
using StrongInject;
using Zaphyros.Core.Logging;
using Microsoft.Extensions.Logging;
//using Console = System.Console;

namespace Zaphyros
{
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


            //var logger = new Logger<Kernel>(new LoggerFactory()
            //    .UseConsole()
            //    .UseDebugger());
            var logger = new LoggerFactory()
                .UseConsole()
                .UseDebugger()
                .CreateLogger(nameof(Kernel));

            logger.LogInformation("Hola Mundo");

            Encoding.RegisterProvider(CosmosEncodingProvider.Instance);

            Console.WriteLine("Setting SeedProvider");
            BCryptImpl.SeedProvider = kernel.SeedProvider;

            kernel.BeforeRun();

            Console.Clear();
            Console.WriteLine("Zaphyros booted successfully.");

            // Random Welcome Message
            var messageIndex = kernel.Random.Next(welcomeMessages.Length);
            Console.WriteLine(welcomeMessages[messageIndex]);

            //Console.WriteLine(typeof(Kernel).IsInstanceOfType(messageIndex));
            //Console.ReadKey();


            // The code below, following the IoC pattern, is typically only aware of the IMessageWriter interface, not the implementation.
            //ThunderboltActivator.Attach<FooThunderboltRegistration>();
            //IMessageWriter messageWriter = ThunderboltActivator.Get<IMessageWriter>();
            //messageWriter.Write("Hello");
        }

        protected override void Run() => kernel.Run();
    }

    public partial class FooThunderboltRegistration : ThunderboltRegistration
    {
        protected override void Register(IThunderboltRegistrar reg)
        {
            reg.AddSingleton<MessageWriter>();
            reg.AddScoped<IMessageWriter, MessageWriter>();
            reg.AddScoped<ScopedMessageWriter>();
            //reg.AddTransientFactory<Qux>(() => new Qux());
        }
    }

    public interface IMessageWriter
    {
        void Write(string message);
    }

    internal class MessageWriter : IMessageWriter
    {
        public void Write(string message)
        {
            Console.WriteLine($"MessageWriter.Write(message: \"{message}\")");
        }
    }

    internal class ScopedMessageWriter : IMessageWriter
    {
        private readonly IMessageWriter messageWriter;

        public ScopedMessageWriter(IMessageWriter messageWriter)
        {
            this.messageWriter = messageWriter;
        }
        public void Write(string message)
        {
            messageWriter.Write(message);
        }
    }

    [Register(typeof(MessageWriter))]
    internal partial class Container : IContainer<MessageWriter> { }
}
