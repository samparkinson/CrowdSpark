using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CrowdSpark.Common;

namespace CrowdSpark.App.Models
{
    public class ProjectAPI : IProjectAPI
    {
        private readonly HttpClient _client;

        public ProjectAPI(ISettings settings, DelegatingHandler handler)
        {
            var client = new HttpClient(handler)
            {
                BaseAddress = settings.ApiBaseAddress
            };
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            _client = client;
        }

        public async Task<IReadOnlyCollection<ProjectSummaryDTO>> GetAll()
        {
            var response = await _client.GetAsync("api/projects");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.To<IReadOnlyCollection<ProjectSummaryDTO>>();
            }

            return new List<ProjectSummaryDTO>().AsReadOnly();
        }

        public async Task<IReadOnlyCollection<ProjectSummaryDTO>> GetBySearch(string searchString)
        {
            var response = await _client.GetAsync($"api/projects?search={searchString}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.To<IReadOnlyCollection<ProjectSummaryDTO>>();
            }

            return new List<ProjectSummaryDTO>().AsReadOnly();
        }

        public async Task<IReadOnlyCollection<ProjectSummaryDTO>> GetByCategory(int categoryID)
        {
            var response = await _client.GetAsync($"api/projects?category={categoryID}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.To<IReadOnlyCollection<ProjectSummaryDTO>>();
            }

            return new List<ProjectSummaryDTO>().AsReadOnly();
        }

        public async Task<ProjectDTO> Get(int projectId)
        {
            var response = await _client.GetAsync($"api/projects/{projectId}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.To<ProjectDTO>();
            }

            return null;
        }

        public async Task<bool> AddSkill(int projectID, string skill)
        {
            throw new NotImplementedException();
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
        // ~ProjectAPI() {
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