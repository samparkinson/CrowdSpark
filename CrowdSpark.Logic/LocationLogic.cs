using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CrowdSpark.Common;
using CrowdSpark.Entitites;

namespace CrowdSpark.Logic
{
    public class LocationLogic : ILocationLogic
    {
        ILocationRepository _repository;

        public LocationLogic(ILocationRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Location>> GetAsync()
        {
            return await _repository.ReadAsync();
        }

        public async Task<Location> GetAsync(int locationId)
        {
            return await _repository.FindAsync(locationId);
        }

        public async Task<IEnumerable<Location>> FindAsync(string searchCity, string searchCountry)
        {
            return await _repository.FindWildcardAsync(searchCity, searchCountry);
        }

        public async Task<IEnumerable<Location>> FindAsync(string searchCity)
        {
            return await _repository.FindWildcardAsync(searchCity);
        }

        public async Task<Location> FindExactAsync(string searchCity, string searchCountry)
        {
            return await _repository.FindAsync(searchCity, searchCountry);         
        }

        public async Task<ResponseLogic> CreateAsync(LocationDTO loc)
        {
            //      var id = await _repository.CreateAsync(loc);   
            return ResponseLogic.SUCCESS;    
        }
     
        public async Task<ResponseLogic> UpdateAsync(Location loc)
        {
            return ResponseLogic.SUCCESS;
        }

        public async Task<ResponseLogic> RemoveAsync(Location loc)
        {
            return ResponseLogic.SUCCESS;
        }

        public async Task<ResponseLogic> DeleteAsync(int locationId)
        {
            return ResponseLogic.SUCCESS;
        }

    }
}
