using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrowdSpark.Common;
using CrowdSpark.Entitites;

namespace CrowdSpark.Logic
{
    public class UserLogic : IUserLogic
    {
        private readonly IUserRepository _repository;
        private readonly ISkillLogic _skillLogic;

        public UserLogic(IUserRepository repository, ISkillLogic skillLogic)
        {
            _repository = repository;
            _skillLogic = skillLogic;
        }

        public async Task<IEnumerable<UserDTO>> GetUsersAsync()
        {
            return await _repository.ReadAsync();
        }

        public async Task<UserDTO> GetUserAsync(int userId)
        {
            return await _repository.FindAsync(userId);
        }

        public async Task<ResponseLogic> CreateUserAsync(UserDTO user)
        {
            var skills = user.Skills;

            var id = await _repository.CreateAsync(user);

            foreach (var skill in skills)
            {
                await _skillLogic.CreateSkillAsync(skill); //TODO, need to convert this to a parralle for each
            }

            if (id == 0)
            {
                return ResponseLogic.ERROR_CREATING;
            }
            else return ResponseLogic.SUCCESS;
        }

        public async Task<ResponseLogic> UpdateUserAsync(int userId, UserDTO user)
        {
            var currentUser = await _repository.FindAsync(userId);

            if (currentUser is null)
            {
                return ResponseLogic.NOT_FOUND;
            }

            var currentSkills = currentUser.Skills;
            var skills = user.Skills;

            var skillsToAdd = skills.Where(s => !currentSkills.Contains(s));
            var skillsToRemove = currentSkills.Where(s => !skills.Contains(s));

            foreach (var skill in skillsToAdd)
            {
                await _skillLogic.CreateSkillAsync(skill); //TODO, need to convert this to a parralle for each
            }
            foreach (var skill in skillsToRemove)
            {
                await _skillLogic.RemoveSkillAsync(skill); //TODO, need to convert this to a parralle for each
            }
            
            var success = await _repository.UpdateAsync(userId, user);

            if (success)
            {
                return ResponseLogic.SUCCESS;
            }
            else
            {
                // roll back skill changes 
                foreach (var skill in skillsToAdd)
                {
                    await _skillLogic.RemoveSkillAsync(skill); //TODO, need to convert this to a parralle for each
                }
                foreach (var skill in skillsToRemove)
                {
                    await _skillLogic.CreateSkillAsync(skill); //TODO, need to convert this to a parralle for each
                }

                return ResponseLogic.ERROR_UPDATING;
            }
        }

        public async Task<ResponseLogic> DeleteUserAsync(int userId)
        {
            var user = await _repository.FindAsync(userId);

            if (user is null)
            {
                return ResponseLogic.NOT_FOUND;
            }

            foreach (var skill in user.Skills)
            {
                await _skillLogic.RemoveSkillAsync(skill);
            } 

            var success = await _repository.DeleteAsync(userId);

            if (success)
            {
                return ResponseLogic.SUCCESS;
            }
            else return ResponseLogic.ERROR_DELETING;
        }

        public async Task<ResponseLogic> AddUserSkillAsync(int userId, Skill skill)
        {
            var user = await _repository.FindAsync(userId);

            if (user.Skills.Contains(skill))
            {
                return ResponseLogic.SUCCESS;
            }

            user.Skills.Add(skill);

            return await UpdateUserAsync(userId, user);
        }

        public async Task<ResponseLogic> RemoveUserSkillAsync(int userId, Skill skill)
        {
            var user = await _repository.FindAsync(userId);

            if (!user.Skills.Contains(skill))
            {
                return ResponseLogic.SUCCESS;
            }

            user.Skills.Remove(skill);

            return await UpdateUserAsync(userId, user);
        }

    }
}
