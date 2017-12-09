﻿using System;
using Windows.Security.Authentication.Web;

namespace CrowdSpark.App.Models
{
    public class Settings : ISettings
    {

        public string Tenant => "ituniversity.onmicrosoft.com";

        public string ClientId => "3b85a201-bf78-4d99-96f3-a605f0b7ed88";

        public string RedirectUri => $"ms-appx-web://Microsoft.AAD.BrokerPlugIn/{WebAuthenticationBroker.GetCurrentApplicationCallbackUri().Host.ToUpper()}";

        public string Instance => "https://login.microsoftonline.com/";

        public string WebAccountProviderId => "https://login.microsoft.com";

        public string ApiResource => "https://ituniversity.onmicrosoft.com/crowdspark.Web";

        public Uri ApiBaseAddress => new Uri("https://crowdsparkwebapp.azurewebsites.net/");

        public string Authority => $"{Instance}{Tenant}";
    }
}
