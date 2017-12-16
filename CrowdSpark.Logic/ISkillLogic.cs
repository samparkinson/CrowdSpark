using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CrowdSpark.Common;
using CrowdSpark.Entitites;

namespace CrowdSpark.Logic
{
    public interface ISkillLogic : IDisposable
    {
        Task<IEnumerable<SkillDTO>> GetAsync();

        Task<SkillDTO> GetAsync(int skillId);

        Task<IEnumerable<SkillDTO>> SearchAsync(string searchString);

        Task<SkillDTO> FindExactAsync(string searchString);

        Task<ResponseLogic> CreateAsync(SkillCreateDTO skill);

        Task<ResponseLogic> UpdateAsync(SkillDTO skill);

        Task<ResponseLogic> RemoveWithObjectAsync(SkillDTO skill);

        Task<ResponseLogic> DeleteAsync(int skillId);
    }
}
