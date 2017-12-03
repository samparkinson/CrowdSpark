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

        public Task<int> CreateAsync(ProjectDTO project)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int projectId)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<ProjectDetailsDTO> FindAsync(int projectId)
        {
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyCollection<ProjectDetailsDTO>> ReadAsync()
        {
            var projects = from c in _context.Projects
                             select new ProjectDetailsDTO
                             {
                                 Id = c.Id,
                                 Title = c.Title,
                                 Description = c.Description,
                                 LocationId = c.LocationId,
                                 Skills = c.Skills
                             };
            return await projects.ToListAsync();
        }

        public Task<bool> UpdateAsync(ProjectDetailsDTO details)
        {
            throw new NotImplementedException();
        }
    }
}