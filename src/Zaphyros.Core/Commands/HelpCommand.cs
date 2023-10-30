namespace Zaphyros.Core.Commands
{
    internal class HelpCommand : CommandBase
    {
        private HelpCommand(string name, string description, CommandType type) : base(new(name, description, type)) { }

        public HelpCommand() : this("help", "Show All Commands Info.", CommandType.Info) { }

        public override CommandResult Execute(string[] args, CancellationToken cancellationToken)
        {
            string? commandToShowHelp = args.Length > 0 ? args[0] : default;

            foreach (var command in CommandHandler.Commands.Values.Where(c => string.IsNullOrEmpty(commandToShowHelp) || c.Info.Name == commandToShowHelp))
            {
                Console.WriteLine(command.Info);
                cancellationToken.ThrowIfCancellationRequested();
            }

            return CommandResult.Success;
        }
    }
}
