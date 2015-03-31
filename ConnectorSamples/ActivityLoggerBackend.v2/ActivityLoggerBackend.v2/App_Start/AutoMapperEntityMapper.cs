using AutoMapper;
using Microsoft.Azure.Mobile.Server.Tables;
using Microsoft.Azure.Mobile.Server.DynamicsCrm;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;


namespace ActivityLoggerBackend
{
    public static class AutoMapperEntityMapper
    {
        public static void InitializeDynamicsCrmCommonMaps()
        {
            Mapper.CreateMap<EntityReference, Guid>().ConvertUsing(er => er == null ? Guid.Empty : er.Id);
            Mapper.CreateMap<EntityReference, Guid?>().ConvertUsing(er => er == null ? (Guid?)null : er.Id);
            Mapper.CreateMap<EntityReference, string>().ConvertUsing(er => er == null ? null : er.LogicalName);
            Mapper.CreateMap<Guid?, EntityReference>().ConvertUsing(g => g == null ? null : new EntityReference { Id = g.Value });
            Mapper.CreateMap<Guid, EntityReference>().ConvertUsing(g => g == Guid.Empty ? null : new EntityReference { Id = g });
            Mapper.CreateMap<OptionSetValue, int?>().ConvertUsing(osv => osv == null ? (int?)null : osv.Value);
            Mapper.CreateMap<OptionSetValue, int>().ConvertUsing(osv => osv == null ? 0 : osv.Value);
            Mapper.CreateMap<int, OptionSetValue>().ConvertUsing(i => i == 0 ? null : new OptionSetValue(i));
            Mapper.CreateMap<int?, OptionSetValue>().ConvertUsing(i => i == null ? null : new OptionSetValue(i.Value));
        }
    }

    public class AutoMapperEntityMapper<TTableData, TEntity> : IEntityMapper<TTableData, TEntity>
        where TTableData : class, ITableData
        where TEntity : Entity
    {
        protected Dictionary<String, string> PropertyMap { get; set; }
        protected bool EnableSoftDelete { get; set; }

        public AutoMapperEntityMapper(bool enableSoftDelete)
        {
            this.EnableSoftDelete = enableSoftDelete;

            var map = Mapper.FindTypeMapFor<TTableData, TEntity>();
            if (map == null) throw new InvalidOperationException(String.Format("Could not find a map from {0} to {1}.", typeof(TTableData), typeof(TEntity)));

            this.PropertyMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var propertyMaps = map.GetPropertyMaps();
            foreach (var m in propertyMaps)
            {
                if (m.SourceMember != null)
                {
                    this.PropertyMap.Add(m.SourceMember.Name, m.DestinationProperty.Name.ToLowerInvariant());
                }
            }

            PropertyMap["createdat"] = "createdon";
            PropertyMap["updatedat"] = "modifiedon";
        }

        public string GetAttributeName(string propertyName)
        {
            return this.PropertyMap[propertyName];
        }

        public IEnumerable<string> GetAttributeNames()
        {
            var names = new HashSet<String>(PropertyMap.Values);
            
            names.Add("modifiedon");
            names.Add("createdon");
            
            if(EnableSoftDelete)
                names.Add("statecode");
            
            return names;
        }

        public TEntity Map(TTableData data)
        {
            return Mapper.Map<TTableData, TEntity>(data);
        }

        public TTableData Map(TEntity entity)
        {
            var data = Mapper.Map<TEntity, TTableData>(entity);

            data.UpdatedAt = entity.GetAttributeValue<DateTime?>("modifiedon");
            data.CreatedAt = entity.GetAttributeValue<DateTime?>("createdon");

            // Upper case guids are currently required by the Azure Mobile Services
            // client libraries because they create new records with Guids as uppercase strings.
            // They will not match against records returned from the service unless
            // the strings are identical.
            data.Id = entity.Id.ToString().ToUpperInvariant();

            if (EnableSoftDelete)
            {
                var optionSetValue = entity.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("statecode");
                if (optionSetValue != null)
                {
                    data.Deleted = optionSetValue.Value != 0;
                }
            }

            return data;
        }
    }
}
