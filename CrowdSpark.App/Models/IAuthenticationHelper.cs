using System.Threading.Tasks;
using Windows.Security.Credentials;

namespace CrowdSpark.App.Models
{
    public interface IAuthenticationHelper
    {
        Task<WebAccount> SignInAsync();
        Task SignOutAsync(WebAccount account);
        Task<string> AcquireTokenSilentAsync();
        Task<WebAccount> GetAccountAsync();
    }
}