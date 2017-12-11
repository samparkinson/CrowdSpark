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

        Task<IEnumerable<Location>> FindAsync(string city, string country);

        Task<IEnumerable<Location>> FindAsync(string city);

        Task<Location> FindExactAsync(string city, string country);

        Task<ResponseLogic> CreateAsync(Location location);

        Task<ResponseLogic> UpdateAsync(Location location);

        Task<ResponseLogic> DeleteAsync(int locationId);
    }
}
