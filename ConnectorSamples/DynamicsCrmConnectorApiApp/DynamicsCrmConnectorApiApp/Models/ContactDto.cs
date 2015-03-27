using Microsoft.Azure.Mobile.Server.Tables;
using System;

namespace DynamicsCrmConnectorApiApp.Models
{
    public class ContactDto : ITableData
    {
        public DateTimeOffset? CreatedAt { get; set; }
        public bool Deleted { get; set; }
        public string Id { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public byte[] Version { get; set; }

        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String Address1_Line1 { get; set; }
        public String Address1_City { get; set; }
        public String Address1_StateOrProvince { get; set; }
        public String Address1_PostalCode { get; set; }
        public String EMailAddress1 { get; set; }
        public String JobTitle { get; set; }
        public String Telephone1 { get; set; }
    }
}