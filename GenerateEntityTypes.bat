@ECHO OFF

.\Tools\CrmSvcUtil\CrmSvcUtil /url:https://sonomap.crm.dynamics.com/XRMServices/2011/Organization.svc /username:developer@sonomap.onmicrosoft.com /password:$0n0m@p! /namespace:Microsoft.Windows.Azure.Service.DynamicsCrm.WebHost.Models /out:.\Microsoft.Windows.Azure.Service.DynamicsCrm.WebHost\Models\Entities.cs
