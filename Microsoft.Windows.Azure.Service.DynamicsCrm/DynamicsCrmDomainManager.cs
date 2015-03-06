using Microsoft.WindowsAzure.Mobile.Service.Tables;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.OData.Query;

namespace Microsoft.Windows.Azure.Service.DynamicsCrm
{
    /// <summary>
    /// Provides a <see cref="IDomainManager{TTableData}"/> implementation targeting Dynamics CRM as the backend store.
    /// </summary>
    /// <typeparam name="TTableData">The data object (DTO) type.</typeparam>
    /// <typeparam name="TEntity">The Entity type in Dynamics corresponding to the <typeparamref name="TTableData"/> type.</typeparam>
    public class DynamicsCrmDomainManager<TTableData, TEntity>: IDomainManager<TTableData>
        where TEntity: Entity
        where TTableData: class, ITableData
    {
        protected IOrganizationService OrganizationService { get; set; }
        protected string EntityLogicalName { get; set; }
        protected IEntityMapper<TTableData, TEntity> Map { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="DynamicsCrmDomainmanager{TTableData,TEntity}"/>
        /// </summary>
        /// <param name="organizationService">The <see cref="IOrganizationService"/> instance to use when communicating with Dynamics CRM.</param>
        /// <param name="map">The <see cref="IEntityMapper{TTableData,TEntity}"/> instance to use when mapping between <typeparamref name="TTableData"/> and <typeparamref name="TEntity"/> instances.</param>
        public DynamicsCrmDomainManager(IOrganizationService organizationService, IEntityMapper<TTableData, TEntity> map)
        {
            this.Map = map;

            OrganizationService = organizationService;
            var entityAttribs = typeof(TEntity).GetCustomAttributes(typeof(EntityLogicalNameAttribute), false);
            if (entityAttribs.Length != 1) throw new InvalidOperationException("Could not determine entity logical name from entity type.");
            EntityLogicalName = ((EntityLogicalNameAttribute)entityAttribs[0]).LogicalName;
        }

        public Task<bool> DeleteAsync(string id)
        {
            Guid entityId;
            if (!Guid.TryParse(id, out entityId))
                return Task.FromResult(false);

            OrganizationService.Delete(EntityLogicalName, entityId);
            return Task.FromResult(true);
        }

        public Task<TTableData> InsertAsync(TTableData data)
        {
            var entity = Map.Map(data);
            entity.Id = OrganizationService.Create(entity);
            return Task.FromResult(Map.Map(entity));
        }

        public System.Web.Http.SingleResult<TTableData> Lookup(string id)
        {
            throw new NotImplementedException();
        }

        public Task<System.Web.Http.SingleResult<TTableData>> LookupAsync(string id)
        {
            Guid entityId;
            if (!Guid.TryParse(id, out entityId))
                return Task.FromResult(SingleResult.Create(new List<TTableData>().AsQueryable()));

            var result = OrganizationService.Retrieve(EntityLogicalName, entityId, new ColumnSet(true));
            var mappedResult = Map.Map(result.ToEntity<TEntity>());

            return Task.FromResult(SingleResult.Create(new List<TTableData> { mappedResult }.AsQueryable()));
        }

        public IQueryable<TTableData> Query()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TTableData>> QueryAsync(ODataQueryOptions query)
        {
            var builder = new QueryExpressionBuilder<TTableData, TEntity>(this.EntityLogicalName, query, this.Map);
            var crmQuery = builder.GetQueryExpression();

            var entityCollection = this.OrganizationService.RetrieveMultiple(crmQuery);
            var dataObjects = entityCollection.Entities.Cast<TEntity>().Select(Map.Map);
            return Task.FromResult(dataObjects);
        }

        public Task<TTableData> ReplaceAsync(string id, TTableData data)
        {
            OrganizationService.Update(Map.Map(data));
            return Task.FromResult(data);
        }

        public Task<TTableData> UpdateAsync(string id, System.Web.Http.OData.Delta<TTableData> patch)
        {
            // doesn't work with JSON, see
            // http://stackoverflow.com/questions/14729249/how-to-use-deltat-from-microsoft-asp-net-web-api-odata-with-code-first-jsonmed
            throw new NotImplementedException();
        }
    }
}
