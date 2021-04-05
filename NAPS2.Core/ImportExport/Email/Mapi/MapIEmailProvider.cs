using NAPS2.Config;
using NAPS2.Lang.Resources;
using NAPS2.Logging;
using NAPS2.Platform;
using NAPS2.Util;
using NAPS2.Worker;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NAPS2.ImportExport.Email.Mapi
{
    public class MapIEmailProvider : IEmailProvider
    {
        private readonly IWorkerServiceFactory workerServiceFactory;
        private readonly MapiWrapper mapIWrapper;
        private readonly IErrorOutput errorOutput;
        private readonly IUserConfigManager userConfigManager;

        public MapIEmailProvider(IWorkerServiceFactory workerServiceFactory, MapiWrapper mapIWrapper, IErrorOutput errorOutput, IUserConfigManager userConfigManager)
        {
            this.workerServiceFactory = workerServiceFactory;
            this.mapIWrapper = mapIWrapper;
            this.errorOutput = errorOutput;
            this.userConfigManager = userConfigManager;
        }

        private static bool UseWorker => Environment.Is64BitProcess && PlatformCompat.Runtime.UseWorker;

        /// <summary>
        /// Sends an email described by the given message object.
        /// </summary>
        /// <param name="message">The object describing the email message.</param>
        /// <param name="progressCallback"></param>
        /// <param name="cancelToken"></param>
        /// <returns>Returns true if the message was sent, false if the user aborted.</returns>
        public async Task<bool> SendEmail(EmailMessage message, ProgressHandler progressCallback, CancellationToken cancelToken)
        {
            var configuredClientName = userConfigManager.Config.EmailSetup?.SystemProviderName;

            MapiSendMailReturnCode EmailInProc(string clientName) => mapIWrapper.SendEmail(clientName, message);
            MapiSendMailReturnCode EmailByWorker(string clientName)
            {
                using (var worker = workerServiceFactory.Create())
                {
                    return worker.Service.SendMapiEmail(clientName, message);
                }
            }

            return await Task.Factory.StartNew(() =>
            {
                // It's difficult to get 32/64 bit right when using mapi32.dll:
                // https://docs.microsoft.com/en-us/office/client-developer/outlook/mapi/building-mapi-applications-on-32-bit-and-64-bit-platforms
                // Also some people have had issues with bad DLL paths (?), so we can fall back to mapi32.dll.

                var emailFunctions = new List<Func<MapiSendMailReturnCode>>();
                if (configuredClientName != null)
                {
                    if (mapIWrapper.CanLoadClient(configuredClientName))
                    {
                        emailFunctions.Add(() => EmailInProc(configuredClientName));
                    }
                    if (UseWorker)
                    {
                        emailFunctions.Add(() => EmailByWorker(configuredClientName));
                    }
                }
                if (mapIWrapper.CanLoadClient(null))
                {
                    emailFunctions.Add(() => EmailInProc(null));
                }
                if (UseWorker)
                {
                    emailFunctions.Add(() => EmailByWorker(null));
                }

                var returnCode = MapiSendMailReturnCode.Failure;
                foreach (var func in emailFunctions)
                {
                    returnCode = func();
                    if (returnCode != MapiSendMailReturnCode.Failure)
                    {
                        break;
                    }
                }

                // Process the result
                if (returnCode == MapiSendMailReturnCode.UserAbort)
                {
                    return false;
                }

                if (returnCode == MapiSendMailReturnCode.Success) return true;
                Log.Error("Error sending email. MAPI error code: {0}", returnCode);
                errorOutput.DisplayError(MiscResources.EmailError, $"MAPI returned error code: {returnCode}");
                return false;

            });
        }
    }
}
