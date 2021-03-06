﻿using CrowdSpark.Common;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CrowdSpark.App.Models
{
    public class UserAPI : IUserAPI
    {
        private readonly HttpClient _client;

        public UserAPI(ISettings settings, DelegatingHandler handler)
        {
            var client = new HttpClient(handler)
            {
                BaseAddress = settings.ApiBaseAddress
            };
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            _client = client;
        }

        public async Task<UserDTO> GetMyself()
        {
            var response = await _client.GetAsync($"api/v1/users");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.To<UserDTO>();              
            }

            return null;
        }

        public async Task<UserDTO> Get(int userId)
        {
            var response = await _client.GetAsync($"api/v1/users/{userId}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.To<UserDTO>();
            }

            return null;
        }

        public async Task<int> Create(UserCreateDTO user)
        {
            var response = await _client.PostAsync("api/v1/users", user.ToHttpContent());
            var newUserId = response.Content.To<int>().Result;
            return response.IsSuccessStatusCode ? newUserId : -1;
        }

        public async Task<bool> Update(UserDTO user)
        {
            var response = await _client.PutAsync("api/v1/users", user.ToHttpContent());

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> AddSkill(SkillDTO skill)
        {
            var response = await _client.PostAsync("api/v1/users/skills", skill.ToHttpContent());

            return response.IsSuccessStatusCode;
        }

        public async Task<IReadOnlyCollection<SkillDTO>> GetSkills()
        {
            var response = await _client.GetAsync($"api/v1/users/skills");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.To<IReadOnlyCollection<SkillDTO>>();
            }

            return new List<SkillDTO>().AsReadOnly();
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
        // ~UserAPI() {
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
