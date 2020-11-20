using System.Web.Http.WebHost;
using System.Web.Routing;
using System.Web.SessionState;

namespace SalonManagementSystem.Controllers
{

    #pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class MyHttpControllerHandler: HttpControllerHandler, IRequiresSessionState
    {
        public MyHttpControllerHandler(RouteData routeData) : base(routeData) { }
    }
}