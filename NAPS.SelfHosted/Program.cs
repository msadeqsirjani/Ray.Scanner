using NAPS.SelfHosted.Configuration;
using System;
using Topshelf;

namespace NAPS.SelfHosted
{
    public class Program
    {
        public static void Main()
        {
            Run();
        }

        public static void Run()
        {
            HostFactory.Run(options =>
            {
                options.Service<WebServer>(config =>
                {
                    config.ConstructUsing(x => new WebServer());
                    config.WhenStarted(x => x.Start());
                    config.WhenStopped(x => x.Stop());
                });
                options.RunAsLocalSystem();

                options.SetDescription("Rayvarz Scanner Service");
                options.SetDisplayName("Rayvarz Scanner Service");
                options.SetServiceName(Environment.MachineName);
            });
        }
    }
}
