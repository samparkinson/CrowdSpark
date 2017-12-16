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
        IProjectRepository _repository;
        ILocationRepository _locationRepository;
        ISkillLogic _skillLogic;
        ISparkLogic _sparkLogic;

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

        public async Task<IEnumerable<ProjectSummaryDTO>> SearchAsync(string searchString)
        {
            return await _repository.SearchAsync(searchString);
        }

        public async Task<IEnumerable<ProjectSummaryDTO>> SearchAsync(int categoryId)
        {
            return await _repository.SearchAsync(categoryId);
        }

        public async Task<ResponseLogic> CreateAsync(CreateProjectDTO project)
        {
            var skills = project.Skills;

            var id = await _repository.CreateAsync(project);

            foreach (var skill in skills)
            {
                await _skillLogic.CreateAsync(new SkillCreateDTO() { Name = skill.Name }); //TODO, need to convert this to a parallel for each
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
                await _skillLogic.CreateAsync(new SkillCreateDTO() { Name = skill.Name }); //TODO, need to convert this to a parallel for each
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
                await _skillLogic.CreateAsync(new SkillCreateDTO() { Name = skill.Name }); //TODO, need to convert this to a parallel for each
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
                    await _skillLogic.CreateAsync(new SkillCreateDTO() { Name = skill.Name }); //TODO, make run in parallel
                }

                return ResponseLogic.ERROR_DELETING;
            }
        }

        public async Task<ResponseLogic> AddSkillAsync(int projectId, SkillDTO skill)
        {
            var project = await _repository.FindAsync(projectId);

            if (project.Skills.Contains(skill))
            {
                return ResponseLogic.SUCCESS;
            }

            project.Skills.Add(skill);

            return await UpdateAsync(project);
        }

        public async Task<ResponseLogic> RemoveSkillAsync(int projectId, SkillDTO skill)
        {
            var project = await _repository.FindAsync(projectId);
            if (project is null) return ResponseLogic.NOT_FOUND;

            if (!project.Skills.Contains(skill))
            {
                return ResponseLogic.SUCCESS;
            }

            project.Skills.Remove(skill);

            return await UpdateAsync(project);
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
        private bool disposedValue = false; // To detect redundant calls

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

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ProjectLogic() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
