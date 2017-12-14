using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CrowdSpark.Common;
using CrowdSpark.Entitites;

namespace CrowdSpark.Logic
{
    public interface ISparkLogic : IDisposable
    {
        Task<IEnumerable<SparkDTO>> GetAsync();

        Task<SparkDTO> GetAsync(int projectId, int userId);

        Task<IEnumerable<SparkDTO>> GetForProjectAsync(int projectId);

        Task<IEnumerable<SparkDTO>> GetForUserAsync(int userId);

        Task<ResponseLogic> CreateAsync(int projectId, int userId);

        Task<ResponseLogic> UpdateAsync(SparkDTO spark);

        Task<ResponseLogic> DeleteAsync(int projectId, int userId);

        Task<ResponseLogic> DeleteForProjectAsync(int projectId);

        Task<ResponseLogic> DeleteForUserAsync(int userId);
    }
}
