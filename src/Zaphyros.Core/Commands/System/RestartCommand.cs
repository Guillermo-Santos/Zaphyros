namespace Zaphyros.Core.Commands.System;

[Command("restart", "Restart this PC.")]
internal partial class RestartCommand : CommandBase
{
    public override CommandResult Execute(string[] args, CancellationToken cancellationToken)
    {
        var cooldown = 5;

        while (cooldown > 0)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Console.WriteLine($"Restarting in {cooldown}...");
            Thread.Sleep(1000);
            cooldown--;
        }

        Sys.Power.Reboot();
        return CommandResult.Success;
    }
}
