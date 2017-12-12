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

        Task<IReadOnlyCollection<ProjectDTO>> IProjectAPI.GetAll()
        {
            throw new NotImplementedException();
        }

        Task<IReadOnlyCollection<ProjectDTO>> IProjectAPI.GetAllFollowed()
        {
            throw new NotImplementedException();
        }

        Task<IReadOnlyCollection<ProjectDTO>> IProjectAPI.GetAllSparked()
        {
            throw new NotImplementedException();
        }

        Task<IReadOnlyCollection<ProjectDTO>> IProjectAPI.GetBySearch(string searchString)
        {
            throw new NotImplementedException();
        }

        Task<ProjectDTO> IProjectAPI.Get(int projectID)
        {
            throw new NotImplementedException();
        }

        Task<bool> IProjectAPI.AddSkill(int projectID, string skill)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

    }
}