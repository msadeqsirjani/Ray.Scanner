using NAPS2.DI.EntryPoints;
using System;

namespace NAPS2.Console
{
    public static class Program
    {
        /// <summary>
        /// The NAPS2.Console.exe main method.
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            // Use reflection to avoid antivirus false positives (yes, really)
            typeof(ConsoleEntryPoint).GetMethod("Run")?.Invoke(null, new object[] { args });
        }
    }
}
