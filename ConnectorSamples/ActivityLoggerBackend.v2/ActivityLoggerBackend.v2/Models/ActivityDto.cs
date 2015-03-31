using ActivityLoggerBackend;
using Microsoft.Azure.Mobile.Server.Tables;
using Newtonsoft.Json;
using System;

namespace ActivityLoggerBackend.Models
{
    public class ActivityDto : ITableData
    {
        public DateTimeOffset? CreatedAt { get; set; }
        public bool Deleted { get; set; }
        public string Id { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public byte[] Version { get; set; }

        public String Subject { get; set; }
        public DateTime? ActualEnd { get; set; }
        public String Details { get; set; }
        public String ActivityTypeCode { get; set; }

        // Upper case guids are currently required by the Azure Mobile Services
        // client libraries because they create new records with Guids as uppercase strings.
        // They will not match against records returned from the service unless
        // the strings are identical.
        [JsonConverter(typeof(UppercaseGuidConverter))]
        public Guid? RegardingObjectId { get; set; }
    }
}