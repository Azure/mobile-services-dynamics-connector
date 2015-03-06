using Microsoft.Windows.Azure.Service.DynamicsCrm.WebHost.Models;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.OData.Query;

namespace Microsoft.Windows.Azure.Service.DynamicsCrm.WebHost.Controllers
{
    [AuthorizeLevel(AuthorizationLevel.User)]
    public class ContactController : BaseController<ContactDto, Contact>
    {
        [HttpGet]
        public Task<IEnumerable<ContactDto>> Get(ODataQueryOptions<ContactDto> query)
        {
            var currentUser = User as ServiceUser;
            return QueryAsync(query);
        }
    }
}