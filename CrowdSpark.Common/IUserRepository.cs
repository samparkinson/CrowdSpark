using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrowdSpark.Common
{
    public interface IUserRepository : IDisposable
    {
        Task<int> CreateAsync(UserCreateDTO user, string azureUId);

        Task<UserDTO> FindAsync(int userId);

        Task<int> GetIdAsync(string azureUId);

        Task<UserDTO> FindFromAzureUIdAsync(string azureUId);

        Task<IReadOnlyCollection<UserDTO>> ReadAsync();

        Task<bool> UpdateAsync(int userId, UserDTO details);

        Task<bool> DeleteAsync(int userId);
    }
}
