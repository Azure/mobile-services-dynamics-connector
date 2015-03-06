using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Tables;
using Microsoft.Xrm.Client.Services;
using Microsoft.Xrm.Sdk;

namespace Microsoft.Windows.Azure.Service.DynamicsCrm
{
    /// <summary>
    /// Provides a Dynamics specific partial impelmentation of <see cref="TableController{TTableData}"/>.
    /// </summary>
    /// <typeparam name="TTableData">The data object (DTO) type.</typeparam>
    /// <typeparam name="TEntity">The corresponding <see cref="Entity"/> type in Dynamics for the <typeparamref name="TTableData"/> type.</typeparam>
    public abstract class DynamicsTableController<TTableData, TEntity> : TableController<TTableData>
        where TTableData : class, ITableData
        where TEntity : Entity
    {
        /// <summary>
        /// Creates a new instance of <see cref="DynamicsTableController{TTableData,TEntity}"/> using the <see cref="IEntityMapper{TTableData,TEntity}"/> specified.
        /// This constructor assumes a connection string named CrmConnection for connecting to Dynamics CRM exists in the web.config.
        /// </summary>
        /// <param name="entityMapper">The <see cref="IEntityMapper{TTableData,TEntity}"/> implementation to use.</param>
        public DynamicsTableController(IEntityMapper<TTableData, TEntity> entityMapper) : this("CrmConnection", entityMapper)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="DynamicsTableController{TTableData,TEntity}"/> using the <see cref="IEntityMapper{TTableData,TEntity}"/> and <paramref name="connectionStringName"/> provided.
        /// </summary>
        /// <param name="connectionStringName">The name of the Dynamics CRM connection string in the web.config to use.</param>
        /// <param name="entityMapper">The <see cref="IEntityMapper{TTableData,TEntity}"/> implementation to use.</param>
        public DynamicsTableController(string connectionStringName, IEntityMapper<TTableData, TEntity> entityMapper)
            : this(new DynamicsCrmDomainManager<TTableData, TEntity>(new OrganizationService(connectionStringName), entityMapper))
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="DynamicsTableController{TTableData,TEntity}"/> using the <see cref="IDomainManager{TTableData}"/> instance provided.
        /// </summary>
        /// <param name="domainManager">The <see cref="IDomainManager{TTableData}"/> instance to use.</param>
        public DynamicsTableController(IDomainManager<TTableData> domainManager) : base(domainManager)
        {
        }
    }
}
