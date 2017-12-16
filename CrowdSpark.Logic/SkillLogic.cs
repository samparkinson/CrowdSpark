using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CrowdSpark.Common;

namespace CrowdSpark.Logic
{
    public class SkillLogic : ISkillLogic
    {
        ISkillRepository _repository;
        IUserRepository _userRepository;
        IProjectRepository _projectRepository;

        public SkillLogic(ISkillRepository repository, IUserRepository userRepository, IProjectRepository projectRepository)
        {
            _repository = repository;
            _userRepository = userRepository;
            _projectRepository = projectRepository;
        }

        public async Task<ResponseLogic> CreateAsync(SkillCreateDTO skill)
        {
            var currentSkill = await _repository.FindAsync(skill.Name);
            if (currentSkill != null)
            {
                return ResponseLogic.SUCCESS;
            }

            var createdId = await _repository.CreateAsync(skill);
            if (createdId > 0)
            {
                return ResponseLogic.SUCCESS;
            }
            else return ResponseLogic.ERROR_CREATING;
        }

        public async Task<IEnumerable<SkillDTO>> SearchAsync(string searchString)
        {
            return await _repository.FindWildcardAsync(searchString);
        }

        public async Task<SkillDTO> FindExactAsync(string searchString)
        {
            return await _repository.FindAsync(searchString);
        }

        public async Task<IEnumerable<SkillDTO>> GetAsync()
        {
            return await _repository.ReadAsync();
        }

        public async Task<SkillDTO> GetAsync(int skillId)
        {
            return await _repository.FindAsync(skillId);
        }

        public async Task<ResponseLogic> UpdateAsync(SkillDTO skill)
        {
            var currentSkill = await _repository.FindAsync(skill.Id);

            if (currentSkill is null)
            {
                return ResponseLogic.NOT_FOUND;
            }

            currentSkill.Name = skill.Name;

            var success = await _repository.UpdateAsync(currentSkill);

            if (success)
            {
                return ResponseLogic.SUCCESS;
            }
            else return ResponseLogic.ERROR_UPDATING;
        }

        public async Task<ResponseLogic> RemoveWithObjectAsync(SkillDTO skill)
        {
            var foundSkill = await _repository.FindAsync(skill.Id);

            if (foundSkill is null)
            {
                return ResponseLogic.NOT_FOUND;
            }

            var users = await _userRepository.ReadAsync();  //TODO, consider moving this into the repo for more efficiency
            var projects = await _projectRepository.ReadDetailedAsync();
            var occurrences = 0;

            foreach (var user in users) //TODO, make this run parallel
            {
                if (user.Skills.Contains(skill))
                    occurrences++;
            }

            foreach (var project in projects) //TODO, make this run parallel
            {
                if (project.Skills.Contains(skill))
                    occurrences++;
            }

            if (occurrences > 1)
            {
                return ResponseLogic.SUCCESS;
            }

            var success = await _repository.DeleteAsync(skill.Id);
            if (success)
            {
                return ResponseLogic.SUCCESS;
            }
            else return ResponseLogic.ERROR_DELETING;
        }

        public async Task<ResponseLogic> DeleteAsync(int skillId)
        {
            var skill = await _repository.FindAsync(skillId);

            if (skill is null)
            {
                return ResponseLogic.NOT_FOUND;
            }

            var users = await _userRepository.ReadAsync();
            var projects = await _projectRepository.ReadDetailedAsync();
            var occurrences = 0;

            foreach (var user in users) //TODO, make this run parallel
            {
                if (user.Skills.Contains(skill))
                    occurrences++;
            }

            foreach (var project in projects) //TODO, make this run parallel
            {
                if (project.Skills.Contains(skill))
                    occurrences++;
            }

            if (occurrences > 1)
            {
                return ResponseLogic.SUCCESS;
            }

            var success = await _repository.DeleteAsync(skillId);
            if (success)
            {
                return ResponseLogic.SUCCESS;
            }
            else return ResponseLogic.ERROR_DELETING;
        }

        public void Dispose()
        {
            _repository.Dispose();
            _userRepository.Dispose();
            _projectRepository.Dispose();
        }
    }
}
