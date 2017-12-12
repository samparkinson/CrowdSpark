using System;

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

        public async Task<IReadOnlyCollection<ProjecDTO>> GetAll()
        {
            return NotImplementedException();
        }

        public async Task<IReadOnlyCollection<ProjectDTO>> GetAllFollowed()
        {
            return NotImplementedException();
        }

        public async Task<IReadOnlyCollection<ProjectDTO>> GetAllSparked()
        {
            return NotImplementedException();
        }

        public async Task<IReadOnlyCollection<ProjectDTO>> GetBySearch(string searchString)
        {
            return NotImplementedException();
        }

        public async Task<ProjectDTO> Get(int projectID)
        {
            return NotImplementedException(); 
        }

        public async Task<bool> AddSkill(int projectID, string skill)
        {
            return NotImplementedException();
        }

    }
}