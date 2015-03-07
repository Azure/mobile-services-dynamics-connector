using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Tables;
using Microsoft.Xrm.Client.Services;
using Microsoft.Xrm.Sdk;
using System.Web.Http.Controllers;

namespace Microsoft.Windows.Azure.Service.DynamicsCrm
{
    public abstract class DynamicsCrmTableController<TTableData, TEntity> : TableController<TTableData>
        where TTableData : class, ITableData
        where TEntity : Entity
    {
        protected IEntityMapper<TTableData, TEntity> EntityMapper { get; set; }

        public DynamicsCrmTableController(IEntityMapper<TTableData, TEntity> entityMapper)
        {
            EntityMapper = entityMapper;
        }

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);

            DomainManager = new DynamicsCrmDomainManager<TTableData, TEntity>(this.Request, this.Services, this.EntityMapper);
        }
    }
}
