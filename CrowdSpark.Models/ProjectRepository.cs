using System;
using CrowdSpark.Entitites;
using CrowdSpark.Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CrowdSpark.Models
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly ICrowdSparkContext _context;

        public ProjectRepository(ICrowdSparkContext context)
        {
            _context = context;
        }

        public async Task<int> CreateAsync(CreateProjectDTO project, int creatorUserId)
        {
            var projectToCreate = new Project
            {
                Title = project.Title,
                Description = project.Description,
                LocationId = project.Location?.Id,
                Location = (project.Location is null) ? null : new Location() { Id = project.Location.Id, City = project.Location.City, Country = project.Location.Country },
                Category = (project.Category is null) ? null : new Category() { Id = project.Category.Id, Name = project.Category.Name },
                CreatedDate = System.DateTime.UtcNow,
                CreatorId = creatorUserId
            };

            _context.Projects.Add(projectToCreate);
            if (await saveContextChanges() > 0)
            {
                return projectToCreate.Id;
            }
            else throw new DbUpdateException("Error creating project", (Exception)null);
        }

        public async Task<bool> DeleteAsync(int projectId)
        {
            var project = await _context.Projects.FindAsync(projectId);
            _context.Projects.Remove(project);

            return (await saveContextChanges() > 0);
        }

        public async Task<ProjectDTO> FindAsync(int projectId)
        {
            var project = await _context.Projects.FindAsync(projectId);

            if (project is null)
            {
                return null;
            }
            else return new ProjectDTO
            {
                Id = project.Id,
                Title = project.Title,
                Description = project.Description,
                Location = (project.Location is null) ? null : new LocationDTO() { Id = project.Location.Id, City = project.Location.City, Country = project.Location.Country },
                Skills = EntityConversionHelper.ConvertSkillsToSkillDTOs(project.Skills),
                Sparks = EntityConversionHelper.ConvertSparksToSparkDTOs(project.Sparks),
                Category = (project.Category is null) ? null : new CategoryDTO() { Id = project.Category.Id, Name = project.Category.Name },
                CreatedDate = project.CreatedDate
            };
        }

        public async Task<IEnumerable<ProjectSummaryDTO>> SearchAsync(string searchString)
        {
            var projects = from p in _context.Projects
                           where (p.Title.ToLower().Contains(searchString.ToLower()) || p.Description.ToLower().Contains(searchString.ToLower()))
                           select new ProjectSummaryDTO
                           {
                               Id = p.Id,
                               Title = p.Title,
                               Description = p.Description,
                               LocationId = p.LocationId,
                               Category = (p.Category == null) ? null : new CategoryDTO() { Id = p.Category.Id, Name = p.Category.Name }
                           };

            return await projects.ToListAsync();
        }

        public async Task<IEnumerable<ProjectSummaryDTO>> SearchAsync(int categoryId)
        {
            var projects = from p in _context.Projects
                           where (p.Category != null && p.Category.Id == categoryId)
                           select new ProjectSummaryDTO
                           {
                               Id = p.Id,
                               Title = p.Title,
                               Description = p.Description,
                               LocationId = p.LocationId,
                               Category = (p.Category == null) ? null : new CategoryDTO() { Id = p.Category.Id, Name = p.Category.Name }
                           };

            return await projects.ToListAsync();
        }

        public async Task<IReadOnlyCollection<ProjectSummaryDTO>> ReadAsync()
        {
            var projects = from p in _context.Projects
                             select new ProjectSummaryDTO
                             {
                                 Id = p.Id,
                                 Title = p.Title,
                                 Description = p.Description,
                                 LocationId = p.LocationId,
                                 Category = (p.Category == null) ? null : new CategoryDTO() { Id = p.Category.Id, Name = p.Category.Name }
                             };

            return await projects.ToListAsync();
        }

        public async Task<IReadOnlyCollection<ProjectDTO>> ReadDetailedAsync()
        {
            var projects = from p in _context.Projects
                           select new ProjectDTO
                           {
                               Id = p.Id,
                               Title = p.Title,
                               Description = p.Description,
                               Location = (p.Location == null) ? null : new LocationDTO() { Id = p.Location.Id, City = p.Location.City, Country = p.Location.Country },
                               Skills = EntityConversionHelper.ConvertSkillsToSkillDTOs(p.Skills),
                               Sparks = EntityConversionHelper.ConvertSparksToSparkDTOs(p.Sparks),
                               CreatedDate = p.CreatedDate,
                               Category = (p.Category == null) ? null : new CategoryDTO() { Id = p.Category.Id, Name = p.Category.Name }
                           };

            return await projects.ToListAsync();
        }

        public async Task<bool> UpdateAsync(ProjectDTO details)
        {
            var projectToUpdate = await _context.Projects.FindAsync(details.Id);
            _context.Projects.Update(projectToUpdate);

            projectToUpdate.Title = details.Title;
            projectToUpdate.Description = details.Description;
            projectToUpdate.LocationId = details.Location?.Id;
            projectToUpdate.Location = (details.Location is null) ? null : new Location() { Id = details.Location.Id, City = details.Location.City, Country = details.Location.Country };
            projectToUpdate.Skills = EntityConversionHelper.ConvertSkillDTOsToProjectSkills(details.Skills, details.Id);
            projectToUpdate.Sparks = EntityConversionHelper.ConvertSparkDTOsToSparks(details.Sparks);
            projectToUpdate.Category = (details.Category is null) ? null : new Category() { Id = details.Category.Id, Name = details.Category.Name };

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
                throw new DbUpdateException("Error modifying project collection", e);
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
