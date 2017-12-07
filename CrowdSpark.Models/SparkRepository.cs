﻿using System;
using CrowdSpark.Entitites;
using CrowdSpark.Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CrowdSpark.Models
{
    public class SparkReposiory : ISparkRepository
    {
        private readonly ICrowdSparkContext _context;

        public SparkReposiory(ICrowdSparkContext context)
        {
            _context = context;
        }

        public async Task<(int, int)> CreateAsync(SparkDTO spark, int userId)
        {
            var project = await _context.Projects.FindAsync(spark.PId);
            var user = await _context.Users.FindAsync(userId);

            var sparkToCreate = new Spark
            {
                ProjectId = project.Id,
                Project = project,
                UserId = user.Id,
                User = user,
                Status = 0 //TODO, set this with the enum
            };

            _context.Sparks.Add(sparkToCreate);
            if (await _context.SaveChangesAsync() > 0)
            {
                return (sparkToCreate.ProjectId, sparkToCreate.UserId);
            }
            else throw new DbUpdateException("Error creating spark", (Exception)null);
        }

        public async Task<bool> DeleteAsync(int projectId, int userId)
        {
            var spark = await _context.Sparks.FindAsync(projectId, userId);
            _context.Sparks.Remove(spark);

            return ( await _context.SaveChangesAsync() > 0 );
        }

        public async Task<Spark> FindAsync(int projectId, int userId)
        {
            return await _context.Sparks.FindAsync(projectId, userId);
        }

        public async Task<IReadOnlyCollection<Spark>> ReadAsync()
        {
            return await _context.Sparks.ToListAsync();
        }

        public async Task<bool> UpdateAsync(Spark details)
        {
            var sparkToUpdate = await _context.Sparks.FindAsync(details.ProjectId, details.UserId);
            _context.Sparks.Update(sparkToUpdate);

            sparkToUpdate.Status = details.Status; //this is the only attribute on a spark that should ever change, the rest are Key objects

            return (await _context.SaveChangesAsync() > 0);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
