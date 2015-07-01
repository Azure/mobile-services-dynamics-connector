using System;
using System.Linq;
using ActivityLoggerBackend.Models;
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
using Microsoft.Crm.Sdk.Messages;
using System.Web.Http.OData;

namespace ActivityLoggerBackend.Controllers
{
    [AuthorizeLevel(AuthorizationLevel.User)]
    public class IncidentController : BaseController<IncidentDto, Incident>
    {
        public IncidentController()
            : base(true)
        {

        }

        [HttpPost]
        public System.Threading.Tasks.Task<IncidentDto> Post(IncidentDto data)
        {
            return InsertAsync(data);
        }
        
        [HttpGet]
        public async Task<IEnumerable<IncidentDto>> Get(ODataQueryOptions query)
        {
            this.Services.Log.Info("inside the query\n");
            this.Services.Log.Info("Query is: " + this.Request.RequestUri.ToString());

            IEnumerable<IncidentDto> incidents = await this.QueryAsync(query);

            foreach (var incident in incidents)
            {
                this.Services.Log.Info("IncidentID: " + incident.Id + ", '" + incident.Text + "', completed: " + incident.Complete);
            }

            return incidents; 
        }

#if false
        public async Task<IncidentDto> PatchTodoItem(string id, Delta<IncidentDto> patch)
        {
            /*var current = (await this.LookupAsync(id)).Queryable.First();
            this.Services.Log.Info("Current item: " + current.Id + ",  '" + current.Text + "', completed: " + current.Complete.ToString());
            patch.Patch(current);
            this.Services.Log.Info("Patch simulation: " + current.Id + ",  '" + current.Text + "', completed: " + current.Complete.ToString());*/

            var incidentDto = (await this.LookupAsync(id)).Queryable.First();

            patch.Patch(incidentDto);

            // Received request to close the Incident
            if (patch.GetChangedPropertyNames().Contains("Complete") && incidentDto.Complete)
            {

            }

            if (patch.GetChangedPropertyNames().Contains("Complete") == true)
            {
                this.Services.Log.Info("resolving the case");
                Guid entityId;
                if (!Guid.TryParse(id, out entityId))
                    return null;

                //case resolution requires special handling
                var incidentResolution = new IncidentResolution()
                {
                    Subject = "Resolved Incident",
                    IncidentId = new EntityReference(Incident.EntityLogicalName, entityId)
                };

                var closeIncidentRequest = new CloseIncidentRequest()
                {
                    IncidentResolution = incidentResolution,
                    Status = new OptionSetValue(5)
                };

                await this.DynamicsCrmDomainManager.Execute(closeIncidentRequest);
                return (await this.LookupAsync(id)).Queryable.First();
            }
            else
            {

                var item = await this.UpdateAsync(id, patch);
                this.Services.Log.Info("Updated item: " + item.Id + ",  '" + item.Text + "', completed: " + item.complete.ToString());

                /*Incident incident = this.EntityMapper.Map(item);
                this.Services.Log.Info("Updated incident: " + incident.Id.ToString() + ",  '" + incident.Title + "', completed: " + incident.StatusCode.Value.ToString());*/
                return item;
            }
        }
#endif 
    }
}