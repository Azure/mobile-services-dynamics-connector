using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Windows.Azure.Service.DynamicsCrm
{
    public class AutoMapperAttributeMap<TTableData, TEntity> : IAttributeMap
    {
        protected Dictionary<String, IMemberAccessor> PropertyMap { get; set; }

        public AutoMapperAttributeMap()
        {
            var map = Mapper.FindTypeMapFor<TTableData, TEntity>();
            if (map == null) throw new InvalidOperationException(String.Format("Could not find a map from {0} to {1}.", typeof(TTableData), typeof(TEntity)));

            this.PropertyMap = new Dictionary<string, IMemberAccessor>(StringComparer.OrdinalIgnoreCase);
            var propertyMaps = map.GetPropertyMaps();
            foreach (var m in propertyMaps)
            {
                if (m.SourceMember != null)
                {
                    this.PropertyMap.Add(m.SourceMember.Name, m.DestinationProperty);
                }
            }
        }

        public string GetAttributeName(string propertyName)
        {
            return this.PropertyMap[propertyName].Name.ToLowerInvariant();
        }


        public IEnumerable<string> GetAttributeNames()
        {
            return from p in PropertyMap.Values
                   select p.Name.ToLowerInvariant();
        }
    }
}
