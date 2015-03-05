using Microsoft.WindowsAzure.Mobile.Service.Tables;
using Microsoft.Xrm.Sdk;

namespace Microsoft.Windows.Azure.Service.DynamicsCrm.WebHost.Controllers
{
    public abstract class BaseController<TDto, TEntity> : DynamicsTableController<TDto, TEntity>
        where TDto : class, ITableData
        where TEntity : Entity
    {
        public BaseController() : base(new AutoMapperAttributeMap<TDto, TEntity>())
        {
        }
    }
}