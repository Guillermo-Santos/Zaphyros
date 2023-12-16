using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Zaphyros.Core.Users;

namespace Zaphyros.Core.Configuration
{

    public static class UserConfiguration
    {

        public static (Dictionary<string, string>, List<PermissionEntry>) InterpretFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException();
            }

            var environmentVariables = new Dictionary<string, string>();
            var permissions = new List<PermissionEntry>();

            string[] lines = File.ReadAllLines(filePath);

            string? currentSection = null;
            foreach (string line in lines)
            {
                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    currentSection = line.Trim('[', ']');
                }
                else if (!string.IsNullOrWhiteSpace(line))
                {
                    switch (currentSection)
                    {
                        case "ENV":
                            var (name, value) = ParseEnvironmentVariable(line);
                            environmentVariables.Add(name, value);
                            break;
                        case "PERMISIONS":
                            permissions.Add(ParsePermissionEntry(line));
                            break;
                    }
                }
            }

            return (environmentVariables, permissions);
        }

        private static (string name, string value) ParseEnvironmentVariable(string line)
        {
            var i = line.IndexOf('=');

            string variableName = line.Substring(0, i);
            string variableValue = line[(i + 1)..].Trim('\"');
            return (variableName, variableValue);
        }

        private static PermissionEntry ParsePermissionEntry(string line)
        {
            string levelPrefix = "Level=";
            string pathPrefix = "Path=";

            var parts = line.Split(" ");

            var level = int.Parse(parts[0][levelPrefix.Length..]);
            var path = parts[1].Substring(pathPrefix.Length + 1, parts[1].Length - 1);

            return new PermissionEntry(path, level);
        }
    }
}
