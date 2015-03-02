using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Windows.Azure.Service.DynamicsCrm.Automapper
{
    public class IntToOptionSetValueValueResolver : ValueResolver<int?, Microsoft.Xrm.Sdk.OptionSetValue>
    {
        protected override Microsoft.Xrm.Sdk.OptionSetValue ResolveCore(int? source)
        {
            return source == null ? null : new Microsoft.Xrm.Sdk.OptionSetValue(source.Value);
        }
    }
}
