using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CrowdSpark.Common;

namespace CrowdSpark.Logic
{
    public class LocationLogic : ILocationLogic
    {
        ILocationRepository _repository;
        IUserRepository _userRepository;
        IProjectRepository _projectRepository;

        public LocationLogic(ILocationRepository repository, IUserRepository userRepository, IProjectRepository projectRepository)
        {
            _repository = repository;
            _userRepository = userRepository;
            _projectRepository = projectRepository;
        }

        public async Task<ResponseLogic> CreateAsync(LocationCreateDTO loc)
        {
            var existingLocation = await _repository.FindAsync(loc.City, loc.Country);

            if (existingLocation != null) return ResponseLogic.SUCCESS;

            var id = await _repository.CreateAsync(loc);
            if (id == 0)
            {
                return ResponseLogic.ERROR_CREATING;
            }
            else return ResponseLogic.SUCCESS;
        }

        public async Task<IEnumerable<LocationDTO>> GetAsync()
        {
            return await _repository.ReadAsync();
        }

        public async Task<LocationDTO> GetAsync(int locationId)
        {
            return await _repository.FindAsync(locationId);
        }

        public async Task<IEnumerable<LocationDTO>> FindAsync(string searchCity, string searchCountry)
        {
            return await _repository.FindWildcardAsync(searchCity, searchCountry);
        }

        public async Task<IEnumerable<LocationDTO>> FindAsync(string searchCity)
        {
            return await _repository.FindWildcardAsync(searchCity);
        }

        public async Task<LocationDTO> FindExactAsync(string searchCity, string searchCountry)
        {
            return await _repository.FindAsync(searchCity, searchCountry);         
        }
     
        public async Task<ResponseLogic> UpdateAsync(LocationDTO loc)
        {
            var foundLocation = await _repository.FindAsync(loc.Id);
            if (foundLocation is null) return ResponseLogic.NOT_FOUND;

            var success = await _repository.UpdateAsync(loc);

            if (success) return ResponseLogic.SUCCESS;
            else return ResponseLogic.ERROR_UPDATING;
        }

        public async Task<ResponseLogic> RemoveWithObjectAsync(LocationDTO loc)
        {
            var foundLocation = await _repository.FindAsync(loc.Id);
            if (foundLocation is null) return ResponseLogic.NOT_FOUND;

            var users = await _userRepository.ReadAsync();
            var projects = await _projectRepository.ReadAsync();
            var occurrences = 0;

            foreach (var user in users)
            {
                if (user.Location == loc)
                    occurrences++;
            }

            foreach (var project in projects)
            {
                if (project.LocationId == loc.Id)
                    occurrences++;
            }

            if (occurrences > 1)
            {
                return ResponseLogic.SUCCESS;
            }

            var success = await _repository.DeleteAsync(loc.Id);

            if (success) return ResponseLogic.SUCCESS;
            else return ResponseLogic.ERROR_DELETING;
        }

        public async Task<ResponseLogic> DeleteAsync(int locationId)
        {
            var foundLocation = await _repository.FindAsync(locationId);
            if (foundLocation is null) return ResponseLogic.NOT_FOUND;

            var success = await _repository.DeleteAsync(locationId);

            if (success) return ResponseLogic.SUCCESS;
            else return ResponseLogic.ERROR_DELETING;
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _repository.Dispose();
                    _userRepository.Dispose();
                    _projectRepository.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
