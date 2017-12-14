using CrowdSpark.Entitites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrowdSpark.Common
{
    public interface ISkillRepository : IDisposable
    {
        Task<int> CreateAsync(SkillCreateDTO skill);
     
        Task<SkillDTO> FindAsync(int skillId);

        Task<SkillDTO> FindAsync(string skillName);

        Task<IEnumerable<SkillDTO>> FindWildcardAsync(string skillName);

        Task<IReadOnlyCollection<SkillDTO>> ReadAsync();

        Task<bool> UpdateAsync(SkillDTO details);

        Task<bool> DeleteAsync(int skillId);
    }
}
