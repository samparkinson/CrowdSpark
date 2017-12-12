using CrowdSpark.Common;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CrowdSpark.App.Models
{
    public interface ICategoryAPI : IDisposable
    {
        Task<IReadOnlyCollection<CategoryDTO>> GetAll();

        Task<IReadOnlyCollection<CategoryDTO>> GetAllFollowed();
    }
}
