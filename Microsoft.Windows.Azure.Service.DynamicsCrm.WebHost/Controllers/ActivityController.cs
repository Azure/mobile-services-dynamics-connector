using Microsoft.Windows.Azure.Service.DynamicsCrm.WebHost.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.OData.Query;

namespace Microsoft.Windows.Azure.Service.DynamicsCrm.WebHost.Controllers
{
    public class ActivityController : BaseController<ActivityDto, ActivityPointer>
    {
        [HttpGet]
        public Task<IEnumerable<ActivityDto>> Get(ODataQueryOptions<ActivityDto> query)
        {
            return QueryAsync(query);
        }

        [HttpPost]
        public Task<ActivityDto> Post(ActivityDto data)
        {
            return InsertAsync(data);
        }
    }
}