using System.Web.Http;
using ActivityLoggerBackend.Models;
using Microsoft.WindowsAzure.Mobile.Service.Security;

namespace ActivityLoggerBackend.Controllers
{
    [AuthorizeLevel(AuthorizationLevel.User)]
    public class AppointmentController : ActivityController<Appointment>
    {
    }
}