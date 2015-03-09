@ECHO OFF

.\Tools\CrmSvcUtil\CrmSvcUtil /url:https://ORGNAME.crm.dynamics.com/XRMServices/2011/Organization.svc /username:USERNAME /password:PASSWORD /namespace:Microsoft.Windows.Azure.Service.DynamicsCrm.WebHost.Models /out:.\Microsoft.Windows.Azure.Service.DynamicsCrm.WebHost\Models\Entities.cs
