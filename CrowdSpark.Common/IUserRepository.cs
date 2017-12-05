﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrowdSpark.Common
{
    public interface IUserRepository : IDisposable
    {
        Task<int> CreateAsync(UserDTO user);

        Task<UserDTO> FindAsync(int userId);

        Task<IReadOnlyCollection<UserDTO>> ReadAsync();

        Task<bool> UpdateAsync(int userId, UserDTO details);

        Task<bool> AddSkillAsync(int userId, SkillDTO skill);

        Task<bool> DeleteAsync(int userId);
    }
}
