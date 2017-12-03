using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrowdSpark.Common
{
    public interface IProjectRepository : IDisposable
    {
        Task<int> CreateAsync(ProjectDTO project);
     
        Task<ProjectDetailsDTO> FindAsync(int projectId);

        Task<IReadOnlyCollection<ProjectDetailsDTO>> ReadAsync();

        Task<bool> UpdateAsync(ProjectDetailsDTO details);

        Task<bool> DeleteAsync(int projectId);
        
    }
}
