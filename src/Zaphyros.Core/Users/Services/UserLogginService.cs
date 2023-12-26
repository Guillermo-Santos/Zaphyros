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
                    //Console.WriteLine(BCrypt.Net.BCrypt.HashPassword(password, userEntry.Password));
                    //Console.WriteLine(userEntry.Password);
                    //Console.WriteLine(BCrypt.Net.BCrypt.Verify(password, userEntry.Password));
                    if (userEntry.NeedReHashing)
                    {
                        var configuration = userEntry.UserType switch
                        {
                            UserType.Admin => PasswordConstants.AdminUser,
                            UserType.Normal => PasswordConstants.NormalUser,
                            _ => throw new ArgumentException($"Invalid User Type {userEntry.UserType}"),
                        };
                        // We do not care why the password need rehashing, we just rehash with everything.
                        userEntry.Password = BCrypt.Net.BCrypt.ValidateAndReplacePassword(password, userEntry.Password, (userEntry.HashType != HashType.None), userEntry.HashType, password, true, configuration.HashType, configuration.WorkFactor, true);
                        userEntry.NeedReHashing = false;
                        userEntry.HashType = configuration.HashType;
                        userEntry.Save();
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
                Sys.Global.Debugger.Send(ex.ToString());
            }

            if (logging)
            {
                Console.Clear();
                Console.WriteLine($"Welcome {username}");
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
                var session = new TerminalSession(new(userEntry!.Name, userEntry!.UserType));
                Kernel.Session = session;
                SessionManagerService.Instance.RegisterSession(session);
                //Kernel.TaskManager.RegisterService(SMS);
            }
        }
    }
}
