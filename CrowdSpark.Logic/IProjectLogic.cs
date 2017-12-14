using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CrowdSpark.Common;
using CrowdSpark.Entitites;

namespace CrowdSpark.Logic
{
    public interface IProjectLogic : IDisposable
    {
        Task<IEnumerable<ProjectSummaryDTO>> GetAsync();

        Task<ProjectDTO> GetDetailedAsync(int projectID);

        Task<ProjectSummaryDTO> GetAsync(int projectID);

        Task<ResponseLogic> CreateAsync(CreateProjectDTO project);

        Task<ResponseLogic> UpdateAsync(ProjectDTO project);

        Task<ResponseLogic> UpdateAsync(ProjectSummaryDTO project);

        Task<ResponseLogic> DeleteAsync(int projectId);

        Task<ResponseLogic> AddSkillAsync(int projectId, SkillDTO skill);

        Task<ResponseLogic> RemoveSkillAsync(int projectId, SkillDTO skill);

    }
}
