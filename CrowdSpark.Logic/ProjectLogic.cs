using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrowdSpark.Common;

namespace CrowdSpark.Logic
{
    public class ProjectLogic : IProjectLogic
    {
        IProjectRepository _repository;
        ILocationRepository _locationRepository;
        ISkillLogic _skillLogic;
        ISparkLogic _sparkLogic;
        ILocationLogic _locationLogic;
        ICategoryLogic _categoryLogic;

        public ProjectLogic(IProjectRepository repository, ILocationRepository locationRepository, ISkillLogic skillLogic, ISparkLogic sparkLogic, ILocationLogic locationLogic, ICategoryLogic categoryLogic)
        {
            _repository = repository;
            _locationRepository = locationRepository;
            _skillLogic = skillLogic;
            _sparkLogic = sparkLogic;
            _locationLogic = locationLogic;
            _categoryLogic = categoryLogic;
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

        public async Task<IEnumerable<ProjectSummaryDTO>> SearchAsync(string searchString)
        {
            return await _repository.SearchAsync(searchString);
        }

        public async Task<IEnumerable<ProjectSummaryDTO>> SearchAsync(int categoryId)
        {
            return await _repository.SearchAsync(categoryId);
        }

        public async Task<(ResponseLogic outcome, int Id)> CreateAsync(CreateProjectDTO project, int creatorId)
        {
            if (project.Location != null)
            {
                var success = await _locationLogic.CreateAsync(new LocationCreateDTO() { City = project.Location.City, Country = project.Location.Country });
                if (success == ResponseLogic.SUCCESS)
                {
                    project.Location = await _locationLogic.FindExactAsync(project.Location.City, project.Location.Country);
                }
                else return (ResponseLogic.ERROR_CREATING, 0);
            }

            if (project.Category != null)
            {
                var success = await _categoryLogic.CreateAsync(new CategoryCreateDTO() { Name = project.Category.Name});
                if (success == ResponseLogic.SUCCESS)
                {
                    project.Category = await _categoryLogic.FindExactAsync(project.Category.Name);
                }
                else return (ResponseLogic.ERROR_CREATING, 0);
            }

            var id = await _repository.CreateAsync(project, creatorId);

            if (id == 0)
            {
                return (ResponseLogic.ERROR_CREATING, 0);
            }

            return (ResponseLogic.SUCCESS, id);
        }

        public async Task<ResponseLogic> UpdateAsync(ProjectDTO project, int requestingUserId)
        {
            var currentProject = await _repository.FindAsync(project.Id);

            if (currentProject is null)
            {
                return ResponseLogic.NOT_FOUND;
            }

            if (currentProject.Creator?.Id != requestingUserId) return ResponseLogic.UNAUTHORISED;

            var currentSkills = currentProject.Skills;
            var skills = project.Skills;

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
            
            var success = await _repository.UpdateAsync(project);

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

        public async Task<ResponseLogic> UpdateAsync(ProjectSummaryDTO project, int requestingUserId)
        {
            var currentProject = await _repository.FindAsync(project.Id);

            if (currentProject.Creator.Id != requestingUserId) return ResponseLogic.UNAUTHORISED;

            if (currentProject is null)
            {
                return ResponseLogic.NOT_FOUND;
            }

            var location = (project.LocationId is null) ? null : (await _locationRepository.FindAsync(project.LocationId.Value));

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

        public async Task<ResponseLogic> DeleteAsync(int projectId, int requestingUserId)
        {
            var project = await _repository.FindAsync(projectId);

            if (project.Creator.Id != requestingUserId) return ResponseLogic.UNAUTHORISED;

            if (project is null)
            {
                return ResponseLogic.NOT_FOUND;
            }

            foreach (var skill in project.Skills)
            {
                await _skillLogic.RemoveWithObjectAsync(skill);
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
                    await _skillLogic.CreateAsync(new SkillCreateDTO() { Name = skill.Name });
                }

                return ResponseLogic.ERROR_DELETING;
            }
        }

        public async Task<ResponseLogic> AddSkillAsync(int projectId, SkillDTO skill, int requestingUserId)
        {
            var project = await _repository.FindAsync(projectId);
            if (project is null) return ResponseLogic.NOT_FOUND;
            
            if (project.Skills.Contains(skill))
            {
                return ResponseLogic.SUCCESS;
            }

            project.Skills.Add(skill);

            return await UpdateAsync(project, requestingUserId);
        }

        public async Task<ResponseLogic> RemoveSkillAsync(int projectId, SkillDTO skill, int requestingUserId)
        {
            var project = await _repository.FindAsync(projectId);
            if (project is null) return ResponseLogic.NOT_FOUND;

            if (!project.Skills.Contains(skill))
            {
                return ResponseLogic.SUCCESS;
            }

            project.Skills.Remove(skill);

            return await UpdateAsync(project, requestingUserId);
        }

        public async Task<IEnumerable<SparkDTO>> GetApprovedSparksAsync(int projectId)
        {
            var project = await _repository.FindAsync(projectId);
            if (project is null) return null;

            var sparks = new List<SparkDTO>();
            sparks.AddRange(project.Sparks.Where(s => s.Status == (int)SparkStatus.APPROVED));

            return sparks;
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
                    _locationRepository.Dispose();
                    _skillLogic.Dispose();
                    _sparkLogic.Dispose();
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
