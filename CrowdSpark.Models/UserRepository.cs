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

        public async Task<int> CreateAsync(UserDTO user)
        {
            var userToCreate = new User
            {
                Firstname = user.Firstname,
                Surname = user.Surname,
                Mail = user.Mail,
                LocationId = user.Location?.Id,
                Skills = user.Skills
            };

            _context.Users.Add(userToCreate);
            if (await _context.SaveChangesAsync() > 0)
            {
                return userToCreate.Id;
            }
            else throw new DbUpdateException("Error creating user", (Exception)null);
        }

        public async Task<bool> DeleteAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            _context.Users.Remove(user);

            return ( await _context.SaveChangesAsync() > 0 );
        }

        public async Task<UserDTO> FindAsync(int userId)
        {
            var user =  await _context.Users.FindAsync(userId);

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
            var users = from u in _context.Users
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

        public async Task<bool> UpdateAsync(int userId, UserDTO user)
        {
            var userToUpdate = await _context.Users.FindAsync(userId);
            _context.Users.Update(userToUpdate);

            userToUpdate.Firstname = user.Firstname;
            userToUpdate.Surname = user.Surname;
            userToUpdate.Mail = user.Mail;
            userToUpdate.LocationId = user.Location.Id;
            userToUpdate.Location = user.Location;
            userToUpdate.Skills = user.Skills;    

            return (await _context.SaveChangesAsync() > 0);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
