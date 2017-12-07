using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace CrowdSpark.App.Models
{
    public class AuthorizedHandler : DelegatingHandler
    {
        private readonly IAuthenticationHelper _helper;

        public AuthorizedHandler(IAuthenticationHelper helper)
        {
            InnerHandler = new HttpClientHandler();

            _helper = helper;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _helper.AcquireTokenSilentAsync();

            if (string.IsNullOrWhiteSpace(token))
            {
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
