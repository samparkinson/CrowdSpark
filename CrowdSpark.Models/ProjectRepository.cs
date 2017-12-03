using System;
using CrowdSpark.Entitites;
using CrowdSpark.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrowdSpark.Models
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly ICrowdSparkContext _context;

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

        public Task<IReadOnlyCollection<ProjectDTO>> ReadAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(ProjectDetailsDTO details)
        {
            throw new NotImplementedException();
        }
    }
}
