using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CrowdSpark.Common;
using CrowdSpark.Entitites;

namespace CrowdSpark.Logic
{
    public interface ISkillLogic
    {
        Task<IEnumerable<Skill>> GetAsync();

        Task<Skill> GetAsync(int skillId);

        Task<IEnumerable<Skill>> FindAsync(string searchString);

        Task<Skill> FindExactAsync(string searchString);

        Task<ResponseLogic> CreateAsync(Skill skill);

        Task<ResponseLogic> UpdateAsync(Skill skill);

        Task<ResponseLogic> RemoveWithObjectAsync(Skill skill);

        Task<ResponseLogic> DeleteAsync(int skillId);
    }
}
