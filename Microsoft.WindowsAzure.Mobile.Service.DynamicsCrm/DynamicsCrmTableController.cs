using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Tables;
using Microsoft.Xrm.Client.Services;
using Microsoft.Xrm.Sdk;

namespace Microsoft.WindowsAzure.Mobile.Service.DynamicsCrm
{
    /// <summary>
    /// Provides a Dynamics specific partial impelmentation of <see cref="TableController{TTableData}"/>.
    /// </summary>
    /// <typeparam name="TTableData">The data object (DTO) type.</typeparam>
    /// <typeparam name="TEntity">The corresponding <see cref="Entity"/> type in Dynamics for the <typeparamref name="TTableData"/> type.</typeparam>
    public abstract class DynamicsCrmTableController<TTableData, TEntity> : TableController<TTableData>
        where TTableData : class, ITableData
        where TEntity : Entity
    {
        /// <summary>
        /// The entity mapper used to convert between TTableData and TEntity types.
        /// </summary>
        protected IEntityMapper<TTableData, TEntity> EntityMapper { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="DynamicsCrmTableController{TTableData,TEntity}"/> using the <see cref="IEntityMapper{TTableData,TEntity}"/> specified.
        /// This constructor assumes a connection string named CrmConnection for connecting to Dynamics CRM exists in the web.config.
        /// </summary>
        /// <param name="entityMapper">The <see cref="IEntityMapper{TTableData,TEntity}"/> implementation to use.</param>
        public DynamicsCrmTableController(IEntityMapper<TTableData, TEntity> entityMapper)
        {
            EntityMapper = entityMapper;
        }

        /// <summary>
        /// Initializes the System.Web.Http.ApiController instance with the specified controllerContext.
        /// </summary>
        /// <param name="controllerContext">The System.Web.Http.Controllers.HttpControllerContext object that is used for the initialization</param>
        protected override void Initialize(System.Web.Http.Controllers.HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);

            this.DomainManager = new DynamicsCrmDomainManager<TTableData, TEntity>(Request, Services, EntityMapper);
        }

    }
}
