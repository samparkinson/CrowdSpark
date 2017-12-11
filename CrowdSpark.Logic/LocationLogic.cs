using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CrowdSpark.Common;
using CrowdSpark.Entitites;

namespace CrowdSpark.Logic
{
    public class LocationLogic : ILocationLogic
    {
        public async Task<ResponseLogic> CreateAsync(Location skill)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseLogic> DeleteAsync(int locationId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Location>> FindAsync(string city, string country)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Location>> FindAsync(string city)
        {
            throw new NotImplementedException();
        }

        public async Task<Location> FindExactAsync(string city, string country)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Location>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<Location> GetAsync(int locationId)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseLogic> UpdateAsync(Location skill)
        {
            throw new NotImplementedException();
        }
    }
}
