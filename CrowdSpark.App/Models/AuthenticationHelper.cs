using System;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web.Core;
using Windows.Security.Credentials;
using Windows.Storage;

namespace CrowdSpark.App.Models
{
    public class AuthenticationHelper : IAuthenticationHelper
    {
        private readonly ISettings _settings;
        private readonly ApplicationDataContainer _appSettings;

        public AuthenticationHelper(ISettings settings)
        {
            _appSettings = ApplicationData.Current.RoamingSettings;
            _settings = settings;
        }

        public async Task<WebAccount> SignInAsync()
        {
            var webAccountProvider = await WebAuthenticationCoreManager.FindAccountProviderAsync(_settings.WebAccountProviderId, _settings.Authority);

            var webTokenRequest = new WebTokenRequest(webAccountProvider, string.Empty, _settings.ClientId);
            webTokenRequest.Properties.Add("resource", _settings.ApiResource);

            var account = default(WebAccount);

            // Check if there's a record of the last account used with the app
            var userId = _appSettings.Values["userId"];
            if (userId != null)
            {
                // Get an account object for the user
                account = await WebAuthenticationCoreManager.FindAccountAsync(webAccountProvider, (string)userId);
                if (account != null)
                {
                    // Ensure that the saved account works for getting the token we need
                    var webTokenRequestResult = await WebAuthenticationCoreManager.RequestTokenAsync(webTokenRequest, account);
                    if (webTokenRequestResult.ResponseStatus == WebTokenRequestStatus.Success)
                    {
                        account = webTokenRequestResult.ResponseData[0].WebAccount;
                    }
                }
                else
                {
                    // The WebAccount object is no longer available. Let's attempt a sign in with the saved username
                    webTokenRequest.Properties.Add("LoginHint", _appSettings.Values["login_hint"].ToString());
                    WebTokenRequestResult webTokenRequestResult = await WebAuthenticationCoreManager.RequestTokenAsync(webTokenRequest);
                    if (webTokenRequestResult.ResponseStatus == WebTokenRequestStatus.Success)
                    {
                        account = webTokenRequestResult.ResponseData[0].WebAccount;
                    }
                }
            }
            else
            {
                // There is no recorded user. Let's start a sign in flow without imposing a specific account.                             
                webTokenRequest = new WebTokenRequest(webAccountProvider, string.Empty, _settings.ClientId, WebTokenRequestPromptType.ForceAuthentication);
                webTokenRequest.Properties.Add("resource", _settings.ApiResource);
                var webTokenRequestResult = await WebAuthenticationCoreManager.RequestTokenAsync(webTokenRequest);
                if (webTokenRequestResult.ResponseStatus == WebTokenRequestStatus.Success)
                {
                    account = webTokenRequestResult.ResponseData[0].WebAccount;
                }
            }

            if (account == null) // sign in not successfull
            {
                _appSettings.Values.Remove("userId");
                _appSettings.Values.Remove("login_hint");
            }
            else
            {
                _appSettings.Values["userId"] = account?.Id;
                _appSettings.Values["login_hint"] = account?.UserName;
            }

            return account;
        }

        public async Task SignOutAsync(WebAccount account)
        {
            _appSettings.Values.Remove("userId");
            _appSettings.Values.Remove("login_hint");

            if (account != null)
            {
                await account.SignOutAsync();
            }
        }

        public async Task<string> AcquireTokenSilentAsync()
        {
            var webAccountProvider = await WebAuthenticationCoreManager.FindAccountProviderAsync(_settings.WebAccountProviderId, _settings.Authority);

            var userId = _appSettings.Values["userId"];
            var userAccount = await WebAuthenticationCoreManager.FindAccountAsync(webAccountProvider, (string)userId);
            var webTokenRequest = new WebTokenRequest(webAccountProvider, string.Empty, _settings.ClientId);
            webTokenRequest.Properties.Add("resource", _settings.ApiResource);
            var webTokenRequestResult = await WebAuthenticationCoreManager.GetTokenSilentlyAsync(webTokenRequest, userAccount);
            if (webTokenRequestResult.ResponseStatus == WebTokenRequestStatus.Success)
            {
                _appSettings.Values["userId"] = userAccount.Id;
                _appSettings.Values["login_hint"] = userAccount.UserName;
                return webTokenRequestResult.ResponseData[0].Token;
            }

            return null;
        }

        public async Task<WebAccount> GetAccountAsync()
        {
            var webAccountProvider = await WebAuthenticationCoreManager.FindAccountProviderAsync(_settings.WebAccountProviderId, _settings.Authority);

            var userId = _appSettings.Values["userId"];

            if (string.IsNullOrWhiteSpace(userId as string))
            {
                return null;
            }

            return await WebAuthenticationCoreManager.FindAccountAsync(webAccountProvider, (string)userId);
        }
    }
}
