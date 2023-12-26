namespace Zaphyros.Core.Users
{
    internal class TerminalSession : Session
    {
        public static string Key => "899F7432-3F58-4994-A8D0-F312DAD5319B";
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private static CommandHandler commandHandler;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public TerminalSession(User user) : base(user)
        {
            commandHandler = new();
            CurrentDirectory = EnvironmentVariables["HOME"].Value;
        }

        public override void Start()
        {
            if (!Directory.Exists(CurrentDirectory))
            {
                Directory.CreateDirectory(CurrentDirectory);
            }
        }

        public override void Update()
        {
            // TODO: Make the terminal service so that it will not halt the execution flow of coroutines, 
            //       this to allow SYSTEM and other task to execute "Concurrently"
            Console.Write($"{CurrentDirectory}> ");
            commandHandler.ExecuteCommand(Console.ReadLine());
        }
    }
}
