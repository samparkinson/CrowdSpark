using CrowdSpark.Entitites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrowdSpark.Common
{
    public interface ILocationRepository : IDisposable
    {
        Task<int> CreateAsync(Location skill);
     
        Task<Location> FindAsync(int locationId);

        Task<IReadOnlyCollection<Location>> ReadAsync();

        Task<bool> UpdateAsync(Location details);

        Task<bool> DeleteAsync(int locationId);
    }
}
