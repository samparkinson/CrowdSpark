using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CrowdSpark.Common;
using CrowdSpark.Entitites;

namespace CrowdSpark.Logic
{
    public interface IUserLogic
    {
        Task<IEnumerable<UserDTO>> GetAsync();

        Task<UserDTO> GetAsync(int userId);

        Task<ResponseLogic> CreateAsync(UserDTO user);

        Task<ResponseLogic> UpdateAsync(int userId, UserDTO user);

        Task<ResponseLogic> DeleteAsync(int userId);

        Task<ResponseLogic> AddSkillAsync(int userId, Skill skill);

        Task<ResponseLogic> RemoveSkillAsync(int userId, Skill skill);

    }
}
