﻿using System;
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
                Skills = project.Skills,
                Category = project.Category,
                CreatedDate = project.CreatedDate
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
                Location = project.Location,
                Skills = project.Skills,
                Sparks = project.Sparks,
                Category = project.Category,
                CreatedDate = project.CreatedDate
            };
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
                                 Category = p.Category
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
                               Location = p.Location,
                               Skills = p.Skills,
                               Sparks = p.Sparks,
                               CreatedDate = p.CreatedDate,
                               Category = p.Category
                           };

            return await projects.ToListAsync();
        }

        public async Task<bool> UpdateAsync(ProjectDTO details)
        {
            var projectToUpdate = await _context.Projects.FindAsync(details.Id);
            _context.Projects.Update(projectToUpdate);

            projectToUpdate.Title = details.Title;
            projectToUpdate.Description = details.Description;
            projectToUpdate.LocationId = details.Location.Id;
            projectToUpdate.Location = details.Location;
            projectToUpdate.Skills = details.Skills;
            projectToUpdate.Sparks = details.Sparks;
            projectToUpdate.Category = details.Category;
            
            //Not updating created date as it should never be updated

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
