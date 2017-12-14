using CrowdSpark.Common;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CrowdSpark.App.Models
{
    public interface IProjectAPI : IDisposable
    {
        Task<IReadOnlyCollection<ProjectSummaryDTO>> GetAll();

        Task<IReadOnlyCollection<ProjectSummaryDTO>> GetAllFollowed();

        Task<IReadOnlyCollection<ProjectSummaryDTO>> GetAllSparked();

        Task<IReadOnlyCollection<ProjectSummaryDTO>> GetBySearch(string searchString);

        Task<ProjectDTO> Get(int projectID);

        Task<bool> AddSkill(int projectID, string skill);
    }
}
