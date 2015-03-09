using System.Web.Http;
using System.Web.Routing;

namespace Microsoft.WindowsAzure.Mobile.Service.DynamicsCrm.WebHost
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            WebApiConfig.Register();
        }
    }
}