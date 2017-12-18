using CrowdSpark.Common;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CrowdSpark.App.Models
{
    public interface IProjectAPI : IDisposable
    {
        Task<IReadOnlyCollection<ProjectSummaryDTO>> GetAll();

        Task<IReadOnlyCollection<ProjectSummaryDTO>> GetBySearch(string searchString);

        Task<IReadOnlyCollection<ProjectSummaryDTO>> GetByCategory(int categoryId);

        Task<ProjectDTO> Get(int projectId);

        Task<int> Create(CreateProjectDTO project);

        Task<bool> Update(ProjectDTO project);

        Task<bool> AddSkill(int projectId, SkillDTO skill);

        Task<IReadOnlyCollection<SkillDTO>> GetSkills(int projectId);

        Task<IReadOnlyCollection<SparkDTO>> GetApprovedSparks(int projectId);

        Task<bool> CreateSpark(int projectId);
    }
}
