using DynamicsCrmConnectorApiApp.Models;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.OData.Query;

namespace DynamicsCrmConnectorApiApp.Controllers
{
    public class TaskController : ActivityController<Task>
    {
        [HttpPost]
        public System.Threading.Tasks.Task<ActivityDto> Post(ActivityDto data)
        {
            return InsertAsync(data);
        }
    }
}