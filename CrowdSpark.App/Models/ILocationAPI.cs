using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CrowdSpark.Common;

namespace CrowdSpark.App.Models
{
    public interface ILocationAPI : IDisposable
    {
        Task<IReadOnlyCollection<LocationDTO>> GetAll();

        Task<LocationDTO> Get(int locationId);

        Task<IReadOnlyCollection<LocationDTO>> GetForCity(string city);

        Task<IReadOnlyCollection<LocationDTO>> GetForCountry(string country);

        Task<IReadOnlyCollection<LocationDTO>> GetForCityAndCountry(string city, string country);

        Task<int> Create(LocationCreateDTO location);

        Task<bool> Update(LocationDTO location);
    }
}
