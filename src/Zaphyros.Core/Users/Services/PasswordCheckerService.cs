using System;
using System.Text;
using Zaphyros.Core.Apps;

#nullable disable
namespace Zaphyros.Core.Users.Services
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
                    Sys.Global.Debugger.Send(userEntry.ToString());
                    return;
                }

                PasswordConfiguration configuration = userEntry.UserType switch
                {
                    UserType.Admin => PasswordConstants.AdminUser,
                    UserType.Normal => PasswordConstants.NormalUser,
                    _ => throw new ArgumentException($"Invalid User Type {userEntry.UserType}"),
                };

                userEntry.NeedReHashing = (userEntry.HashType != configuration.HashType) || BCrypt.Net.BCrypt.PasswordNeedsRehash(userEntry.Password, configuration.WorkFactor);
                Sys.Global.Debugger.Send(userEntry.ToString());
                userEntry.Save();
            }
            catch (Exception ex)
            {
                Sys.Global.Debugger.Send(ex.Message);
            }
        }

        public override void AfterStart()
        {
            entries.Dispose();
            _ = Kernel.TaskManager.RegisterService(new UserLogginService());
        }
    }
}
