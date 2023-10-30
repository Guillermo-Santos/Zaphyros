namespace Zaphyros.Core.Commands.System;

[Command("shutdown", "Turn off this PC.")]
internal partial class ShutDownCommand : CommandBase
{
    public override CommandResult Execute(string[] args, CancellationToken cancellationToken)
    {
        var cooldown = 5;
        while (cooldown > 0)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Console.WriteLine($"Shutdown in {cooldown}...");
            Thread.Sleep(1000);
            cooldown--;
        }

        Sys.Power.Shutdown();

        return CommandResult.Success;
    }
}
