using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json.Linq;
using System;
using Windows.UI.Popups;
using Microsoft.WindowsAzure.MobileServices;

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

        protected override async void OnNavigatedTo(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            await AuthenticateAsync();
        }

        private MobileServiceUser user;
        private async Task AuthenticateAsync()
        {
            string authority = "https://login.windows.net/sonomap.onmicrosoft.com";
            string resourceURI = "https://sonoma-azure-demo.azure-mobile.net/login/aad";
            string clientID = "23bff0e9-7ce7-433a-9d4f-4fb93098c0d3";
            while (user == null)
            {
                string message;
                try
                {
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
                }
                catch (InvalidOperationException)
                {
                    message = "You must log in. Login Required";
                }
                var dialog = new MessageDialog(message);
                dialog.Commands.Add(new UICommand("OK"));
                await dialog.ShowAsync();
            }
        }
    }
}
