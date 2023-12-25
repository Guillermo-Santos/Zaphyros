using System.Security;
using System.Text;
using BCrypt.Net;
using Zaphyros.Core.Apps;

namespace Zaphyros.Core.Users
{
    internal sealed class UserLogginService : Service
    {

        public override void Update()
        {
            bool logging = false;
            Console.Write("UserName: ");
            var username = Console.ReadLine();
            Console.Write("Passoword: ");
            ConsoleKeyInfo keyInfo;
            var sb = new StringBuilder();
            do
            {
                keyInfo = Console.ReadKey(true);
                if(keyInfo.Key is >= ConsoleKey.A and <= ConsoleKey.NumPad9)
                {
                    sb.Append(keyInfo.KeyChar);
                }

            }
            while (keyInfo.Key is not ConsoleKey.Enter);
            Console.WriteLine();
            
            var password = sb.ToString();

            try
            {
                var userEntry = UserEntry.GetUserEntries().Where(ue => ue.Name == username).FirstOrDefault();
                if (userEntry is not null && !string.IsNullOrEmpty(password))
                {
                    Console.WriteLine(userEntry.Password);
                    Console.WriteLine(BCrypt.Net.BCrypt.HashPassword(password, userEntry.Password));
                    Console.WriteLine(userEntry.Password == BCrypt.Net.BCrypt.HashPassword(password, userEntry.Password));

                    if (userEntry.NeedReHashing)
                    {
                        var configuration = userEntry.UserType switch
                        {
                            UserType.Admin => PasswordConstants.AdminUser,
                            UserType.Normal => PasswordConstants.NormalUser,
                            _ => throw new ArgumentException($"Invalid User Type {userEntry.UserType}"),
                        };
                        Console.WriteLine(userEntry.Password);
                        // We do not care why the password need rehashing, we just rehash with everything.
                        Console.WriteLine("Update");
                        userEntry.Password = BCrypt.Net.BCrypt.ValidateAndReplacePassword(password, userEntry.Password, (userEntry.HashType != HashType.None), userEntry.HashType, password, true, configuration.HashType, configuration.WorkFactor);
                        userEntry.NeedReHashing = false;
                        userEntry.HashType = configuration.HashType;

                        UserEntry.Save(userEntry);

                        logging = true;
                        Console.WriteLine(userEntry.Password);

                    }
                    else
                    {
                        Console.WriteLine("Normal");
                        logging = BCrypt.Net.BCrypt.EnhancedVerify(password, userEntry.Password, userEntry.HashType);
                    }

                    Console.WriteLine(userEntry);
                }

            }
            catch (BcryptAuthenticationException)
            {
                // We know that the password is wrong, just do nothing
                Sys.Global.Debugger.Send($"{nameof(UserLogginService)} - Throw - {nameof(Update)} - Failed Auth");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            if (logging)
            {
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
            //TODO: Register User Session as a service.
        }
    }
}
