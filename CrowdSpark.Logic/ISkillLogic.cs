using System;
using System.Threading.Tasks;
using CrowdSpark.Common;
using CrowdSpark.Entitites;

namespace CrowdSpark.Logic
{
    public interface ISkillLogic
    {
        Task<ResponseLogic> CreateSkillAsync(Skill skill);

        Task<ResponseLogic> RemoveSkillAsync(Skill skill);
    }
}
