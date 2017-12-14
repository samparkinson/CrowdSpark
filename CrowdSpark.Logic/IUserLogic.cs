using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CrowdSpark.Common;
using CrowdSpark.Entitites;

namespace CrowdSpark.Logic
{
    public interface IUserLogic : IDisposable
    {
        Task<IEnumerable<UserDTO>> GetAsync();

        Task<UserDTO> GetAsync(int userId);

        Task<int> GetIdAsync(string azureUId);

        Task<ResponseLogic> CreateAsync(UserDTO user, string azureUId);

        Task<ResponseLogic> UpdateAsync(int userId, UserDTO user);

        Task<ResponseLogic> DeleteAsync(int userId);

        Task<ResponseLogic> AddSkillAsync(int userId, SkillDTO skill);

        Task<ResponseLogic> RemoveSkillAsync(int userId, SkillDTO skill);

    }
}
