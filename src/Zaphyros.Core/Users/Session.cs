using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Cosmos.Core.Memory;
using Zaphyros.Core.Configuration;

namespace Zaphyros.Core.Users
{
    internal struct EnvironmentVariable
    {
        public string Name;
        public string Value;
        public bool SystemManaged;
    }

    internal abstract class Session
    {
        public Guid Id { get; private set; }
        public string Name => User.Name;
        public User User { get; private set; }
        public SessionState State { get; protected set; }
        public string CurrentDirectory { get; set; }

        public Dictionary<string, EnvironmentVariable> EnvironmentVariables { get; private set; }

        public Session(User user)
        {
            User = user;
            Id = Guid.NewGuid(); 
            var home = new EnvironmentVariable()
            {
                Name = "HOME",
                Value = $"0\\Users\\{User.Name}\\",
                SystemManaged = true
            };
            CurrentDirectory = home.Value;
            EnvironmentVariables = new()
            {
                { home.Name, home }
            };

        }

        public virtual void Start() { }
        public abstract void Update();
        public virtual void Stop() { }

    }

    internal class TerminalSession : Session
    {
        public static string Key => "899F7432-3F58-4994-A8D0-F312DAD5319B";
        public string CurrentWorkingDirectory { get; set; } = Environment.CurrentDirectory;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private static CommandHandler commandHandler;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public TerminalSession(User user) : base(user)
        {
            commandHandler = new();
            CurrentDirectory = @"0:\";
        }

        public override void Update()
        {
            Console.Write($"{CurrentDirectory}> ");
            var input = Console.ReadLine();

            Console.WriteLine(input);

            commandHandler.ExecuteCommand(input);
            //Heap.Collect();
        }
    }
}
