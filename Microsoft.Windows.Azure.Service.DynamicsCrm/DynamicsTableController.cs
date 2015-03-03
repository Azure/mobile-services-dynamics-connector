using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Tables;
using Microsoft.Xrm.Client.Services;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Windows.Azure.Service.DynamicsCrm
{
    public abstract class DynamicsTableController<TDTO, TEntity> : TableController<TDTO>
        where TDTO : class, ITableData
        where TEntity : Entity
    {
        public DynamicsTableController() : this("CrmConnection")
        {
        }

        public DynamicsTableController(string connectionStringName)
            : this(new DynamicsCrmDomainManager<TDTO, TEntity>(new OrganizationService(connectionStringName)))
        {
        }

        public DynamicsTableController(IDomainManager<TDTO> domainManager) : base(domainManager)
        {
        }
    }
}
