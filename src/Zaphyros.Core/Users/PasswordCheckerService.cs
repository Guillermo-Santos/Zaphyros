using System;
using System.Text;
using Zaphyros.Core.Apps;

<<<<<<< Updated upstream:src/Zaphyros.Core/Users/PasswordCheckerService.cs
namespace Zaphyros.Core.Users
=======
#nullable disable
namespace Zaphyros.Core.Users.Services
>>>>>>> Stashed changes:src/Zaphyros.Core/Users/Services/PasswordCheckerService.cs
{
    internal sealed class PasswordCheckerService : Service
    {
        private IEnumerator<UserEntry> entries;

        public override void BeforeStart()
        {
            entries = UserEntry.GetUserEntries().GetEnumerator();
        }
        public override void Update()
        {
            if (!entries.MoveNext())
            {
                Stop();
                return;
            }

            try
            {
                var userEntry = entries.Current;
                if (userEntry.NeedReHashing)
                {
                    Console.WriteLine(userEntry);
                    return;
                }

                PasswordConfiguration configuration = userEntry.UserType switch
                {
                    UserType.Admin => PasswordConstants.AdminUser,
                    UserType.Normal => PasswordConstants.NormalUser,
                    _ => throw new ArgumentException($"Invalid User Type {userEntry.UserType}"),
                };

                userEntry.NeedReHashing = (userEntry.HashType != configuration.HashType) || BCrypt.Net.BCrypt.PasswordNeedsRehash(userEntry.Password, configuration.WorkFactor);
                Console.WriteLine(userEntry);
                UserEntry.Save(userEntry);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public override void AfterStart()
        {
            entries.Dispose();
            _ = Kernel.TaskManager.RegisterService(new UserLogginService());
        }
    }
}
