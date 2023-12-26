using IL2CPU.API.Attribs;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Zaphyros.Core
{
    internal static class SysFiles
    {
        [ManifestResourceStream(ResourceName = "Zaphyros.Core.Resources.Greetings")]
        public static byte[] Greetings;
        [ManifestResourceStream(ResourceName = "Zaphyros.Core.Resources.Framework.System.Private.CoreLib.dll")]
        public static byte[] CorLib;

        //[ManifestResourceStream(ResourceName = "Zaphyros.Core.Resources.Users.users")]
        //public static byte[] usrFile;

        public static readonly string USER_FILE = "0:\\System\\users.sys";
    }
}