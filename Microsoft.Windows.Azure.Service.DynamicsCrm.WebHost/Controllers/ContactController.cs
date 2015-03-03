using Microsoft.Windows.Azure.Service.DynamicsCrm.WebHost.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.OData.Query;

namespace Microsoft.Windows.Azure.Service.DynamicsCrm.WebHost.Controllers
{
    public class ContactController : DynamicsTableController<ContactDto, Contact>
    {
        [HttpGet]
        public Task<IEnumerable<ContactDto>> Get(ODataQueryOptions<ContactDto> query)
        {
            return QueryAsync(query);
        }
    }
}