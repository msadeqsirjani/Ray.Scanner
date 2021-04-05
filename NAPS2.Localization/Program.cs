using System;

namespace NAPS2.Localization
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                PrintUsage();
            }

            var command = args[0];
            switch (command)
            {
                case "templates":
                    {
                        if (args.Length != 1)
                        {
                            PrintUsage();
                        }

                        Templates.Update();
                        break;
                    }
                case "language":
                    {
                        if (args.Length != 2)
                        {
                            PrintUsage();
                        }

                        Language.Update(args[1]);
                        break;
                    }
                default:
                    PrintUsage();
                    break;
            }
        }

        private static void PrintUsage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("NAPS2.Localization.exe templates");
            Console.WriteLine("NAPS2.Localization.exe language fr");
            Environment.Exit(0);
        }
    }
}
