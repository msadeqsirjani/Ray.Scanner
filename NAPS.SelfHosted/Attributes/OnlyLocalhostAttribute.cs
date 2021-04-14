using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace NAPS.SelfHosted.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]

    public class OnlyLocalhostAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var isLocal = actionContext.RequestContext.IsLocal;

            if (!isLocal)
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden);
                return;
            }

            base.OnAuthorization(actionContext);
        }
    }
}