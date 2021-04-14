using System.Web.Http;
using Microsoft.Owin.Cors;
using Owin;

namespace NAPS.SelfHosted
{
    public class Startup
    {
        public static void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                "DefaultApi",
                "{controller}/{id}",
                new { id = RouteParameter.Optional }
            );

            app.UseWebApi(config);
            app.UseCors(CorsOptions.AllowAll);
        }
    }
}