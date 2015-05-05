using System;
using System.Linq;
using DynamicsConnector.Models;
using Microsoft.Azure.Mobile.Server.Security;
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
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Azure.Mobile.Security;

namespace DynamicsConnector.Controllers
{
    //[AuthorizeLevel(AuthorizationLevel.User)]
    public class ContactController : BaseController<ContactDto, Contact>
    {
        public ContactController() : base(true)
        {

        }

        
        public async Task<IEnumerable<ContactDto>> GetAllContact(ODataQueryOptions<ContactDto> query)
        {
            return await QueryAsync(query, qe => qe.Criteria.AddCondition("ownerid", ConditionOperator.EqualUserId));
        }


        public async System.Threading.Tasks.Task<IHttpActionResult> Post(ContactDto data)
        {
            ContactDto item = await InsertAsync(data);

            return CreatedAtRoute("Tables", new { id = item.Id }, item);

        }
    }
}