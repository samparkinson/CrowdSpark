using CrowdSpark.Entitites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrowdSpark.Common
{
    public interface ILocationRepository : IDisposable
    {
        Task<int> CreateAsync(LocationDTO location);
     
        Task<Location> FindAsync(int locationId);

        Task<Location> FindAsync(string searchCity, string searchCountry);

        Task<IEnumerable<Location>> FindWildcardAsync(string city, string country);

        Task<IEnumerable<Location>> FindWildcardAsync(string city);

        Task<IReadOnlyCollection<Location>> ReadAsync();

        Task<bool> UpdateAsync(Location details);

        Task<bool> DeleteAsync(int locationId);
    }
}
