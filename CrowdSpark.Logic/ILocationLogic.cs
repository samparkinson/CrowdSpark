using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CrowdSpark.Common;
using CrowdSpark.Entitites;

namespace CrowdSpark.Logic
{
    public interface ILocationLogic
    {
        Task<IEnumerable<Location>> GetAsync();

        Task<Location> GetAsync(int locationId);

        Task<IEnumerable<Location>> FindAsync(string searchCity, string searchCountry);

        Task<Location> FindExactAsync(string searchCity, string searchCountry);

        Task<ResponseLogic> CreateAsync(LocationDTO loc);

        Task<ResponseLogic> UpdateAsync(Location loc);

        Task<ResponseLogic> RemoveAsync(Location loc);

        Task<ResponseLogic> DeleteAsync(int locationId);
    }
}
