using CrowdSpark.Entitites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrowdSpark.Common
{
    public interface ISparkRepository : IDisposable
    {
        Task<(int, int)> CreateAsync(int projectId, int userId); // returns the Project Id and the UserId which combined are the key
     
        Task<SparkDTO> FindAsync(int projectId, int userId);

        Task<IReadOnlyCollection<SparkDTO>> ReadAsync();

        Task<IReadOnlyCollection<SparkDTO>> ReadForProjectAsync(int projectId);

        Task<IReadOnlyCollection<SparkDTO>> ReadForUserAsync(int userId);

        Task<bool> UpdateAsync(SparkDTO details);

        Task<bool> DeleteAsync(int projectId, int userId);
    }
}
