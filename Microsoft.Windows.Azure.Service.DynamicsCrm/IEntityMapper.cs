using Microsoft.WindowsAzure.Mobile.Service.Tables;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Windows.Azure.Service.DynamicsCrm
{
    public interface IEntityMapper<TTableData, TEntity>
        where TTableData : class, ITableData
        where TEntity : Entity
    {
        string GetAttributeName(string propertyName);
        IEnumerable<string> GetAttributeNames();

        TEntity MapTo(TTableData data);
        TTableData MapFrom(TEntity data);
    }
}
