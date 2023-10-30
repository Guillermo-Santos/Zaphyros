using libDotNetClr;
using LibDotNetParser.CILApi;

namespace Zaphyros.Core.Apps
{
    internal class DotNetApp : App
    {
        public readonly DotNetClr Clr;

        public DotNetApp(DotNetFile dotNetFile)
        {
            Clr = new DotNetClr(dotNetFile);
            Clr.RegisterResolveCallBack(AssemblyCallback);
        }

        public DotNetApp(byte[] dllFile) : this(new DotNetFile(dllFile)) { }

        public DotNetApp(string filePath) : this(new DotNetFile(filePath)) { }

        public virtual void Run(string[] args) => Clr.Start(args);
        public virtual void Run() => Clr.Start();


        private static byte[]? AssemblyCallback(string dll)
        {
            
            Console.WriteLine(dll);
            return dll switch
            {
                "System.Private.CoreLib" => File.ReadAllBytes("0:\\System\\System.Private.CoreLib.dll"),
                _ => null
            };
        }
    }
}
