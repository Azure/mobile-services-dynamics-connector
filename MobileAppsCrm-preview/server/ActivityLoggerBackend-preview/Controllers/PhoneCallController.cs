using System.Web.Http;
using ActivityLoggerBackend.Models;
using Microsoft.Azure.Mobile.Security;

namespace ActivityLoggerBackend.Controllers
{
    [AuthorizeLevel(AuthorizationLevel.User)]
    public class PhoneCallController : ActivityController<PhoneCall>
    {
    }
}