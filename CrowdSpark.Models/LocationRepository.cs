using System;
using CrowdSpark.Entitites;
using CrowdSpark.Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CrowdSpark.Models
{
    public class LocationRepository : ILocationRepository
    {
        private readonly ICrowdSparkContext _context;

        public LocationRepository(ICrowdSparkContext context)
        {
            _context = context;
        }

        public async Task<int> CreateAsync(LocationCreateDTO location)
        {
            var locationToCreate = new Location
            {
                City = location.City,
                Country = location.Country
            };

            _context.Locations.Add(locationToCreate);
            if (await saveContextChanges() > 0)
            {
                return locationToCreate.Id;
            }
            else throw new DbUpdateException("Error creating location", (Exception)null);
        }

        public async Task<IEnumerable<LocationDTO>> FindWildcardAsync(string city, string country)
        {
            return await _context.Locations.Where(l => l.City.ToLower().Contains(city.ToLower())
                                                  && l.Country.ToLower().Contains(country.ToLower()))
                                                  .Select(l => new LocationDTO() { Id = l.Id, City = l.City, Country = l.Country})
                                                  .ToArrayAsync();
        }

        public async Task<IEnumerable<LocationDTO>> FindWildcardAsync(string city)
        {
            return await _context.Locations.Where(l => l.City.ToLower().Contains(city.ToLower()))
                            .Select(l => new LocationDTO() { Id = l.Id, City = l.City, Country = l.Country })
                            .ToArrayAsync();
        }

        public async Task<bool> DeleteAsync(int locationId)
        {
            var location = await _context.Locations.FindAsync(locationId);
            if (location is null) return false;
            _context.Locations.Remove(location);

            return (await saveContextChanges() > 0);
        }

        public async Task<LocationDTO> FindAsync(int locationId)
        {
            var location = await _context.Locations.FindAsync(locationId);

            return new LocationDTO() { Id = location.Id, City = location.City, Country = location.Country };
        }

        public async Task<LocationDTO> FindAsync(string searchCity, string searchCountry)
        {
            var location =  await _context.Locations.FindAsync(searchCity, searchCountry);

            return new LocationDTO() { Id = location.Id, City = location.City, Country = location.Country };
        }

        public async Task<IReadOnlyCollection<LocationDTO>> ReadOrderedAsync()
        {
            return await _context.Locations.OrderBy(item => item.Country )
                .ThenBy(n => n.City)
                .Select(l => new LocationDTO() { Id = l.Id, City = l.City, Country = l.Country })
                .ToListAsync();
        }

        public async Task<IReadOnlyCollection<LocationDTO>> ReadAsync()
        {
            return await _context.Locations
                            .Select(l => new LocationDTO() { Id = l.Id, City = l.City, Country = l.Country })
                            .ToListAsync();
        }

        public async Task<bool> UpdateAsync(LocationDTO details)
        {
            var locationToUpdate = await _context.Locations.FindAsync(details.Id);
            _context.Locations.Update(locationToUpdate);

            locationToUpdate.City = details.City;
            locationToUpdate.Country = details.Country;

            return (await saveContextChanges() > 0);
        }

        async Task<int> saveContextChanges()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (System.Data.DataException e)
            {
                throw new DbUpdateException("Error modifying location collection", e);
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
