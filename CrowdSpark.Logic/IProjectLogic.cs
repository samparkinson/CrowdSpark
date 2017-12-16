using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CrowdSpark.Common;

namespace CrowdSpark.Logic
{
    public interface IProjectLogic : IDisposable
    {
        Task<IEnumerable<ProjectSummaryDTO>> GetAsync();

        Task<ProjectDTO> GetDetailedAsync(int projectID);

        Task<ProjectSummaryDTO> GetAsync(int projectID);

        Task<IEnumerable<ProjectSummaryDTO>> SearchAsync(string searchString);

        Task<IEnumerable<ProjectSummaryDTO>> SearchAsync(int categoryId);

        Task<(ResponseLogic outcome, int Id)> CreateAsync(CreateProjectDTO project, int creatorId);

        Task<ResponseLogic> UpdateAsync(ProjectDTO project, int requestingUserId);

        Task<ResponseLogic> UpdateAsync(ProjectSummaryDTO project, int requestingUserId);

        Task<ResponseLogic> DeleteAsync(int projectId, int requestingUserId);

        Task<ResponseLogic> AddSkillAsync(int projectId, SkillDTO skill, int requestingUserId);

        Task<ResponseLogic> RemoveSkillAsync(int projectId, SkillDTO skill, int requestingUserId);

        Task<IEnumerable<SparkDTO>> GetApprovedSparksAsync(int projectId);

    }
}
