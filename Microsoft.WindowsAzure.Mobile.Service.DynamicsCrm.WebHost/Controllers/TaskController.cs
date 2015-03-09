using Microsoft.WindowsAzure.Mobile.Service.DynamicsCrm.WebHost.Models;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.OData.Query;

namespace Microsoft.WindowsAzure.Mobile.Service.DynamicsCrm.WebHost.Controllers
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