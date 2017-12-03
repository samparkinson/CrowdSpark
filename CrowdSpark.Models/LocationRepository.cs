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

        public async Task<int> CreateAsync(Location location)
        {
            _context.Location.Add(location);
            if (await _context.SaveChangesAsync() > 0)
            {
                return location.Id;
            }
            else throw new DbUpdateException("Error creating location", (Exception)null);
        }

        public async Task<bool> DeleteAsync(int locationId)
        {
            var location = await _context.Location.FindAsync(locationId);
            _context.Location.Remove(location);

            return ( await _context.SaveChangesAsync() > 0 );
        }

        public async Task<Location> FindAsync(int locationId)
        {
            return await _context.Location.FindAsync(locationId);
        }

        public async Task<IReadOnlyCollection<Location>> ReadAsync()
        {
            return await _context.Location.ToListAsync();
        }

        public async Task<bool> UpdateAsync(Location details)
        {
            var locationToUpdate = await _context.Location.FindAsync(details.Id);
            _context.Location.Update(locationToUpdate);

            locationToUpdate.City = details.City;
            locationToUpdate.Country = details.Country;

            return (await _context.SaveChangesAsync() > 0);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
