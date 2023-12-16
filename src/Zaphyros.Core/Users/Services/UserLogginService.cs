using System.Security;
using System.Text;
using BCrypt.Net;
using Zaphyros.Core.Apps;

namespace Zaphyros.Core.Users.Services
{
    internal sealed class UserLogginService : Service
    {
        private bool logging = false;
        private UserEntry? userEntry;

        public override void Update()
        {
            Console.Write("UserName: ");
            var username = Console.ReadLine();
            Console.Write("Passoword: ");
            ConsoleKeyInfo keyInfo;
            var sb = new StringBuilder();
            do
            {
                keyInfo = Console.ReadKey(true);
                if (keyInfo.Key is >= ConsoleKey.A and <= ConsoleKey.NumPad9)
                {
                    sb.Append(keyInfo.KeyChar);
                }

            }
            while (keyInfo.Key is not ConsoleKey.Enter);
            Console.WriteLine();

            var password = sb.ToString();

            try
            {
                userEntry = UserEntry.GetUserEntries().Where(ue => ue.Name == username).FirstOrDefault();
                if (userEntry is not null && !string.IsNullOrEmpty(password))
                {

                    if (userEntry.NeedReHashing)
                    {
                        var configuration = userEntry.UserType switch
                        {
                            UserType.Admin => PasswordConstants.AdminUser,
                            UserType.Normal => PasswordConstants.NormalUser,
                            _ => throw new ArgumentException($"Invalid User Type {userEntry.UserType}"),
                        };
                        // We do not care why the password need rehashing, we just rehash with everything.
                        userEntry.Password = BCrypt.Net.BCrypt.ValidateAndReplacePassword(password, userEntry.Password, true, userEntry.HashType, password, true, configuration.HashType, configuration.WorkFactor, true);
                        userEntry.NeedReHashing = false;
                        userEntry.HashType = configuration.HashType;
                        logging = true;
                    }
                    else
                    {
                        logging = BCrypt.Net.BCrypt.EnhancedVerify(password, userEntry.Password, userEntry.HashType);
                    }
                }

            }
            catch (BcryptAuthenticationException)
            {
                // We know that the password is wrong, just do nothing
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            if (logging)
            {
                Console.WriteLine("Welcome {0}", username);
                Stop();
            }
            else
            {
                Console.WriteLine("Invalid User or Password");
            }
        }

        public override void AfterStart()
        {
            if (logging)
            {
                var SMS = new SessionManagerService();
                var session = new TerminalSession(new User(userEntry!.Name, userEntry.UserType));
                Kernel.Session = session;
                SMS.RegisterSession(session);
                Kernel.TaskManager.RegisterService(SMS);
            }
        }
    }
}
