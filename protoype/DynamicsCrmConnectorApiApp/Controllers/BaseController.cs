using Microsoft.Azure.Mobile.Server.DynamicsCrm;
using Microsoft.Azure.Mobile.Server.Tables;
using Microsoft.Xrm.Sdk;

namespace DynamicsConnector.Controllers
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