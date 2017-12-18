using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CrowdSpark.Common;

namespace CrowdSpark.App.Models
{
    public interface ISkillAPI : IDisposable
    {
        Task<IReadOnlyCollection<SkillDTO>> GetAll();

        Task<IReadOnlyCollection<SkillDTO>> GetBySearch(string searchString);

        Task<int> Create(SkillCreateDTO skill);

    }
}
