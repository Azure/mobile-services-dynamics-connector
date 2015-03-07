using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using Windows.UI.Popups;
using Microsoft.WindowsAzure.MobileServices;
using Windows.UI.Xaml;
using Windows.Security.Credentials;

namespace DemoMobileApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private MobileServiceUser user;
        private async Task AuthenticateAsync()
        {
            var provider = "aad";

            // Use the PasswordVault to securely store and access credentials.
            PasswordVault vault = new PasswordVault();
            PasswordCredential credential = null;
            string message;

            while (credential == null)
            {
                try
                {
                    // Try to get an existing credential from the vault.
                    credential = vault.FindAllByResource(provider).FirstOrDefault();
                }
                catch (Exception)
                {
                    // When there is no matching resource an error occurs, which we ignore.
                }

                if (credential != null)
                {
                    // Create a user from the stored credentials.
                    user = new MobileServiceUser(credential.UserName);
                    credential.RetrievePassword();
                    user.MobileServiceAuthenticationToken = credential.Password;

                    // Set the user from the stored credentials.
                    App.MobileService.CurrentUser = user;

                    try
                    {
                        // Try to return an item now to determine if the cached credential has expired.
                        var firstContact = await App.MobileService.GetTable<Contact>().Take(1).ToListAsync();
                    }
                    catch (MobileServiceInvalidOperationException ex)
                    {
                        if (ex.Response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        {
                            // Remove the credential with the expired token.
                            vault.Remove(credential);
                            credential = null;
                            continue;
                        }
                    }
                }
                else
                {
                    try
                    {
                        string authority = "https://login.windows.net/sonomap.onmicrosoft.com";
                        string resourceURI = "https://sonoma-azure-demo.azure-mobile.net/login/aad";
                        string clientID = "23bff0e9-7ce7-433a-9d4f-4fb93098c0d3";

                        AuthenticationContext ac = new AuthenticationContext(authority);
                        AuthenticationResult ar = await ac.AcquireTokenAsync(resourceURI, clientID, (Uri)null);

                        if (ar.Status == AuthenticationStatus.Success)
                        {
                            JObject payload = new JObject();
                            payload["access_token"] = ar.AccessToken;
                            user = await App.MobileService.LoginAsync(MobileServiceAuthenticationProvider.WindowsAzureActiveDirectory, payload);
                            message = string.Format("You are now logged in - {0}", user.UserId);
                        }
                        else
                        {
                            message = ar.ErrorDescription;
                        }
                        
                        // Create and store the user credentials.
                        credential = new PasswordCredential(provider,
                            user.UserId, user.MobileServiceAuthenticationToken);
                        vault.Add(credential);
                    }
                    catch (MobileServiceInvalidOperationException)
                    {
                        message = "You must log in. Login Required";
                    }
                }
                message = string.Format("You are now logged in - {0}", user.UserId);
                var dialog = new MessageDialog(message);
                dialog.Commands.Add(new UICommand("OK"));
                await dialog.ShowAsync();
            }
            
        }

        private async void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            // Login the user and then load data from the mobile service.
            await AuthenticateAsync();

            // Hide the login button and load items from the mobile service.
            this.ButtonLogin.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            RefreshContacts();
        }

        private void RefreshContacts()
        {

        }
    }
}
