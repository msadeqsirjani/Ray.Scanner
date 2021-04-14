using NAPS.SelfHosted.Attributes;
using NAPS2.Config;
using NAPS2.Scan;
using System.Collections.Generic;
using System.Web.Http;

namespace NAPS.SelfHosted.Controllers
{
    [OnlyLocalhost]
    public class ScannerController : ApiController
    {
        private IScanDriverFactory ScanDriverFactory;


    }
}