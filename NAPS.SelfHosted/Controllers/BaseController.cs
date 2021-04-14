using NAPS.SelfHosted.Attributes;
using NAPS2.DI.Modules;
using Ninject;
using System.Web.Http;

namespace NAPS.SelfHosted.Controllers
{
    [OnlyLocalhost]
    public class BaseController : ApiController
    {
        public StandardKernel Kernel { get; set; }

        public BaseController()
        {
            Kernel = new StandardKernel(new CommonModule());
        }
    }
}
