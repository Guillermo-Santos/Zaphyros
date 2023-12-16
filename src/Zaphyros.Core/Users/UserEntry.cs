using System.Text;
using BCrypt.Net;

namespace Zaphyros.Core.Users
{
    public sealed class UserEntry
    {
        private const string Separator = "::";
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

            return new()
            {
                Name = properties[0],
                UserType = (UserType)int.Parse(properties[1]),
                HashType = (HashType)int.Parse(properties[2]),
                NeedReHashing = (properties[3] == "1"),
                Password = properties[4],
            };
        }

        public static IEnumerable<UserEntry> GetUserEntries()
        {
            var reader = new StreamReader(File.OpenRead(@"0:\System\users"), encoding: Encoding.ASCII);
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
    }
}
