using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CrowdSpark.Common;
using CrowdSpark.Entitites;

namespace CrowdSpark.Logic
{
    public interface IUserLogic
    {
        Task<IEnumerable<UserDTO>> GetUsersAsync();

        Task<UserDTO> GetUserAsync(int userId);

        Task<ResponseLogic> CreateUserAsync(UserDTO user);

        Task<ResponseLogic> UpdateUserAsync(int userId, UserDTO user);

        Task<ResponseLogic> DeleteUserAsync(int userId);

        Task<ResponseLogic> AddUserSkillAsync(int userId, Skill skill);

        Task<ResponseLogic> RemoveUserSkillAsync(int userId, Skill skill);

    }
}
