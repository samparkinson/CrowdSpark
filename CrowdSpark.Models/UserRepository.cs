using System;
using CrowdSpark.Entitites;
using CrowdSpark.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrowdSpark.Models
{
    public class UserRepository : IUserRepository
    {
        private readonly ICrowdSparkContext _context;

        public Task<int> CreateAsync(UserDTO user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<UserDTO> FindAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<UserDTO>> ReadAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(UserDTO details)
        {
            throw new NotImplementedException();
        }
    }
}
