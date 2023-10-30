namespace Zaphyros.Core.Commands
{
    internal class ClearCommand : CommandBase
    {
        private ClearCommand(string name, string description, CommandType type) : base(new(name, description, type)) { }

        public ClearCommand() : this("clear", "Clear Console Content.", CommandType.Info) { }
        public override CommandResult Execute(string[] args, CancellationToken cancellationToken)
        {
            Console.Clear();
            return CommandResult.Success;
        }
    }
}