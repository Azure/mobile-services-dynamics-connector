using Microsoft.Azure.Mobile.Server.Tables;
using System;

namespace ActivityLoggerBackend.Models
{
    public class IncidentDto : ITableData
    {
        public DateTimeOffset? CreatedAt { get; set; }
        public bool Deleted { get; set; }
        public string Id { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public byte[] Version { get; set; }

        public string Text { get; set;}
        public bool Complete { get; set; }
    }
}