using CrowdSpark.Entitites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrowdSpark.Common
{
    public interface ISkillRepository : IDisposable
    {
        Task<int> CreateAsync(Skill skill);
     
        Task<Skill> FindAsync(int skillId);

        Task<IReadOnlyCollection<Skill>> ReadAsync();

        Task<bool> UpdateAsync(Skill details);

        Task<bool> DeleteAsync(int skillId);
    }
}
