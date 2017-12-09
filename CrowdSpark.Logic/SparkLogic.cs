﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CrowdSpark.Common;
using CrowdSpark.Entitites;

namespace CrowdSpark.Logic
{
    public class SparkLogic : ISparkLogic
    {
        ISparkRepository _repository;

        public SparkLogic(ISparkRepository repository)
        {
            _repository = repository;
        }

        public async Task<ResponseLogic> CreateAsync(SparkDTO spark)
        {
            var userId = 0; // TODO, get user id from auth

            //Check if already exists
            if ((await _repository.FindAsync(spark.PId, userId)) != null) return ResponseLogic.SUCCESS;

            if (_repository.CreateAsync(spark, userId) != null) return ResponseLogic.SUCCESS;
            else return ResponseLogic.ERROR_CREATING;
        }

        public async Task<ResponseLogic> DeleteAsync(int projectId, int userId)
        {
            if ((await _repository.FindAsync(projectId, userId)) == null) return ResponseLogic.NOT_FOUND;

            if (await _repository.DeleteAsync(projectId, userId)) return ResponseLogic.SUCCESS;
            else return ResponseLogic.ERROR_DELETING;
        }

        public async Task<IEnumerable<Spark>> GetAsync()
        {
            return await _repository.ReadAsync();
        }

        public async Task<Spark> GetAsync(int projectId, int userId)
        {
            return await _repository.FindAsync(projectId, userId);
        }

        public async Task<IEnumerable<Spark>> GetForProjectAsync(int projectId)
        {
            return await _repository.ReadForProjectAsync(projectId);
        }

        public async Task<IEnumerable<Spark>> GetForUserAsync(int userId)
        {
            return await _repository.ReadForUserAsync(userId);
        }

        public async Task<ResponseLogic> UpdateAsync(Spark spark)
        {
            var currentSpark = await _repository.FindAsync(spark.ProjectId, spark.UserId);

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
                if (!await _repository.DeleteAsync(spark.ProjectId, spark.UserId)) return ResponseLogic.ERROR_DELETING;
            }

            return ResponseLogic.SUCCESS;
        }

        public async Task<ResponseLogic> DeleteForUserAsync(int userId)
        {
            var sparks =  await _repository.ReadForUserAsync(userId);

            foreach (var spark in sparks)
            {
                if (!await _repository.DeleteAsync(spark.ProjectId, spark.UserId)) return ResponseLogic.ERROR_DELETING;
            }

            return ResponseLogic.SUCCESS;

        }
    }
}
