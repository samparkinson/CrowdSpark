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

        Task<ResponseLogic> CreateAsync(ProjectDTO project);

        Task<ResponseLogic> UpdateAsync(ProjectDTO project);

        Task<ResponseLogic> UpdateAsync(ProjectSummaryDTO project);

        Task<ResponseLogic> DeleteAsync(int projectId);

        Task<ResponseLogic> AddSkillAsync(int projectId, Skill skill);

        Task<ResponseLogic> RemoveSkillAsync(int projectId, Skill skill);

    }
}
