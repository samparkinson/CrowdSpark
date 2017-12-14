using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CrowdSpark.App.Models
{
    public static class Extensions
    {
        public static HttpContent ToHttpContent<T>(this T obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            var content = new StringContent(json);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return content;
        }

        public static async Task<T> To<T>(this HttpContent content)
        {
            var json = await content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
