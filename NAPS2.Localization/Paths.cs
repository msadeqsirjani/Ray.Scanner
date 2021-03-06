using System;
using System.IO;
using System.Reflection;

namespace NAPS2.Localization
{
    internal class Paths
    {
        private static string _root;

        public static string Root
        {
            get
            {
                if (_root != null) return _root;
                _root = Assembly.GetExecutingAssembly().Location;
                while (Path.GetFileName(_root) != "NAPS2")
                {
                    _root = Path.GetDirectoryName(_root);
                    if (_root != null) continue;
                    Console.WriteLine("Couldn't find NAPS2 folder");
                    Environment.Exit(0);
                }

                return _root;
            }
        }
    }
}
