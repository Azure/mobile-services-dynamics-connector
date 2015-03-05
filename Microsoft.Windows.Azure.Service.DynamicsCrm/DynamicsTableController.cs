using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Tables;
using Microsoft.Xrm.Client.Services;
using Microsoft.Xrm.Sdk;

namespace Microsoft.Windows.Azure.Service.DynamicsCrm
{
    public abstract class DynamicsTableController<TDto, TEntity> : TableController<TDto>
        where TDto : class, ITableData
        where TEntity : Entity
    {
        public DynamicsTableController() : this("CrmConnection")
        {
        }

        public DynamicsTableController(string connectionStringName)
            : this(new DynamicsCrmDomainManager<TDto, TEntity>(new OrganizationService(connectionStringName)))
        {
        }

        public DynamicsTableController(IDomainManager<TDto> domainManager) : base(domainManager)
        {
        }
    }
}
