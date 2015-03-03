using Microsoft.WindowsAzure.Mobile.Service.Tables;
using System;

namespace Microsoft.Windows.Azure.Service.DynamicsCrm.WebHost.Models
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
        public String Description { get; set; }
        public String ActivityTypeCode { get; set; }
    }
}