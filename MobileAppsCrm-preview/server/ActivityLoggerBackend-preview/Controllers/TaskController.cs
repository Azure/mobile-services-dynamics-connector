using ActivityLoggerBackend.Models;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.OData.Query;
using Microsoft.Azure.Mobile.Security;

namespace ActivityLoggerBackend.Controllers
{
    [AuthorizeLevel(AuthorizationLevel.User)]
    public class TaskController : ActivityController<Task>
    {
        [HttpPost]
        public System.Threading.Tasks.Task<ActivityDto> Post(ActivityDto data)
        {
            return InsertAsync(data);
        }
    }
}