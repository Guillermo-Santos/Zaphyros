using System.Text;
using BCrypt.Net;
using Cosmos.Core;

namespace Zaphyros.Core.Users
{
    public sealed class UserEntry
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public UserType UserType { get; set; }
        public HashType HashType { get; set; }
        public bool NeedReHashing { get; set; }

        public override string ToString()
        {
            return string.Concat(Name, Separator, (int)UserType, Separator, (int)HashType, Separator, NeedReHashing ? "1" : "0", Separator, Password);
        }

        public static UserEntry Parse(string line)
        {
            var properties = line.Split(Separator);

            if (properties.Length == 0 || properties.Length > 5)
            {
                throw new ArgumentOutOfRangeException(nameof(line));
            }

            var userEntry = new UserEntry()
            {
                Name = properties[0],
                UserType = (UserType)int.Parse(properties[1]),
                HashType = (HashType)int.Parse(properties[2]),
                NeedReHashing = (properties[3] == "1"),
                Password = properties[4],
            };

            Sys.Global.Debugger.Send(userEntry.ToString());

            return userEntry;
        }

        public static void Save(UserEntry userEntry)
        {
            var lines = new List<string>();

            var text = File.ReadAllText(SysFiles.USER_FILE);

            lines.AddRange(text.Split(Environment.NewLine));

            var index = lines.FindIndex(line => line.StartsWith(userEntry.Name));
            if (index > -1)
            {
                lines[index] = userEntry.ToString();
            }
            else
            {
                lines.Add(userEntry.ToString());
            }

            using var writer = new StreamWriter(File.OpenWrite(SysFiles.USER_FILE), encoding: Encoding.ASCII);
            foreach (var line in lines)
            {
                writer.WriteLine(line);
            }
        }

        public static IEnumerable<UserEntry> GetUserEntries()
        {
            var reader = new StreamReader(File.OpenRead(SysFiles.USER_FILE), encoding: Encoding.ASCII);
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();

                if (string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line) || line.StartsWith(";"))
                {
                    continue;
                }

                yield return Parse(line);
            }
        }

        private static readonly string Separator = "::";
    }
}
