using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CrowdSpark.App
{
    public interface IHttpHandler : IDisposable
    {
        Task<HttpResponseMessage> GetAsync(string url);
        Task<HttpResponseMessage> PostAsync(string url, HttpContent content);
        Task<HttpResponseMessage> PutAsync(string url, HttpContent content);
        Task<HttpResponseMessage> DeleteAsync(string url);
        Uri BaseAddress { get; set; }
        HttpRequestHeaders DefaultRequestHeaders { get; }

    }
}
