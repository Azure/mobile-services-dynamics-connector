using Microsoft.WindowsAzure.Mobile.Service.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Microsoft.Windows.Azure.Service.DynamicsCrm.WebHost.Models
{
    public class AccountDto: ITableData
    {
        public DateTimeOffset? CreatedAt { get; set; }
        public bool Deleted { get; set; }
        public string Id { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public byte[] Version { get; set; }

        public string Name { get; set; }
        public string City { get; set; }
        public int? StatusCode { get; set; }
    }
}