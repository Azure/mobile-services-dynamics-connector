@ECHO OFF

..\Tools\CrmSvcUtil\CrmSvcUtil /url:https://ORGNAME.crm.dynamics.com/XRMServices/2011/Organization.svc /username:USERNAME /password:PASSWORD /namespace:ActivityLogger.Models /out:.\ActivityLoggerBackend\Models\Entities.cs
