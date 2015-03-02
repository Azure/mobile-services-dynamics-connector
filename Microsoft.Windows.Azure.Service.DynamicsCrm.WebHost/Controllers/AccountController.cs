using Microsoft.Windows.Azure.Service.DynamicsCrm.WebHost.Models;
using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.Xrm.Client.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.OData;
using System.Web.Http.OData.Query;

namespace Microsoft.Windows.Azure.Service.DynamicsCrm.WebHost.Controllers
{
    //http://www.odata.org/documentation/odata-version-2-0/operations/
    public class AccountController : TableController<AccountDto>
    {
        protected override void Initialize(System.Web.Http.Controllers.HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);

            var orgService = new OrganizationService("CrmConnection");
            DomainManager = new DynamicsCrmDomainManager<AccountDto, Account>(orgService);
        }

        [HttpGet]
        public Task<IEnumerable<AccountDto>> Get(ODataQueryOptions<AccountDto> query)
        {
            return DomainManager.QueryAsync(query);
        }

        [HttpGet]
        public Task<SingleResult<AccountDto>> Get([FromODataUri]String id)
        {
            return DomainManager.LookupAsync(id);
        }

        [HttpDelete]
        public Task<bool> Delete(String id)
        {
            return DomainManager.DeleteAsync(id);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Post(AccountDto data)
        {
            var result = await DomainManager.InsertAsync(data);
            return this.Request.CreateResponse(HttpStatusCode.Created, result);
        }

        [HttpPut]
        public async System.Threading.Tasks.Task Put(AccountDto data)
        {
            await DomainManager.ReplaceAsync(data.Id, data);
        }

        [HttpPatch]
        public async System.Threading.Tasks.Task Patch(AccountDto data)
        {
            await DomainManager.ReplaceAsync(data.Id, data);
        }
    }
}
