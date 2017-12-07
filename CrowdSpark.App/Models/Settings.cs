using System;
using System.Collections.Generic;
using System;
using System.Globalization;
using Windows.Security.Authentication.Web;

namespace CrowdSpark.App.Models
{
    public class Settings : ISettings
    {
<<<<<<< HEAD
        public string Tenant => "ituniversity.onmicrosoft.com";

        public string ClientId => "";
=======
        public string Tenant => "ondfisk.onmicrosoft.com";

        public string ClientId => "48ad45dd-7c9c-4e94-b2e2-c3b09aae44a3";
>>>>>>> 504abfb18d37a3fdad2b8fea19b6d718fd00cf49

        public string RedirectUri => $"ms-appx-web://Microsoft.AAD.BrokerPlugIn/{WebAuthenticationBroker.GetCurrentApplicationCallbackUri().Host.ToUpper()}";

        public string Instance => "https://login.microsoftonline.com/";

        public string WebAccountProviderId => "https://login.microsoft.com";

<<<<<<< HEAD
        public string ApiResource => "https://ituniversity.onmicrosoft.com/crowdspark";

        public Uri ApiBaseAddress => new Uri("https://crowdsparkwebapp.azurewebsites.net/");
=======
        public string ApiResource => "https://ondfisk.onmicrosoft.com/BDSA2017.Lecture11.Web";

        public Uri ApiBaseAddress => new Uri("https://bdsa2017.azurewebsites.net/");
>>>>>>> 504abfb18d37a3fdad2b8fea19b6d718fd00cf49

        public string Authority => $"{Instance}{Tenant}";
    }
}
