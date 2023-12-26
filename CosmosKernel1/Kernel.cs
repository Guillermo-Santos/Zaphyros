using System;
using Zaphyros.Logging;

namespace CosmosKernel1
{
    public class Kernel : Sys.Kernel
    {
        protected override void BeforeRun()
        {
            var factory = new LoggerFactory();
            factory.UseConsole()
                //.UseDebugger();
                ;
            var logger = (IScopableLogger)factory.CreateLogger(nameof(Kernel));
            //var scope = logger.BeginScope<Kernel>(this);

            var a = true;

            if (a)
            {
                var r = (IScopableLogger)factory.CreateLogger("Default Instantiations");
                r.Log(LogLevel.Trace, 1, "", null, (string state, Exception? error) => "Direct Method Link!");
                r.Log(LogLevel.Trace, 1, "", null, (string state, Exception? error) => "\n\tasds\tDire\n\tsd\tct Method Link!");
                r.LogInformation("Juhu~");
            }

            logger.Log(LogLevel.Information, 1, "", null, (string state, Exception? error) => "Cosmos booted successfully. Type a line of text to get it echoed back.");
            Microsoft.Extensions.Logging.LoggerExtensions.LogInformation(logger, "Cosmos booted successfully. Type a line of text to get it echoed back.");

            //scope.Dispose();
            factory.Dispose();
        }

        protected override void Run()
        {
            Console.Write("Input: ");
            var input = Console.ReadLine();
            Console.Write("Text typed: ");
            Console.WriteLine(input);
        }
    }
}
