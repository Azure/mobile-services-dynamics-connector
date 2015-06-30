# Azure Mobile Apps Dynamics CRM Domain Manager and sample

This folder contains a DomainManager extension to Azure Mobile Apps for connecting to Dynamics CRM Online. There is also a sample Azure Mobile App backend project, **ActivityLoggerBackend-preview**. This project sets up the 'tables' endpoints for several built-in CRM entities, such as Activities, Contacts, and Tasks and enables offline sync with the Azure Mobile client SDKs.

## Prerequisites 

- Visual Studio 2013
- .NET Runtime version 2.5.2
- Azure subscription
- Dynamics CRM Online subscription

## Overview

To deploy this sample, you will follow these steps:

1. Provision a new Azure Mobile App, which will set up an authentication gateway and an Azure Web App where you will deploy your backend code.
2. In Visual Studio, build and deploy the project **ActivityLoggerBackend-preview.sln**.
3. Configure the Azure Active Directory authentication settings to enable on-behalf-of access between a mobile client application and the CRM backend.

## AAD configuration overview

- You will create two Active Directory application entries:
  - One for the iOS client app, called `ActivityLoggerNative`
  - One for the Azure Mobile App backend, called `ActivityLoggerBackend`

- The native app AAD registration `ActivityLoggerNative` will have the following permissions
  - Azure Active Directory: **Enable sign-on and read users' profiles**
  - ActivityLoggerBackend: **Access ActivityLoggerBackend**

- The Azure Mobile App backend AAD registration `ActivityLoggerBackend` will have the following permissions:
  - Azure Active Directory: **Enable sign-on and read users' profiles**
  - Dynamics CRM Online: **Access CRM Online as organization users**

Note that the native client app does not need direct permissions to Dynamics CRM. Instead, it will retrieve an authentication token from AAD for the specific logged-in user. Then, this token is passed to the Azure Mobile App backend as part of the `LoginAsync` method. Since the Azure Mobile App backend has access to AAD and has delegated access to Dynamics CRM Online, it can use this user authentication token to securely take actions in Dynamics CRM on-behalf-of the logged in user in the native client application.

## 1. Provision a new Azure Mobile App

- Follow the steps in the tutorial TODO to provision a new Azure Mobile App.

## 2. Publish the backend project to your Azure Mobile App code site

- In Visual Studio, open the solution **ActivityLoggerBackend-preview.sln** and build.

- Right-click the project **ActivityLoggerBackend-preview** and click **Publish**.

- Select **Azure Web Apps** as the publish target and select your Azure Mobile App code site.

- Once the wizard has completed, you will see a confirmation page showing that your backend has been deployed.

## 3. Register your backend application with Azure Active Directory

1. Log on to the [Preview Azure Management Portal], and navigate to your App Service gateway.

2. Under **Settings**, choose **Identity**, and then select **Azure Active Directory**. Copy the **APP URL**. Make sure that you are using the HTTPS scheme.

3. Sign in to the [Classic Azure Management Portal] and navigate to **Active Directory**.

4. Select your directory, and then select the **Applications** tab at the top. Click **ADD** at the bottom to create a new app registration. 

5. Click **Add an application my organization is developing**.

6. In the Add Application Wizard, enter **ActivityLoggerBackend** for your application and click the **Web Application And/Or Web API** type. Then click to continue.

7. In the **SIGN-ON URL** box, paste the App ID you copied from the Active Directory identity provider settings of your gateway. Enter the same unique resource identifier in the **App ID URI** box. Then click to continue.

8. Once the application has been added, click the **Configure** tab. Edit the **Reply URL** under **Single Sign-on** to be the URL of your gateway appended with the path, _/signin-aad_. For example, `https://contosogateway.azurewebsites.net/signin-aad`. Make sure that you are using the HTTPS scheme.

9. In the **Configure** tab, in the **permissions to other applications** section, click **Add application**. Select **Dynamics CRM Online** and grant the **Access CRM Online as organization users** delegated permission. Then click **Save**.

10. Ensure there is another entry in the **permissions** section for **Windows Azure Active Directory** with the Delegated Permissions **Enable sign-on and read users' profiles**.

## <a name="secrets"> </a>4. Add Azure Active Directory information to your Azure Mobile App backend

1. In your Active Directory application, navigate to **CONFIGURE** tab and copy the **Client ID** for the app.

2. Return to your Mobile App in the preview management portal. In the **User Authentication** blade for your Mobile App gateway, paste in the **Client ID** setting for the Azure Active Directory identity provider.
  
3. In the **Allowed Tenants** list, add the domain of the directory in which you registered the application (e.g. contoso.onmicrosoft.com). You can find your default domain name by clicking the **Domains** tab on your Azure Active Directory tenant. Add your domain name to the **Allowed Tenants** list then click **Save**.  

## 5. Register the native iOS application with Azure Active Directory

1. Navigate to **Active Directory** in the [Classic Azure Management Portal].

2. Select your directory, and then select the **Applications** tab at the top. Click **ADD** at the bottom to create a new app registration. 

3. Click **Add an application my organization is developing**.

4. In the Add Application Wizard, enter a **Name** for your application such as "ActivityLoggerNativeApp" and click the  **Native Client Application** type. Then click to continue.

5. In the **Redirect URI** box, enter the /login/done endpoint for your App Service gateway. This value should be similar to https://contoso.azurewebsites.net/login/done.

6. Once the native application has been added, click the **Configure** tab. Copy the **Client ID**. You will need this later.

7. Scroll the page down to the **permissions to other applications** section and click **Add application**.

8. Search for the web application **ActivityLoggerBackend** that you registered earlier and click the plus icon. Then click the check to close the dialog.

9. On the new entry you just added, open the **Delegated Permissions** dropdown and select **Access (appName)**. Then click **Save**

## 6. Configure your Mobile Backend app settings

1. Return to the AAD Applications tab for your tenant, and select the application **ActivityLoggerBackend**.

2. Under Configure, scroll down to Keys. You will obtain a Client Secret by generating a new key. Note once you create a key and leave the page, there is no way to get it out of the portal again. Upon creation you must copy and save this value in a secure location. Select a duration for your key, then click save, and copy out the resulting value.

3. Navigate to your Mobile App code site in the preview management portal and click App Settings. Here you can provide a key-value pair to help you reference the necessary credentials.

* Set CrmAuthorityUrl to be the authority endpoint for your AAD tenant. This should be the same as the authority value used for your client app. It will be of the form `https://login.windows.net/contoso.onmicrosoft.com`

* Set CrmClientSecret to be the client secret value you obtained earlier.

* Set CrmUrl to be the URL for your Dynamics CRM Online tenant, which will have the form `https://donnam.crm.dynamics.com`

* Set AzureActiveDirectoryClientId to the client ID of the **ActivityLoggerBackend** Active Directory application.

[Azure Management Portal]: https://manage.windowsazure.com/
[How to configure your Mobile App with Azure Active Directory]: ../articles/app-service-how-to-configure-active-directory-authentication-preview.md


[Preview Azure Management Portal]: https://portal.azure.com/
[Classic Azure Management Portal]: https://manage.windowsazure.com/
[SharePoint Online]: http://office.microsoft.com/en-us/sharepoint/
[Authenticate your app with Active Directory Authentication Library Single Sign-On]: app-service-mobile-dotnet-backend-ios-aad-sso-preview.md
[Mobile Apps .NET Backend App Service Extension]: http://www.nuget.org/packages/Microsoft.Azure.Mobile.Server.AppService/
