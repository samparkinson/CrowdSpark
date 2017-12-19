using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CrowdSpark.Common;

namespace CrowdSpark.Logic
{
    public class SparkLogic : ISparkLogic
    {
        ISparkRepository _repository;

        public SparkLogic(ISparkRepository repository)
        {
            _repository = repository;
        }

        public async Task<ResponseLogic> CreateAsync(int projectId, int userId)
        {
            //Check if already exists
            if ((await _repository.FindAsync(projectId, userId)) != null) return ResponseLogic.SUCCESS;

            if (_repository.CreateAsync(projectId, userId) != null) return ResponseLogic.SUCCESS;
            else return ResponseLogic.ERROR_CREATING;
        }

        public async Task<ResponseLogic> DeleteAsync(int projectId, int userId)
        {
            if ((await _repository.FindAsync(projectId, userId)) == null) return ResponseLogic.NOT_FOUND;

            if (await _repository.DeleteAsync(projectId, userId)) return ResponseLogic.SUCCESS;
            else return ResponseLogic.ERROR_DELETING;
        }

        public async Task<IEnumerable<SparkDTO>> GetAsync()
        {
            return await _repository.ReadAsync();
        }

        public async Task<SparkDTO> GetAsync(int projectId, int userId)
        {
            return await _repository.FindAsync(projectId, userId);
        }

        public async Task<IEnumerable<SparkDTO>> GetForProjectAsync(int projectId)
        {
            return await _repository.ReadForProjectAsync(projectId);
        }

        public async Task<IEnumerable<SparkDTO>> GetForUserAsync(int userId)
        {
            return await _repository.ReadForUserAsync(userId);
        }

        public async Task<ResponseLogic> UpdateAsync(SparkDTO spark)
        {
            var currentSpark = await _repository.FindAsync(spark.PId, spark.UId);

            if (currentSpark is null)
            {
                return ResponseLogic.NOT_FOUND;
            }

            currentSpark.Status = spark.Status;

            var success = await _repository.UpdateAsync(currentSpark);

            if (success)
            {
                return ResponseLogic.SUCCESS;
            }
            else return ResponseLogic.ERROR_UPDATING;
        }

        public async Task<ResponseLogic> DeleteForProjectAsync(int projectId)
        {
            var sparks = await _repository.ReadForProjectAsync(projectId);

            foreach (var spark in sparks)
            {
                if (!await _repository.DeleteAsync(spark.PId, spark.UId)) return ResponseLogic.ERROR_DELETING;
            }

            return ResponseLogic.SUCCESS;
        }

        public async Task<ResponseLogic> DeleteForUserAsync(int userId)
        {
            var sparks =  await _repository.ReadForUserAsync(userId);

            foreach (var spark in sparks)
            {
                if (!await _repository.DeleteAsync(spark.PId, spark.UId)) return ResponseLogic.ERROR_DELETING;
            }

            return ResponseLogic.SUCCESS;

        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _repository.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
