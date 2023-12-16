using System;
using System.Text;
using Zaphyros.Core.Apps;

namespace Zaphyros.Core.Users.Services
{
    internal sealed class PasswordCheckerService : Service
    {
        private List<string> lines;
        private int index;
        public override void BeforeStart()
        {
            index = 0;
            lines = new List<string>();

            var text = File.ReadAllText(@"0:\System\users");

            lines.AddRange(text.Split(Environment.NewLine));
        }
        public override void Update()
        {
            if (index == lines.Count)
            {
                Stop();
                return;
            }
            try
            {
                var line = lines[index];
                if (string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line) || line.StartsWith(";"))
                {
                    index++;
                    return;
                }

                var userEntry = UserEntry.Parse(line);
                PasswordConfiguration configuration = userEntry.UserType switch
                {
                    UserType.Admin => PasswordConstants.AdminUser,
                    UserType.Normal => PasswordConstants.NormalUser,
                    _ => throw new ArgumentException($"Invalid User Type {userEntry.UserType}"),
                };

                if (userEntry.HashType != configuration.HashType)
                {
                    userEntry.NeedReHashing = true;
                    lines[index] = userEntry.ToString();
                    index++;
                    if (index == lines.Count)
                    {
                        Stop();
                    }
                    Console.WriteLine(userEntry);
                    return;
                }

                if (BCrypt.Net.BCrypt.PasswordNeedsRehash(userEntry.Password, configuration.WorkFactor))
                {
                    userEntry.NeedReHashing = true;
                    lines[index] = userEntry.ToString();
                }
                Console.WriteLine(userEntry);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            index++;
        }

        public override void AfterStart()
        {
            using (var writer = new StreamWriter(File.OpenWrite(@"0:\System\users"), encoding: Encoding.ASCII))
            {
                foreach (var line in lines)
                {
                    writer.WriteLine(line);
                }
            }

            Kernel.TaskManager.RegisterService(new UserLogginService());
        }
    }
}
