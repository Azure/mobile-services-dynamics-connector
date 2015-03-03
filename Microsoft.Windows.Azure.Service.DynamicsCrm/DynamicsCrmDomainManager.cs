using AutoMapper;
using Microsoft.WindowsAzure.Mobile.Service.Tables;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            throw new NotImplementedException();
        }

        public Task<TTableData> InsertAsync(TTableData data)
        {
            throw new NotImplementedException();
        }

        public System.Web.Http.SingleResult<TTableData> Lookup(string id)
        {
            throw new NotImplementedException();
        }

        public Task<System.Web.Http.SingleResult<TTableData>> LookupAsync(string id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TTableData> Query()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TTableData>> QueryAsync(ODataQueryOptions query)
        {
            var attributeMap = new AutoMapperAttributeMap<TTableData, TEntity>();
            var builder = new QueryExpressionBuilder(this.EntityLogicalName, query, attributeMap);
            var crmQuery = builder.GetQueryExpression();

            var entityCollection = this.OrganizationService.RetrieveMultiple(crmQuery);
            var dataObjects = entityCollection.Entities.Cast<TEntity>().Select(Mapper.Map<TTableData>);
            return Task.FromResult(dataObjects);
        }

        public Task<TTableData> ReplaceAsync(string id, TTableData data)
        {
            throw new NotImplementedException();
        }

        public Task<TTableData> UpdateAsync(string id, System.Web.Http.OData.Delta<TTableData> patch)
        {
            throw new NotImplementedException();
        }
    }
}
