namespace Zaphyros.Core.Users
{
    internal class TerminalSession : Session
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private static CommandHandler commandHandler;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public TerminalSession(User user) : base(user)
        {
            commandHandler = new();
        }

        public override void Start()
        {
            var currentDirectory = EnvironmentVariables["HOME"].Value;
            if (!Directory.Exists(currentDirectory))
            {
                Directory.CreateDirectory(currentDirectory);
            }

            Environment.CurrentDirectory = currentDirectory;
        }

        public override void Update()
        {
            // TODO: Make the terminal service so that it will not halt the execution flow of coroutines, 
            //       this to allow SYSTEM and other task to execute "Concurrently"
            // TODO: CommandHandler should become a 'ShellService' that communicates with the session (this to allow remote shells in the future),
            //       future about the Session object and how it interact with other objects is yet to decide.
            Console.Write($"{Environment.CurrentDirectory}> ");
            commandHandler.ExecuteCommand(Console.ReadLine());
        }
    }
}
