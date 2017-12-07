using System;
using System.Collections.Generic;
using System;
using System.Globalization;
using Windows.Security.Authentication.Web;

namespace CrowdSpark.App.Models
{
    public class Settings : ISettings
    {

        public string Tenant => "ituniversity.onmicrosoft.com";

        public string ClientId => "";

        public string RedirectUri => $"ms-appx-web://Microsoft.AAD.BrokerPlugIn/{WebAuthenticationBroker.GetCurrentApplicationCallbackUri().Host.ToUpper()}";

        public string Instance => "https://login.microsoftonline.com/";

        public string WebAccountProviderId => "https://login.microsoft.com";

        public string ApiResource => "https://ituniversity.onmicrosoft.com/crowdspark";

        public Uri ApiBaseAddress => new Uri("https://crowdsparkwebapp.azurewebsites.net/");

        public string Authority => $"{Instance}{Tenant}";
    }
}
