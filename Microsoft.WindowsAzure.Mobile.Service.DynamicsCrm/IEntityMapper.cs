using Microsoft.WindowsAzure.Mobile.Service.Tables;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.WindowsAzure.Mobile.Service.DynamicsCrm
{
    /// <summary>
    /// An interface to be implemented by classes to map between <typeparamref name="TTableData"/> and <typeparamref name="TEntity"/> instances.
    /// </summary>
    /// <typeparam name="TTableData">The data object (DTO) used when interacting with the OData endpoint.</typeparam>
    /// <typeparam name="TEntity">The <see cref="Entity"/> type in Dynamics CRM corresponding to the <typeparamref name="TTableData"/> type.</typeparam>
    public interface IEntityMapper<TTableData, TEntity>
        where TTableData : class, ITableData
        where TEntity : Entity
    {
        string GetAttributeName(string propertyName);
        IEnumerable<string> GetAttributeNames();

        /// <summary>
        /// Maps a record from a <typeparamref name="TTableData"/> instance to a <typeparamref name="TEntity"/> instance.
        /// </summary>
        /// <param name="data">The data to be mapped.</param>
        /// <returns>The mapped record.</returns>
        TEntity Map(TTableData data);

        /// <summary>
        /// Maps a record from a <typeparamref name="TEntity"/> instance to a <typeparamref name="TTableData"/> instance.
        /// </summary>
        /// <param name="data">The data to be mapped.</param>
        /// <returns>The mapped record.</returns>
        TTableData Map(TEntity data);
    }
}
