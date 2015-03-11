using Microsoft.WindowsAzure.Mobile.Service.Tables;
using Microsoft.Xrm.Sdk;

namespace Microsoft.WindowsAzure.Mobile.Service.DynamicsCrm.WebHost.Controllers
{
    public abstract class BaseController<TDto, TEntity> : DynamicsCrmTableController<TDto, TEntity>
        where TDto : class, ITableData
        where TEntity : Entity
    {
        public BaseController()
            : this(false)
        {
        }

        public BaseController(bool enableSoftDelete)
            : base(new AutoMapperEntityMapper<TDto, TEntity>(enableSoftDelete))
        {

        }
    }
}