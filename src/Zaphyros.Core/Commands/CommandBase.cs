namespace Zaphyros.Core.Commands
{
    public abstract class CommandBase : ICommand
    {
        private readonly CommandInfo _info;
        public CommandInfo Info => _info;

        public CommandBase(CommandInfo info)
        {
            _info = info;
        }

        public abstract CommandResult Execute(string[] args, CancellationToken cancellationToken);
    }
}
