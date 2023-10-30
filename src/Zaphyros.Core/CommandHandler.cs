using Zaphyros.Core.Commands.System;
using System;
using System.Collections.Generic;
using System.Data;
using Zaphyros.Core.Commands;

namespace Zaphyros.Core
{

    public partial class CommandHandler
    {
        public static Dictionary<string, ICommand> Commands;

        public CommandHandler()
        {
            Commands = new();
            RegisterDefaultCommands();
        }

        public static void RegisterCommand(ICommand command)
        {
            if (Commands.ContainsKey(command.Info.Name))
            {
                throw new ArgumentException($"Command {command.Info.Name} already exists");
            }

            Commands.Add(command.Info.Name, command);
        }



        public void ExecuteCommand(string? commandText)
        {
            if (string.IsNullOrWhiteSpace(commandText)) return;

            List<string> commandParts = CommandHelper.ParseCommandLine(commandText);

            if (commandParts.Count == 0) return;

            var commandName = commandParts[0];
            commandParts.RemoveAt(0);

            if (Commands.TryGetValue(commandName, out var command))
            {
                try
                {
                    var cToken = new CancellationToken();
                    var result = command.Execute(commandParts.ToArray(), cToken);

                    switch (result)
                    {
                        case CommandResult.Success:
                            break;
                        case CommandResult.Failure:
                            Console.WriteLine("Fail");
                            break;
                        case CommandResult.Unknown:
                            Console.WriteLine("Unknown");
                            break;
                    }
                }
                catch (OperationCanceledException)
                {
                }
            }
        }

        private partial void RegisterDefaultCommands();
    }
}