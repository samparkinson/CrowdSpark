using CrowdSpark.Entitites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrowdSpark.Common
{
    public interface ILocationRepository : IDisposable
    {
        Task<int> CreateAsync(LocationCreateDTO location);
     
        Task<LocationDTO> FindAsync(int locationId);

        Task<LocationDTO> FindAsync(string searchCity, string searchCountry);

        Task<IEnumerable<LocationDTO>> FindWildcardAsync(string city, string country);

        Task<IEnumerable<LocationDTO>> FindWildcardAsync(string city);

        Task<IReadOnlyCollection<LocationDTO>> ReadAsync();

        Task<bool> UpdateAsync(LocationDTO details);

        Task<bool> DeleteAsync(int locationId);
    }
}
