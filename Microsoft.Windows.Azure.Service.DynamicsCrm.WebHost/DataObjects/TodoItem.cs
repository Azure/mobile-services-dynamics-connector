using Microsoft.WindowsAzure.Mobile.Service;

namespace Microsoft.Windows.Azure.Service.DynamicsCrm.WebHost.DataObjects
{
    public class TodoItem : EntityData
    {
        public string Text { get; set; }

        public bool Complete { get; set; }
    }
}