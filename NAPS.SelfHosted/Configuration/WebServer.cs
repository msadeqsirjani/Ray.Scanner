using Microsoft.Owin.Hosting;
using System;

namespace NAPS.SelfHosted.Configuration
{
    public class WebServer
    {
        private IDisposable webapp;
        private const string URL = "http://localhost:9000";

        public void Start()
        {
            webapp = WebApp.Start<Startup>(URL);
        }

        public void Stop()
        {
            webapp?.Dispose();
        }
    }
}
