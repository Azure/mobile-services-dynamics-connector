using System.Web.Http;
using ActivityLoggerBackend.Models;

namespace ActivityLoggerBackend.Controllers
{
    [Authorize]
    public class AppointmentController : ActivityController<Appointment>
    {
    }
}