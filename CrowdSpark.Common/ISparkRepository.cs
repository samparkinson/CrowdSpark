using CrowdSpark.Entitites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrowdSpark.Common
{
    public interface ISparkRepository : IDisposable
    {
        Task<(int, int)> CreateAsync(SparkDTO spark, int userId); // returns the Project Id and the UserId which combined are the key
     
        Task<Spark> FindAsync(int projectId, int userId);

        Task<IReadOnlyCollection<Spark>> ReadAsync();

        Task<IReadOnlyCollection<Spark>> ReadForProjectAsync(int projectId);

        Task<IReadOnlyCollection<Spark>> ReadForUserAsync(int userId);

        Task<bool> UpdateAsync(Spark details);

        Task<bool> DeleteAsync(int projectId, int userId);
    }
}
