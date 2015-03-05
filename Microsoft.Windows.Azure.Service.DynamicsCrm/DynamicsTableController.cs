using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Tables;
using Microsoft.Xrm.Client.Services;
using Microsoft.Xrm.Sdk;

namespace Microsoft.Windows.Azure.Service.DynamicsCrm
{
    public abstract class DynamicsTableController<TTableData, TEntity> : TableController<TTableData>
        where TTableData : class, ITableData
        where TEntity : Entity
    {
        public DynamicsTableController(IEntityMapper<TTableData, TEntity> entityMapper) : this("CrmConnection", entityMapper)
        {
        }

        public DynamicsTableController(string connectionStringName, IEntityMapper<TTableData, TEntity> entityMapper)
            : this(new DynamicsCrmDomainManager<TTableData, TEntity>(new OrganizationService(connectionStringName), entityMapper))
        {
        }

        public DynamicsTableController(IDomainManager<TTableData> domainManager) : base(domainManager)
        {
        }
    }
}
