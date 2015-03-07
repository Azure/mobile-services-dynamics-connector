using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using Microsoft.WindowsAzure.Mobile.Service.Tables;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.WebServiceClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.OData;
using System.Web.Http.OData.Query;

namespace Microsoft.Windows.Azure.Service.DynamicsCrm
{
    public class DynamicsCrmDomainManager<TTableData, TEntity>: DomainManager<TTableData>
        where TEntity: Entity
        where TTableData: class, ITableData
    {
        protected string EntityLogicalName { get; set; }
        public IEntityMapper<TTableData, TEntity> Map { get; private set; }

        public string CrmUrlSettingsKey { get; set; }
        public string CrmAuthorityUrlSettingsKey { get; set; }
        public string CrmClientSecretSettingsKey { get; set; }

        public DynamicsCrmDomainManager(HttpRequestMessage request, ApiServices services, IEntityMapper<TTableData, TEntity> map)
            :base(request, services, false)
        {
            this.Map = map;

            var entityAttribs = typeof(TEntity).GetCustomAttributes(typeof(EntityLogicalNameAttribute), false);
            if (entityAttribs.Length != 1) throw new InvalidOperationException("Could not determine entity logical name from entity type.");
            EntityLogicalName = ((EntityLogicalNameAttribute)entityAttribs[0]).LogicalName;

            CrmUrlSettingsKey = "CrmUrl";
            CrmAuthorityUrlSettingsKey = "CrmAuthorityUrl";
            CrmClientSecretSettingsKey = "CrmClientSecret";
        }

        private IOrganizationService _organizationService;
        protected IOrganizationService OrganizationService 
        { 
            get
            {
                if(_organizationService == null)
                {
                    var settings = this.Services.Settings;

                    string crmUrl = settings[CrmUrlSettingsKey];
                    string servicePath = "/XRMServices/2011/Organization.svc/web";
                    string version = "7.0.0.0";
                    string authorityUrl = settings[CrmAuthorityUrlSettingsKey];
                    string clientSecret = settings[CrmClientSecretSettingsKey];
                    string serviceUri = String.Concat(crmUrl, servicePath, "?SdkClientVersion=", version);

                    var user = this.Request.GetRequestContext().Principal as ServiceUser;

                    var creds = user.GetIdentitiesAsync().Result.OfType<AzureActiveDirectoryCredentials>().FirstOrDefault();
                    AuthenticationContext ac = new AuthenticationContext(authorityUrl, false);
                    var ar = ac.AcquireToken(crmUrl,
                        new ClientCredential(settings.AzureActiveDirectoryClientId, clientSecret),
                        new UserAssertion(creds.AccessToken));

                    var orgService = new OrganizationWebProxyClient(new Uri(crmUrl + servicePath), true);
                    orgService.HeaderToken = ar.AccessToken;
                    orgService.SdkClientVersion = version;

                    _organizationService = orgService;
                }

                return _organizationService;
            }
        }

        public override Task<bool> DeleteAsync(string id)
        {
            Guid entityId;
            if (!Guid.TryParse(id, out entityId))
                return Task.FromResult(false);

            OrganizationService.Delete(EntityLogicalName, entityId);
            return Task.FromResult(true);
        }

        public override Task<TTableData> InsertAsync(TTableData data)
        {
            var entity = Map.MapTo(data);
            entity.Id = OrganizationService.Create(entity);
            return Task.FromResult(Map.MapFrom(entity));
        }

        public override SingleResult<TTableData> Lookup(string id)
        {
            throw new NotImplementedException();
        }

        public override  Task<SingleResult<TTableData>> LookupAsync(string id)
        {
            Guid entityId;
            if (!Guid.TryParse(id, out entityId))
                return Task.FromResult(SingleResult.Create(new List<TTableData>().AsQueryable()));

            var result = OrganizationService.Retrieve(EntityLogicalName, entityId, new ColumnSet(true));
            var mappedResult = Map.MapFrom(result.ToEntity<TEntity>());

            return Task.FromResult(SingleResult.Create(new List<TTableData> { mappedResult }.AsQueryable()));
        }

        public override IQueryable<TTableData> Query()
        {
            throw new NotImplementedException();
        }

        public override Task<IEnumerable<TTableData>> QueryAsync(ODataQueryOptions query)
        {
            var builder = new QueryExpressionBuilder<TTableData, TEntity>(this.EntityLogicalName, query, this.Map);
            var crmQuery = builder.GetQueryExpression();

            var entityCollection = this.OrganizationService.RetrieveMultiple(crmQuery);
            var dataObjects = entityCollection.Entities.Cast<TEntity>().Select(Map.MapFrom);
            return Task.FromResult(dataObjects);
        }

        public override Task<TTableData> ReplaceAsync(string id, TTableData data)
        {
            OrganizationService.Update(Map.MapTo(data));
            return Task.FromResult(data);
        }

        public override Task<TTableData> UpdateAsync(string id, System.Web.Http.OData.Delta<TTableData> patch)
        {
            // doesn't work with JSON, see
            // http://stackoverflow.com/questions/14729249/how-to-use-deltat-from-microsoft-asp-net-web-api-odata-with-code-first-jsonmed
            throw new NotImplementedException();
        }

        public override Task<TTableData> UndeleteAsync(string id, Delta<TTableData> patch)
        {
            throw new NotImplementedException();
        }
    }
}
