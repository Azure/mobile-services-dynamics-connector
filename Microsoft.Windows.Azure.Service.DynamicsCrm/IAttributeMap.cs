using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Windows.Azure.Service.DynamicsCrm
{
    public interface IAttributeMap
    {
        string GetAttributeName(string propertyName);
        IEnumerable<string> GetAttributeNames();
    }
}
