using ActivityLoggerBackend.Models;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.OData.Query;
using Microsoft.Azure.Mobile.Security;

namespace ActivityLoggerBackend.Controllers
{
    [AuthorizeLevel(AuthorizationLevel.User)]
    public abstract class ActivityController<TEntity> : BaseController<ActivityDto, TEntity>
        where TEntity : Entity
    {
        [HttpGet]
        public Task<IEnumerable<ActivityDto>> Get(ODataQueryOptions query)
        {
            return QueryAsync(query, qe =>
                qe.AddLink("contact", "regardingobjectid", "contactid").LinkCriteria.AddCondition("ownerid", ConditionOperator.EqualUserId));
        }
    }
}