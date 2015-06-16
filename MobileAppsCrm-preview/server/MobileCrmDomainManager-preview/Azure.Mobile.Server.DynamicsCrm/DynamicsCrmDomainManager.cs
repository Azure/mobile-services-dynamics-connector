using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Security;
using Microsoft.Azure.Mobile.Server.Tables;
using Microsoft.Azure.Mobile.Server.AppService;
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
using Microsoft.Azure.Mobile.Security;

namespace Microsoft.Azure.Mobile.Server.DynamicsCrm
{
    /// <summary>
    /// Provides a <see cref="IDomainManager{TTableData}"/> implementation targeting Dynamics CRM as the backend store.
    /// </summary>
    /// <typeparam name="TTableData">The data object (DTO) type.</typeparam>
    /// <typeparam name="TEntity">The Entity type in Dynamics corresponding to the <typeparamref name="TTableData"/> type.</typeparam>
    public class DynamicsCrmDomainManager<TTableData, TEntity>: DomainManager<TTableData>
        where TEntity: Entity
        where TTableData: class, ITableData
    {
        protected string EntityLogicalName { get; set; }
        protected IEntityMapper<TTableData, TEntity> Map { get; private set; }

        public string CrmUrlSettingsKey { get; set; }
        public string CrmAuthorityUrlSettingsKey { get; set; }
        public string CrmClientSecretSettingsKey { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="DynamicsCrmDomainmanager{TTableData,TEntity}"/>
        /// </summary>
        /// <param name="organizationService">The <see cref="IOrganizationService"/> instance to use when communicating with Dynamics CRM.</param>
        /// <param name="map">The <see cref="IEntityMapper{TTableData,TEntity}"/> instance to use when mapping between <typeparamref name="TTableData"/> and <typeparamref name="TEntity"/> instances.</param>
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
        protected async Task<IOrganizationService> GetOrganizationServiceAsync() 
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
                  
                var creds = await user.GetIdentityAsync<AzureActiveDirectoryCredentials>();

                // create authentication context with no token cache. 
                AuthenticationContext ac = new AuthenticationContext(authorityUrl, validateAuthority: false, tokenCache: null);
                var ar = await ac.AcquireTokenAsync(crmUrl,
                    new ClientCredential(settings["AzureActiveDirectoryClientId"], clientSecret),
                    new UserAssertion(creds.AccessToken));

                var orgService = new OrganizationWebProxyClient(new Uri(crmUrl + servicePath), true);
                orgService.HeaderToken = ar.AccessToken;
                orgService.SdkClientVersion = version;

                _organizationService = orgService;
            }

            return _organizationService;
        }

        public override async Task<bool> DeleteAsync(string id)
        {
            Guid entityId;
            if (!Guid.TryParse(id, out entityId))
                return false;

            var orgService = await GetOrganizationServiceAsync();
            orgService.Delete(EntityLogicalName, entityId);
            return true;
        }

        public override async Task<TTableData> InsertAsync(TTableData data)
        {
            var entity = Map.Map(data);
            var orgService = await GetOrganizationServiceAsync();
            entity.Id = orgService.Create(entity);
            return await LookupAsync(entity.Id);
        }

        public override SingleResult<TTableData> Lookup(string id)
        {
            throw new NotImplementedException();
        }

        protected async Task<TTableData> LookupAsync(Guid id)
        {
            var orgService = await GetOrganizationServiceAsync();

            var result = orgService.Retrieve(EntityLogicalName, id, new ColumnSet(true));
            return Map.Map(result.ToEntity<TEntity>());
        }

        public override async Task<SingleResult<TTableData>> LookupAsync(string id)
        {
            var results = new List<TTableData>();

            Guid entityId;
            if (Guid.TryParse(id, out entityId))
            {
                results.Add(await LookupAsync(entityId));
            }

            return SingleResult.Create(results.AsQueryable());
        }

        public override IQueryable<TTableData> Query()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TTableData>> QueryAsync(ODataQueryOptions query, Action<QueryExpression> queryModifier)
        {
            var builder = new QueryExpressionBuilder<TTableData, TEntity>(this.EntityLogicalName, query, this.Map);
            var crmQuery = builder.GetQueryExpression();

            if(queryModifier != null)
            {
                queryModifier(crmQuery);
            }

            var orgService = await GetOrganizationServiceAsync();
            var entityCollection = orgService.RetrieveMultiple(crmQuery);
            var dataObjects = new List<TTableData>();
            return entityCollection.Entities.Cast<TEntity>().Select(Map.Map);
        }

        public override Task<IEnumerable<TTableData>> QueryAsync(ODataQueryOptions query)
        {
            return QueryAsync(query, null);
        }

        public override async Task<TTableData> ReplaceAsync(string id, TTableData data)
        {
            TEntity entity = Map.Map(data);
            var orgService = await GetOrganizationServiceAsync();
            orgService.Update(entity);
            return await LookupAsync(entity.Id);
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
