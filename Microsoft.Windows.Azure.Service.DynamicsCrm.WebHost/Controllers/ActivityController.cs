using Microsoft.Windows.Azure.Service.DynamicsCrm.WebHost.Models;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.OData.Query;

namespace Microsoft.Windows.Azure.Service.DynamicsCrm.WebHost.Controllers
{
    public abstract class ActivityController<TEntity> : BaseController<ActivityDto, TEntity>
        where TEntity : Entity
    {
        [HttpGet]
        public Task<IEnumerable<ActivityDto>> Get(ODataQueryOptions<ActivityDto> query)
        {
            return QueryAsync(query);
        }
    }
}