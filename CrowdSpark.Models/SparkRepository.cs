using System;
using CrowdSpark.Entitites;
using CrowdSpark.Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CrowdSpark.Models
{
    public class SparkRepository : ISparkRepository
    {
        private readonly ICrowdSparkContext _context;

        public SparkRepository(ICrowdSparkContext context)
        {
            _context = context;
        }

        public async Task<(int, int)> CreateAsync(int projectId, int userId)
        {
            var project = await _context.Projects.FindAsync(projectId);
            var user = await _context.Users.FindAsync(userId);

            var sparkToCreate = new Spark
            {
                ProjectId = project.Id,
                Project = project,
                UserId = user.Id,
                User = user,
                Status = (int)SparkStatus.PENDING,
                CreatedDate = System.DateTime.UtcNow
            };

            _context.Sparks.Add(sparkToCreate);
            if (await saveContextChanges() > 0)
            {
                return (sparkToCreate.ProjectId, sparkToCreate.UserId);
            }
            else throw new DbUpdateException("Error creating spark", (Exception)null);
        }

        public async Task<bool> DeleteAsync(int projectId, int userId)
        {
            var spark = await _context.Sparks.FindAsync(projectId, userId);
            _context.Sparks.Remove(spark);

            return ( await saveContextChanges() > 0 );
        }

        public async Task<SparkDTO> FindAsync(int projectId, int userId)
        {
            var spark = await _context.Sparks.FindAsync(projectId, userId);

            return new SparkDTO()
            {
                PId = spark.ProjectId,
                UId = spark.UserId,
                Status = spark.Status,
                CreatedDate = spark.CreatedDate
            };
        }

        public async Task<IReadOnlyCollection<SparkDTO>> ReadAsync()
        {
            return await _context.Sparks.Select(s => new SparkDTO()
            {
                PId = s.ProjectId,
                UId = s.UserId,
                Status = s.Status,
                CreatedDate = s.CreatedDate
            }).ToListAsync();
        }

        public async Task<IReadOnlyCollection<SparkDTO>> ReadForProjectAsync(int projectId)
        {
            return await _context.Sparks.Where(s => s.ProjectId == projectId)
                .Select(s => new SparkDTO()
                {
                    PId = s.ProjectId,
                    UId = s.UserId,
                    Status = s.Status,
                    CreatedDate = s.CreatedDate
                }).ToListAsync();
        }

        public async Task<IReadOnlyCollection<SparkDTO>> ReadForUserAsync(int userId)
        {
            return await _context.Sparks.Where(s => s.UserId == userId)
                .Select(s => new SparkDTO()
                {
                    PId = s.ProjectId,
                    UId = s.UserId,
                    Status = s.Status,
                    CreatedDate = s.CreatedDate
                }).ToListAsync();
        }

        public async Task<bool> UpdateAsync(SparkDTO details)
        {
            var sparkToUpdate = await _context.Sparks.FindAsync(details.PId, details.UId);
            _context.Sparks.Update(sparkToUpdate);

            sparkToUpdate.Status = details.Status; //This is the only attribute on a spark that should ever change

            return (await saveContextChanges() > 0);
        }

        async Task<int> saveContextChanges()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (System.Data.DataException e)
            {
                throw new DbUpdateException("Error modifying spark collection", e);
            }
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _context.Dispose();
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
