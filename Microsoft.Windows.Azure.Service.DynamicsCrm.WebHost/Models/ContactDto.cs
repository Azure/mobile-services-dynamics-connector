using Microsoft.WindowsAzure.Mobile.Service.Tables;
using System;

namespace Microsoft.Windows.Azure.Service.DynamicsCrm.WebHost.Models
{
    public class ContactDto : ITableData
    {
        public DateTimeOffset? CreatedAt { get; set; }
        public bool Deleted { get; set; }
        public string Id { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public byte[] Version { get; set; }
    }
}