using NAPS2.DI.EntryPoints;
using System;

namespace NAPS2.Worker
{
    public static class Program
    {
        /// <summary>
        /// The NAPS2.Worker.exe main method.
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            // Use reflection to avoid antivirus false positives (yes, really)
            typeof(WorkerEntryPoint).GetMethod("Run")?.Invoke(null, new object[] { args });
        }
    }
}
