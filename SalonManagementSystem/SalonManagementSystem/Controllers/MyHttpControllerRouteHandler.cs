using System.Web;
using System.Web.Http.WebHost;
using System.Web.Routing;

namespace SalonManagementSystem.Controllers
{

    #pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class MyHttpControllerRouteHandler : HttpControllerRouteHandler
    {
        protected override IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new MyHttpControllerHandler(requestContext.RouteData);
        }
    }
}