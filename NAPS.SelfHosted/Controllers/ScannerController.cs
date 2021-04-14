using NAPS2.Config;
using NAPS2.Scan;
using NAPS2.Scan.Twain;
using Ninject;
using System.Collections.Generic;
using System.Web.Http;

namespace NAPS.SelfHosted.Controllers
{
    public class ScannerController : BaseController
    {
        private readonly IProfileManager profileManager;
        private readonly TwainScanDriver twainScanDriver;

        public ScannerController()
        {
            profileManager = Kernel.Get<ProfileManager>();
            twainScanDriver = Kernel.Get<TwainScanDriver>();
        }

        [HttpGet]

        public List<ScanDevice> GetProfiles()
        {
            return twainScanDriver.GetDeviceList();
        }
    }
}