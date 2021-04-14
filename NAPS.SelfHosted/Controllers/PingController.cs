using NAPS.SelfHosted.Attributes;
using System.Web.Http;

namespace NAPS.SelfHosted.Controllers
{
    [OnlyLocalhost]
    public class PingController : ApiController
    {
        public IHttpActionResult Get()
        {
            return Json("Pong");
        }
    }
}