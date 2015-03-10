using System;
using System.Linq;
using Microsoft.WindowsAzure.Mobile.Service.DynamicsCrm.WebHost.Models;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using Microsoft.Xrm.Client.Services;
using Microsoft.Xrm.Sdk.Client;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.OData.Query;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.WebServiceClient;
using Microsoft.Xrm.Client;
using System.Net;

namespace Microsoft.WindowsAzure.Mobile.Service.DynamicsCrm.WebHost.Controllers
{
    [AuthorizeLevel(AuthorizationLevel.User)]
    public class ContactController : BaseController<ContactDto, Contact>
    {
        public ContactController() : base(true)
        {

        }

        [HttpGet]
        public async Task<IEnumerable<ContactDto>> Get(ODataQueryOptions<ContactDto> query)
        {
            return await QueryAsync(query);
        }
    }
}