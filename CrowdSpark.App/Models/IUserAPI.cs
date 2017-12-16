using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CrowdSpark.Common;

namespace CrowdSpark.App.Models
{
    public interface IUserAPI : IDisposable
    {
        Task<UserDTO> GetMyself();

        Task<UserDTO> Get(int userId);

        Task<bool> Create(UserDTO user);

        Task<bool> Update(UserDTO user);

        Task<bool> AddSkill(SkillDTO skill);

        Task<IReadOnlyCollection<SkillDTO>> GetSkills();
    }
}
