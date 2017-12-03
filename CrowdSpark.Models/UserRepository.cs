using System;
using CrowdSpark.Entitites;
using CrowdSpark.Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CrowdSpark.Models
{
    public class UserRepository : IUserRepository
    {
        private readonly ICrowdSparkContext _context;

        public UserRepository(ICrowdSparkContext context)
        {
            _context = context;
        }

        public Task<int> CreateAsync(UserDTO user)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteAsync(int userId)
        {
            var user = await _context.User.FindAsync(userId);
            _context.User.Remove(user);

            return ( await _context.SaveChangesAsync() > 0 );
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public async Task<UserDTO> FindAsync(int userId)
        {
            var user =  await _context.User.FindAsync(userId);

            return new UserDTO
            {
                Firstname = user.Firstname,
                Surname = user.Surname,
                Mail = user.Mail,
                Location = user.Location,
                Skills = user.Skills
            };
        }

        public async Task<IReadOnlyCollection<UserDTO>> ReadAsync()
        {
            var users = from u in _context.User
                           select new UserDTO
                           {
                               Firstname = u.Firstname,
                               Surname = u.Surname,
                               Mail = u.Mail,
                               Location = u.Location,
                               Skills = u.Skills
                           };

            return await users.ToListAsync();
        }

        public Task<bool> UpdateAsync(UserDTO details)
        {
            throw new NotImplementedException();
        }
    }
}
