using System.Web.Http;

namespace NAPS.SelfHosted.Controllers
{
    public class PingController : BaseController
    {
        public IHttpActionResult Get()
        {
            return Json("Pong");
        }
    }
}