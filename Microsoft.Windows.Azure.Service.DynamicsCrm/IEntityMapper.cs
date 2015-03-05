using Microsoft.WindowsAzure.Mobile.Service.Tables;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Windows.Azure.Service.DynamicsCrm
{
    public interface IEntityMapper<TDto, TEntity>
        where TDto : class, ITableData
        where TEntity : Entity
    {
        string GetAttributeName(string propertyName);
        IEnumerable<string> GetAttributeNames();

        TEntity MapTo(TDto data);
        TDto MapFrom(TEntity data);
    }
}
