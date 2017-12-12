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

        public async Task<int> CreateAsync(LocationDTO location)
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

        public async Task<IEnumerable<Location>> FindWildcardAsync(string city, string country)
        {
            return await _context.Locations.Where(l => l.City.ToLower().Contains(city.ToLower())
                                                  && l.Country.ToLower().Contains(country.ToLower())).ToArrayAsync();
        }

        public async Task<IEnumerable<Location>> FindWildcardAsync(string city)
        {
            return await _context.Locations.Where(l => l.City.ToLower().Contains(city.ToLower())).ToArrayAsync();
        }

        public async Task<bool> DeleteAsync(int locationId)
        {
            var location = await _context.Locations.FindAsync(locationId);
            _context.Locations.Remove(location);

            return (await saveContextChanges() > 0);
        }

        public async Task<Location> FindAsync(int locationId)
        {
            return await _context.Locations.FindAsync(locationId);
        }

        public async Task<Location> FindAsync(string searchCity, string searchCountry)
        {
            return await _context.Locations.FindAsync(searchCity, searchCountry);
        }

        public async Task<IReadOnlyCollection<Location>> ReadAsync()
        {
            return await _context.Locations.ToListAsync();
        }

        public async Task<bool> UpdateAsync(Location details)
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
