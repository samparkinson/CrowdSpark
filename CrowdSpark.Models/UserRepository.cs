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

        public async Task<int> CreateAsync(UserCreateDTO user, string azureUIDd)
        {
            var userToCreate = new User
            {
                Firstname = user.Firstname,
                Surname = user.Surname,
                Mail = user.Mail,
                LocationId = user.Location?.Id,
                Skills = EntityConversionHelper.ConvertSkillDTOsToSkills(user.Skills),
                AzureUId = azureUIDd
            };

            _context.Users.Add(userToCreate);
            if (await saveContextChanges() > 0)
            {
                return userToCreate.Id;
            }
            else throw new DbUpdateException("Error creating user", (Exception)null);
        }

        public async Task<bool> DeleteAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user is null) return false;
            _context.Users.Remove(user);

            return ( await saveContextChanges() > 0 );
        }

        public async Task<UserDTO> FindAsync(int userId)
        {
            var user =  await _context.Users.FindAsync(userId);

            if (user is null) return null;

            return new UserDTO
            {
                Id = user.Id,
                Firstname = user.Firstname,
                Surname = user.Surname,
                Mail = user.Mail,
                Location = (user.Location == null) ? null : new LocationDTO() { Id = user.Location.Id, City = user.Location.City, Country = user.Location.Country },
                Skills = EntityConversionHelper.ConvertSkillsToSkillDTOs(user.Skills)
            };
        }

        public async Task<int> GetIdAsync(string azureUId)
        {
            var user = await _context.Users.Where(u => u.AzureUId == azureUId).FirstOrDefaultAsync();

            if (user is null) return -1;

            return user.Id;
        }

        public async Task<UserDTO> FindFromAzureUIdAsync(string azureUId)
        {
            var user = await _context.Users.Where(u => u.AzureUId == azureUId).FirstOrDefaultAsync();

            if (user is null) return null;

            return new UserDTO
            {
                Id = user.Id,
                Firstname = user.Firstname,
                Surname = user.Surname,
                Mail = user.Mail,
                Location = (user.Location == null) ? null : new LocationDTO() { Id = user.Location.Id, City = user.Location.City, Country = user.Location.Country },
                Skills = EntityConversionHelper.ConvertSkillsToSkillDTOs(user.Skills)
            };
        }

        public async Task<IReadOnlyCollection<UserDTO>> ReadAsync()
        {
            var users = from u in _context.Users
                           select new UserDTO
                           {
                               Id = u.Id,
                               Firstname = u.Firstname,
                               Surname = u.Surname,
                               Mail = u.Mail,
                               Location = (u.Location == null) ? null : new LocationDTO() { Id = u.Location.Id, City = u.Location.City, Country = u.Location.Country },
                               Skills = EntityConversionHelper.ConvertSkillsToSkillDTOs(u.Skills)
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
            userToUpdate.LocationId = user.Location?.Id;
            userToUpdate.Location = (user.Location == null) ? null : new Location() { Id = user.Location.Id, City = user.Location.City, Country = user.Location.Country };
            userToUpdate.Skills = EntityConversionHelper.ConvertSkillDTOsToSkills(user.Skills);    

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
                throw new DbUpdateException("Error modifying user collection", e);
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
