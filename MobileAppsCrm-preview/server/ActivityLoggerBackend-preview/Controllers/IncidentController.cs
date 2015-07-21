using System;
using System.Linq;
using ActivityLoggerBackend.Models;
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

namespace ActivityLoggerBackend.Controllers
{
    [Authorize]
    public class IncidentController : BaseController<IncidentDto, Incident>
    {
        public IncidentController()
            : base(true)
        {

        }

        [HttpGet]
        public async Task<IEnumerable<IncidentDto>> Get(ODataQueryOptions query)
        {
            var tracer = Configuration.Services.GetTraceWriter();
            tracer.Info("inside the query\n");
            tracer.Info("Query is: " + this.Request.RequestUri.ToString());

            IEnumerable<IncidentDto> incidents = await this.QueryAsync(query);
            foreach (var incident in incidents)
            {
                tracer.Info("Incident " + incident.Id + ", '" + incident.Text + "', completed: " + incident.Complete.ToString());
            }

            return incidents; 
        }
    }
}