using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Web.Http;
using Microsoft.Windows.Azure.Service.DynamicsCrm.WebHost.DataObjects;
using Microsoft.Windows.Azure.Service.DynamicsCrm.WebHost.Models;
using Microsoft.WindowsAzure.Mobile.Service;
using AutoMapper;
using Microsoft.Xrm.Sdk;

namespace Microsoft.Windows.Azure.Service.DynamicsCrm.WebHost
{
    public static class WebApiConfig
    {
        public static void Register()
        {
            // Use this class to set configuration options for your mobile service
            ConfigOptions options = new ConfigOptions();

            // Use this class to set WebAPI configuration options
            HttpConfiguration config = ServiceConfig.Initialize(new ConfigBuilder(options));

            // To display errors in the browser during development, uncomment the following
            // line. Comment it out again when you deploy your service for production use.
            // config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            var map = Mapper.CreateMap<Account, AccountDto>();
            Mapper.CreateMap<EntityReference, Guid>().ConvertUsing(er => er == null ? Guid.Empty : er.Id);
            Mapper.CreateMap<EntityReference, Guid?>().ConvertUsing(er => er == null ? (Guid?)null : er.Id);
            Mapper.CreateMap<EntityReference, string>().ConvertUsing(er => er == null ? null : er.LogicalName);

            // TODO: how to resolve logical name?
            Mapper.CreateMap<Guid?, EntityReference>().ConvertUsing(g => g == null ? null : new EntityReference { Id = g.Value });
            Mapper.CreateMap<Guid, EntityReference>().ConvertUsing(g => g == Guid.Empty ? null : new EntityReference { Id = g });

            map.ForMember(dto => dto.City, opt => opt.MapFrom(crm => crm.Address1_City));
            map.ForMember(dto => dto.CreatedAt, opt => opt.MapFrom(crm => (DateTimeOffset?)crm.CreatedOn));
            map.ForMember(dto => dto.UpdatedAt, opt => opt.MapFrom(crm => (DateTimeOffset?)crm.ModifiedOn));
            map.ForMember(dto => dto.ParentAccountId, opt => opt.MapFrom(crm => crm.ParentAccountId));
            map.ForMember(dto => dto.ParentAccountType, opt => opt.MapFrom(crm => crm.ParentAccountId));
            
            var reverseMap = map.ReverseMap();
            reverseMap.ForMember(crm => crm.Address1_City, opt => opt.MapFrom(dto => dto.City));
            reverseMap.ForMember(crm => crm.CreatedOn, opt => opt.MapFrom(dto => dto.CreatedAt));
            reverseMap.ForMember(crm => crm.ModifiedOn, opt => opt.MapFrom(dto => dto.UpdatedAt));
            reverseMap.ForMember(crm => crm.ParentAccountId, opt => opt.MapFrom(dto => dto.ParentAccountId));
            reverseMap.AfterMap((dto, crm) =>
            {
                if (crm.ParentAccountId != null)
                    crm.ParentAccountId.LogicalName = "account";
            });
        }
    }
}

