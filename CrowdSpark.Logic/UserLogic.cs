using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrowdSpark.Common;

namespace CrowdSpark.Logic
{
    public class UserLogic : IUserLogic
    {
        private readonly IUserRepository _repository;
        private readonly ISparkLogic _sparkLogic;
        private readonly ISkillLogic _skillLogic;
        private readonly ILocationLogic _locationLogic;

        public UserLogic(IUserRepository repository, ISkillLogic skillLogic, ISparkLogic sparkLogic, ILocationLogic locationLogic)
        {
            _repository = repository;
            _skillLogic = skillLogic;
            _sparkLogic = sparkLogic;
            _locationLogic = locationLogic;
        }

        public async Task<IEnumerable<UserDTO>> GetAsync()
        {
            return await _repository.ReadAsync();
        }

        public async Task<UserDTO> GetAsync(int userId)
        {
            return await _repository.FindAsync(userId);
        }

        public async Task<int> GetIdFromAzureUIdAsync(string azureUId)
        {
            return await _repository.GetIdAsync(azureUId);
        }

        public async Task<ResponseLogic> CreateAsync(UserCreateDTO user, string azureUId)
        {
            var existing = await _repository.FindFromAzureUIdAsync(azureUId);

            if (existing != null)
            {
                return ResponseLogic.ALREADY_EXISTS;
            }

            if (user.Location != null)
            {
                var success = await _locationLogic.CreateAsync(new LocationCreateDTO() { City = user.Location.City, Country = user.Location.Country });
                if (success == ResponseLogic.SUCCESS)
                {
                    user.Location = await _locationLogic.FindExactAsync(user.Location.City, user.Location.Country);
                }
                else return ResponseLogic.ERROR_CREATING;
            }

            var id = await _repository.CreateAsync(user, azureUId);

            if (id == 0)
            {
                return ResponseLogic.ERROR_CREATING;
            }

            return ResponseLogic.SUCCESS;
        }

        public async Task<ResponseLogic> UpdateAsync(int userId, UserDTO user)
        {
            var currentUser = await _repository.FindAsync(userId);

            if (currentUser is null)
            {
                return ResponseLogic.NOT_FOUND;
            }

            var currentSkills = (currentUser.Skills == null) ? new SkillDTO[] { } : currentUser.Skills;
            var skills = (user.Skills == null) ? new SkillDTO[] { } : user.Skills;

            var skillsToAdd = skills.Where(s => !currentSkills.Contains(s));
            var skillsToRemove = currentSkills.Where(s => !skills.Contains(s));

            foreach (var skill in skillsToAdd)
            {
                await _skillLogic.CreateAsync(new SkillCreateDTO() { Name = skill.Name });
            }
            foreach (var skill in skillsToRemove)
            {
                await _skillLogic.RemoveWithObjectAsync(skill);
            }
            
            var success = await _repository.UpdateAsync(userId, user);

            if (success)
            {
                return ResponseLogic.SUCCESS;
            }

            // roll back skill changes 
            foreach (var skill in skillsToAdd)
            {
                await _skillLogic.RemoveWithObjectAsync(skill);
            }
            foreach (var skill in skillsToRemove)
            {
                await _skillLogic.CreateAsync(new SkillCreateDTO() { Name = skill.Name });
            }

            return ResponseLogic.ERROR_UPDATING;

        }

        public async Task<ResponseLogic> DeleteAsync(int userId)
        {
            var user = await _repository.FindAsync(userId);

            if (user is null)
            {
                return ResponseLogic.NOT_FOUND;
            }

            foreach (var skill in user.Skills)
            {
                await _skillLogic.RemoveWithObjectAsync(skill);
            }

            await _sparkLogic.DeleteForUserAsync(userId);

            var success = await _repository.DeleteAsync(userId);

            if (success)
            {
                return ResponseLogic.SUCCESS;
            }
            else return ResponseLogic.ERROR_DELETING;
        }

        public async Task<ResponseLogic> AddSkillAsync(int userId, SkillDTO skill)
        {
            var user = await _repository.FindAsync(userId);

            if (user.Skills.Contains(skill))
            {
                return ResponseLogic.SUCCESS;
            }

            user.Skills.Add(skill);

            return await UpdateAsync(userId, user);
        }

        public async Task<ResponseLogic> AddSkillAsync(int userId, int skillId)
        {
            var user = await _repository.FindAsync(userId);
            var skill = await _skillLogic.GetAsync(skillId);

            if (user.Skills.Contains(skill))
            {
                return ResponseLogic.SUCCESS;
            }

            if (skill != null )user.Skills.Add(skill);

            return await UpdateAsync(userId, user);
        }

        public async Task<ResponseLogic> RemoveSkillAsync(int userId, SkillDTO skill)
        {
            var user = await _repository.FindAsync(userId);

            if (!user.Skills.Contains(skill))
            {
                return ResponseLogic.SUCCESS;
            }

            user.Skills.Remove(skill);

            return await UpdateAsync(userId, user);
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _repository.Dispose();
                    _sparkLogic.Dispose();
                    _skillLogic.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
