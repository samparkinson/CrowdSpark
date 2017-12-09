using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CrowdSpark.Common;
using CrowdSpark.Entitites;

namespace CrowdSpark.Logic
{
    public interface ISparkLogic
    {
        Task<IEnumerable<Spark>> GetAsync();

        Task<Spark> GetAsync(int projectId, int userId);

        Task<IEnumerable<Spark>> GetForProjectAsync(int projectId);

        Task<IEnumerable<Spark>> GetForUserAsync(int userId);

        Task<ResponseLogic> CreateAsync(SparkDTO spark);

        Task<ResponseLogic> UpdateAsync(Spark spark);

        Task<ResponseLogic> DeleteAsync(int projectId, int userId);

        Task<ResponseLogic> DeleteForProjectAsync(int projectId);

        Task<ResponseLogic> DeleteForUserAsync(int userId);
    }
}
