using AutoMapper;
using Microsoft.Windows.Azure.Service.DynamicsCrm.Automapper;
using Microsoft.Windows.Azure.Service.DynamicsCrm.WebHost.Models;
using Microsoft.WindowsAzure.Mobile.Service;
using System;
using System.Web.Http;

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
            map.ForMember(dto => dto.City, opt => opt.MapFrom(crm => crm.Address1_City))
                .ForMember(dto => dto.CreatedAt, opt => opt.MapFrom(crm => (DateTimeOffset?)crm.CreatedOn))
                .ForMember(dto => dto.UpdatedAt, opt => opt.MapFrom(crm => (DateTimeOffset?)crm.ModifiedOn))
                .ForMember(dto => dto.StatusCode, opt => opt.ResolveUsing(new OptionSetValueToIntValueResolver()).FromMember(crm => crm.StatusCode));

            var reverseMap = map.ReverseMap();
            reverseMap.ForMember(crm => crm.Address1_City, opt => opt.MapFrom(dto => dto.City))
                .ForMember(crm => crm.CreatedOn, opt => opt.MapFrom(dto => dto.CreatedAt))
                .ForMember(crm => crm.ModifiedOn, opt => opt.MapFrom(dto => dto.UpdatedAt))
                .ForMember(crm => crm.StatusCode, opt => opt.ResolveUsing(new IntToOptionSetValueValueResolver()).FromMember(crm => crm.StatusCode))
                .AfterMap((dto, crm) =>
                {
                    if (crm.Id == Guid.Empty)
                        crm.Id = Guid.NewGuid();
                });
        }
    }
}

