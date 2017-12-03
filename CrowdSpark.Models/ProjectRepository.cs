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

        public async Task<int> CreateAsync(ProjectDTO project)
        {
            var projectToCreate = new Project
            {
                Title = project.Title,
                Description = project.Description,
                LocationId = project.Location.Id,
                Location = project.Location,
                Skills = project.Skills
            };

            _context.Projects.Add(projectToCreate);
            if (await _context.SaveChangesAsync() > 0)
            {
                return projectToCreate.Id;
            }
            else throw new DbUpdateException("Error creating project", (Exception)null);
        }

        public async Task<bool> DeleteAsync(int projectId)
        {
            var project = await _context.Projects.FindAsync(projectId);
            _context.Projects.Remove(project);

            return (await _context.SaveChangesAsync() > 0);
        }

        public async Task<ProjectDetailsDTO> FindAsync(int projectId)
        {
            var project = await _context.Projects.FindAsync(projectId);

            return new ProjectDetailsDTO
            {
                Id = project.Id,
                Title = project.Title,
                Description = project.Description,
                LocationId = project.Location.Id,
                Skills = project.Skills
            };
        }

        public async Task<IReadOnlyCollection<ProjectDetailsDTO>> ReadAsync()
        {
            var projects = from p in _context.Projects
                             select new ProjectDetailsDTO
                             {
                                 Id = p.Id,
                                 Title = p.Title,
                                 Description = p.Description,
                                 LocationId = p.LocationId,
                                 Skills = p.Skills
                             };

            return await projects.ToListAsync();
        }

        public async Task<bool> UpdateAsync(ProjectDetailsDTO details)
        {
            var projectToUpdate = await _context.Projects.FindAsync(details.Id);
            _context.Projects.Update(projectToUpdate);

            var location = _context.Location.Find(details.LocationId);

            projectToUpdate.Title = details.Title;
            projectToUpdate.Description = details.Description;
            projectToUpdate.LocationId = details.LocationId;
            projectToUpdate.Location = location;
            projectToUpdate.Skills = details.Skills;

            return (await _context.SaveChangesAsync() > 0);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
