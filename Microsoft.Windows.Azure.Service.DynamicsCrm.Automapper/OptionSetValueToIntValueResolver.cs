using AutoMapper;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Windows.Azure.Service.DynamicsCrm.Automapper
{
    public class OptionSetValueToIntValueResolver : IValueResolver
    {
        public ResolutionResult Resolve(ResolutionResult source)
        {
            if (source.ShouldIgnore || source.Value == null)
                return source.New(null);
            var value = source.Value as OptionSetValue;
            if (value == null)
                return source.New(null);
            return source.New(value.Value);
        }
    }
}
