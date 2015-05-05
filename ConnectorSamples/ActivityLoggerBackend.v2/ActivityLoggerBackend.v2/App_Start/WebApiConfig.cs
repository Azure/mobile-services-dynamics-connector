using AutoMapper;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.AppService.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using ActivityLoggerBackend.Models;

namespace ActivityLoggerBackend
{
    public static class WebApiConfig
    {
        public static void Register()
        {
            AppServiceExtensionConfig.Initialize();

            // Use this class to set configuration options for your mobile service
            ConfigOptions options = new ConfigOptions();

            // Use this class to set WebAPI configuration options
            HttpConfiguration config = ServiceConfig.Initialize(new ConfigBuilder(options));

            // enforce user authentication even when debugging locally
            config.SetIsHosted(true);

            // To display errors in the browser during development, uncomment the following
            // line. Comment it out again when you deploy your service for production use.
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            AutoMapperEntityMapper.InitializeDynamicsCrmCommonMaps();

            Mapper.CreateMap<Contact, ContactDto>()
                .ReverseMap();

            Mapper.CreateMap<Incident, IncidentDto>()
                .ForMember(a => a.Complete, opt => opt.MapFrom(t => (t.StatusCode.Value == 1)) )
                .ForMember(a => a.Text, opt => opt.MapFrom(t => t.Title))
                .ReverseMap()
                .ForMember(t => t.ActivitiesComplete, opt => opt.MapFrom(a => a.Complete))
                .ForMember(t => t.Title, opt => opt.MapFrom(a => a.Text))
                /*.AfterMap((a, t) =>
                {
                    if (t.RegardingObjectId != null)
                    {
                        t.RegardingObjectId.LogicalName = Incident.EntityLogicalName;
                    }
                })*/;

            Mapper.CreateMap<Task, ActivityDto>()
                .ForMember(a => a.Details, opt => opt.MapFrom(t => t.Description))
                .ReverseMap()
                .ForMember(t => t.Description, opt => opt.MapFrom(a => a.Details))
                .AfterMap((a, t) =>
                {
                    if (t.RegardingObjectId != null)
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
