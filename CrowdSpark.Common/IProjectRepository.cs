using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrowdSpark.Common
{
    public interface IProjectRepository : IDisposable
    {
        Task<int> CreateAsync(CreateProjectDTO project);
     
        Task<ProjectDTO> FindAsync(int projectId);

        Task<IEnumerable<ProjectSummaryDTO>> SearchAsync(string searchString);

        Task<IReadOnlyCollection<ProjectSummaryDTO>> ReadAsync();

        Task<IReadOnlyCollection<ProjectDTO>> ReadDetailedAsync();

        Task<bool> UpdateAsync(ProjectDTO details);

        Task<bool> DeleteAsync(int projectId);
        
    }
}
