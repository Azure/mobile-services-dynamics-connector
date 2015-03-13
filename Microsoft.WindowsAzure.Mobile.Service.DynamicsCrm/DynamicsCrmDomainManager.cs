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

namespace Microsoft.WindowsAzure.Mobile.Service.DynamicsCrm
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
            var entity = Map.Map(data);
            entity.Id = OrganizationService.Create(entity);
            return Task.FromResult(Lookup(entity.Id));
        }

        public override SingleResult<TTableData> Lookup(string id)
        {
            throw new NotImplementedException();
        }

        protected TTableData Lookup(Guid id)
        {
            var result = OrganizationService.Retrieve(EntityLogicalName, id, new ColumnSet(true));
            return Map.Map(result.ToEntity<TEntity>());
        }

        public override  Task<SingleResult<TTableData>> LookupAsync(string id)
        {
            Guid entityId;
            if (!Guid.TryParse(id, out entityId))
                return Task.FromResult(SingleResult.Create(new List<TTableData>().AsQueryable()));

            return Task.FromResult(SingleResult.Create(new List<TTableData> { Lookup(entityId) }.AsQueryable()));
        }

        public override IQueryable<TTableData> Query()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TTableData>> QueryAsync(ODataQueryOptions query, Action<QueryExpression> queryModifier)
        {
            var builder = new QueryExpressionBuilder<TTableData, TEntity>(this.EntityLogicalName, query, this.Map);
            var crmQuery = builder.GetQueryExpression();

            if(queryModifier != null)
            {
                queryModifier(crmQuery);
            }

            var entityCollection = this.OrganizationService.RetrieveMultiple(crmQuery);
            var dataObjects = new List<TTableData>();
            return Task.FromResult(entityCollection.Entities.Cast<TEntity>().Select(Map.Map));
        }

        public override Task<IEnumerable<TTableData>> QueryAsync(ODataQueryOptions query)
        {
            return QueryAsync(query, null);
        }

        public override Task<TTableData> ReplaceAsync(string id, TTableData data)
        {
            TEntity entity = Map.Map(data);
            OrganizationService.Update(entity);
            return Task.FromResult(Lookup(entity.Id));
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
