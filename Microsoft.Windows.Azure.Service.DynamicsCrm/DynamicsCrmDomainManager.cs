using AutoMapper;
using Microsoft.WindowsAzure.Mobile.Service.Tables;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.OData.Query;

namespace Microsoft.Windows.Azure.Service.DynamicsCrm
{
    public class DynamicsCrmDomainManager<TTableData, TEntity>: IDomainManager<TTableData>
        where TEntity: Entity
        where TTableData: class, ITableData
    {
        protected IOrganizationService OrganizationService { get; set; }
        protected string EntityLogicalName { get; set; }
        
        public DynamicsCrmDomainManager(IOrganizationService organizationService)
        {
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
            var entity = Mapper.Map<TTableData, TEntity>(data);
            entity.Id = OrganizationService.Create(entity);
            return Task.FromResult(Mapper.Map<TEntity, TTableData>(entity));
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
            var mappedResult = Mapper.Map<TEntity, TTableData>(result.ToEntity<TEntity>());

            return Task.FromResult(SingleResult.Create(new List<TTableData> { mappedResult }.AsQueryable()));
        }

        public IQueryable<TTableData> Query()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TTableData>> QueryAsync(ODataQueryOptions query)
        {
            var builder = new QueryExpressionBuilder<TTableData, TEntity>(this.EntityLogicalName, query);
            var crmQuery = builder.GetQueryExpression();

            var entityCollection = this.OrganizationService.RetrieveMultiple(crmQuery);
            var dataObjects = entityCollection.Entities.Cast<TEntity>().Select(Mapper.Map<TTableData>);
            return Task.FromResult(dataObjects);
        }

        public Task<TTableData> ReplaceAsync(string id, TTableData data)
        {
            OrganizationService.Update(Mapper.Map<TTableData, TEntity>(data));
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
