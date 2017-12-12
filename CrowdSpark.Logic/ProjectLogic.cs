using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrowdSpark.Common;
using CrowdSpark.Entitites;

namespace CrowdSpark.Logic
{
    public class ProjectLogic : IProjectLogic
    {
        private readonly IProjectRepository _repository;
        private readonly ILocationRepository _locationRepository;
        private readonly ISkillLogic _skillLogic;
        private readonly ISparkLogic _sparkLogic;

        public ProjectLogic(IProjectRepository repository, ILocationRepository locationRepository, ISkillLogic skillLogic, ISparkLogic sparkLogic)
        {
            _repository = repository;
            _locationRepository = locationRepository;
            _skillLogic = skillLogic;
            _sparkLogic = sparkLogic;
        }

        public async Task<IEnumerable<ProjectSummaryDTO>> GetAsync()
        {
            return await _repository.ReadAsync();
        }

        public async Task<ProjectDTO> GetDetailedAsync(int projectId)
        {
            return await _repository.FindAsync(projectId);
        }

        public async Task<ProjectSummaryDTO> GetAsync(int projectId)
        {
            var project = await _repository.FindAsync(projectId);

            if (project is null) return null;

            return new ProjectSummaryDTO {
                Id = project.Id,
                Title = project.Title,
                Description = project.Description,
                LocationId = project.Location.Id,
                Category = project.Category
            };
        }

        public async Task<ResponseLogic> CreateAsync(ProjectDTO project)
        {
            var skills = project.Skills;

            var id = await _repository.CreateAsync(project);

            foreach (var skill in skills)
            {
                await _skillLogic.CreateAsync(skill); //TODO, need to convert this to a parallel for each
            }

            if (id == 0)
            {
                foreach (var skill in skills)
                {
                    await _skillLogic.RemoveWithObjectAsync(skill); //TODO, need to convert this to a parallel for each
                }

                return ResponseLogic.ERROR_CREATING;
            }

            return ResponseLogic.SUCCESS;
        }

        public async Task<ResponseLogic> UpdateAsync(ProjectDTO project)
        {
            var currentProject = await _repository.FindAsync(project.Id);

            if (currentProject is null)
            {
                return ResponseLogic.NOT_FOUND;
            }

            var currentSkills = currentProject.Skills;
            var skills = project.Skills;

            var skillsToAdd = skills.Where(s => !currentSkills.Contains(s));
            var skillsToRemove = currentSkills.Where(s => !skills.Contains(s));

            foreach (var skill in skillsToAdd)
            {
                await _skillLogic.CreateAsync(skill); //TODO, need to convert this to a parallel for each
            }
            foreach (var skill in skillsToRemove)
            {
                await _skillLogic.RemoveWithObjectAsync(skill); //TODO, need to convert this to a parallel for each
            }
            
            var success = await _repository.UpdateAsync(project);

            if (success)
            {
                return ResponseLogic.SUCCESS;
            }

            // roll back skill changes 
            foreach (var skill in skillsToAdd)
            {
                await _skillLogic.RemoveWithObjectAsync(skill); //TODO, need to convert this to a parallel for each
            }
            foreach (var skill in skillsToRemove)
            {
                await _skillLogic.CreateAsync(skill); //TODO, need to convert this to a parallel for each
            }

            return ResponseLogic.ERROR_UPDATING;

        }

        public async Task<ResponseLogic> UpdateAsync(ProjectSummaryDTO project)
        {
            var currentProject = await _repository.FindAsync(project.Id);

            if (currentProject is null)
            {
                return ResponseLogic.NOT_FOUND;
            }

            var location = new Location();

            if (project.LocationId is null)
            {
                location = null;
            }
            else location = await _locationRepository.FindAsync(project.LocationId.Value);

            currentProject.Title = project.Title;
            currentProject.Description = project.Description;
            currentProject.Location = location;
            currentProject.Category = project.Category;

            var success = await _repository.UpdateAsync(currentProject);

            if (success)
            {
                return ResponseLogic.SUCCESS;
            }

            return ResponseLogic.ERROR_UPDATING;
        }

        public async Task<ResponseLogic> DeleteAsync(int projectId)
        {
            var project = await _repository.FindAsync(projectId);

            if (project is null)
            {
                return ResponseLogic.NOT_FOUND;
            }

            foreach (var skill in project.Skills)
            {
                await _skillLogic.RemoveWithObjectAsync(skill); //TODO, make run in parallel
            }

            await _sparkLogic.DeleteForProjectAsync(projectId);

            var success = await _repository.DeleteAsync(projectId);

            if (success)
            {
                return ResponseLogic.SUCCESS;
            }
            else
            {
                foreach (var skill in project.Skills)
                {
                    await _skillLogic.CreateAsync(skill); //TODO, make run in parallel
                }

                return ResponseLogic.ERROR_DELETING;
            }
        }

        public async Task<ResponseLogic> AddSkillAsync(int projectId, Skill skill)
        {
            var project = await _repository.FindAsync(projectId);

            if (project.Skills.Contains(skill))
            {
                return ResponseLogic.SUCCESS;
            }

            project.Skills.Add(skill);

            return await UpdateAsync(project);
        }

        public async Task<ResponseLogic> RemoveSkillAsync(int projectId, Skill skill)
        {
            var project = await _repository.FindAsync(projectId);

            if (!project.Skills.Contains(skill))
            {
                return ResponseLogic.SUCCESS;
            }

            project.Skills.Remove(skill);

            return await UpdateAsync(project);
        }
    }
}
