﻿using NAPS2.DI.EntryPoints;
using System;

namespace NAPS2.Server
{
    public static class Program
    {
        /// <summary>
        /// The NAPS2.Server.exe main method.
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            // Use reflection to avoid antivirus false positives (yes, really)
            typeof(ServerEntryPoint).GetMethod("Run")?.Invoke(null, new object[] { args });
        }
    }
}
