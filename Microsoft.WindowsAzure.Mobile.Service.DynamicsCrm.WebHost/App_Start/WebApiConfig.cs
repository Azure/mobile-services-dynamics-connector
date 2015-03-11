using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service.DynamicsCrm.WebHost.Models;
using Microsoft.WindowsAzure.Mobile.Service;
using AutoMapper;
using Microsoft.Xrm.Sdk;
using Microsoft.WindowsAzure.Mobile.Service.Tables;
using Microsoft.WindowsAzure.Mobile.Service.Security.Providers;

namespace Microsoft.WindowsAzure.Mobile.Service.DynamicsCrm.WebHost
{
    public static class WebApiConfig
    {
        public static void Register()
        {
            // Use this class to set configuration options for your mobile service
            ConfigOptions options = new ConfigOptions();
            options.LoginProviders.Remove(typeof(AzureActiveDirectoryLoginProvider));
            options.LoginProviders.Add(typeof(AzureActiveDirectoryExtendedLoginProvider));

            // Use this class to set WebAPI configuration options
            HttpConfiguration config = ServiceConfig.Initialize(new ConfigBuilder(options));

            // enforce user authentication even when debugging locally
            config.SetIsHosted(true);

            // To display errors in the browser during development, uncomment the following
            // line. Comment it out again when you deploy your service for production use.
            // config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            AutoMapperEntityMapper.InitializeDynamicsCrmCommonMaps();

            Mapper.CreateMap<Contact, ContactDto>()
                .ReverseMap();

            Mapper.CreateMap<Task, ActivityDto>()
                .ForMember(a => a.Details, opt => opt.MapFrom(t => t.Description))
                .ReverseMap()
                .ForMember(t => t.Description, opt => opt.MapFrom(a => a.Details))
                .AfterMap((a, t) =>
                {
                    if(t.RegardingObjectId != null)
                    {
                        t.RegardingObjectId.LogicalName = Contact.EntityLogicalName;
                    }
                });

            Mapper.CreateMap<PhoneCall, ActivityDto>()
                .ForMember(a => a.Details, opt => opt.MapFrom(p => p.Description))
                .ReverseMap()
                .ForMember(p => p.Description, opt => opt.MapFrom(a => a.Details))
                .AfterMap((a, p) =>
                {
                    if (p.RegardingObjectId != null)
                    {
                        p.RegardingObjectId.LogicalName = Contact.EntityLogicalName;
                    }
                });

            Mapper.CreateMap<Appointment, ActivityDto>()
                .ForMember(a => a.Details, opt => opt.MapFrom(ap => ap.Description))
                .ReverseMap()
                .ForMember(ap => ap.Description, opt => opt.MapFrom(a => a.Details))
                .AfterMap((a, ap) =>
                {
                    if (ap.RegardingObjectId != null)
                    {
                        ap.RegardingObjectId.LogicalName = Contact.EntityLogicalName;
                    }
                });
        }

        
    }
}

