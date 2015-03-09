@ECHO OFF

.\Tools\CrmSvcUtil\CrmSvcUtil /url:https://ORGNAME.crm.dynamics.com/XRMServices/2011/Organization.svc /username:USERNAME /password:PASSWORD /namespace:Microsoft.WindowsAzure.Mobile.Service.DynamicsCrm.WebHost.Models /out:.\Microsoft.WindowsAzure.Mobile.Service.DynamicsCrm.WebHost\Models\Entities.cs
