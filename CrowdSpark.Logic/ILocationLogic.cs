using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CrowdSpark.Common;

namespace CrowdSpark.Logic
{
    public interface ILocationLogic : IDisposable
    {
        Task<IEnumerable<LocationDTO>> GetAsync();

        Task<LocationDTO> GetAsync(int locationId);

        Task<IEnumerable<LocationDTO>> FindAsync(string searchCity, string searchCountry);

        Task<LocationDTO> FindExactAsync(string searchCity, string searchCountry);

        Task<ResponseLogic> CreateAsync(LocationCreateDTO loc);

        Task<ResponseLogic> UpdateAsync(LocationDTO loc);

        Task<ResponseLogic> RemoveWithObjectAsync(LocationDTO loc);

        Task<ResponseLogic> DeleteAsync(int locationId);
    }
}
