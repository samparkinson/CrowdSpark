using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using CrowdSpark.Common;

namespace CrowdSpark.App.Models
{
    public class LocationAPI : ILocationAPI
    {
        private readonly HttpClient _client;

        public LocationAPI(ISettings settings, DelegatingHandler handler)
        {
            var client = new HttpClient(handler)
            {
                BaseAddress = settings.ApiBaseAddress
            };
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            _client = client;
        }

        public async Task<int> Create(LocationCreateDTO location)
        {
            var response = await _client.PostAsync("api/v1/locations", location.ToHttpContent());
            var newLocationId = response.Content.To<int>().Result;
            return response.IsSuccessStatusCode ? newLocationId : -1;
        }

        public async Task<LocationDTO> Get(int locationId)
        {
            var response = await _client.GetAsync($"api/v1/locations/{locationId}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.To<LocationDTO>();
            }

            return null;
        }

        public async Task<IReadOnlyCollection<LocationDTO>> GetAll()
        {
            var response = await _client.GetAsync("api/v1/locations");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.To<IReadOnlyCollection<LocationDTO>>();
            }

            return new List<LocationDTO>().AsReadOnly();
        }

        public async Task<IReadOnlyCollection<LocationDTO>> GetForCity(string city)
        {
            var response = await _client.GetAsync($"api/v1/locations?city={city}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.To<IReadOnlyCollection<LocationDTO>>();
            }

            return new List<LocationDTO>().AsReadOnly();
        }

        public async Task<IReadOnlyCollection<LocationDTO>> GetForCountry(string country)
        {
            var response = await _client.GetAsync($"api/v1/locations?country={country}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.To<IReadOnlyCollection<LocationDTO>>();
            }

            return new List<LocationDTO>().AsReadOnly();
        }

        public async Task<IReadOnlyCollection<LocationDTO>> GetForCityAndCountry(string city, string country)
        {
            var response = await _client.GetAsync($"api/v1/locations?city={city}&country={country}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.To<IReadOnlyCollection<LocationDTO>>();
            }

            return new List<LocationDTO>().AsReadOnly();
        }

       

        public async Task<bool> Update(LocationDTO location)
        {
            var response = await _client.PutAsync($"api/v1/locations", location.ToHttpContent());

            return response.IsSuccessStatusCode;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _client.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~LocationAPI() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion


    }
}
