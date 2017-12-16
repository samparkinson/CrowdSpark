using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CrowdSpark.Common;

namespace CrowdSpark.Logic
{
    public interface ICategoryLogic : IDisposable
    {
        Task<IEnumerable<CategoryDTO>> GetAsync();

        Task<CategoryDTO> GetAsync(int categoryId);

        Task<IEnumerable<CategoryDTO>> FindAsync(string searchString);

        Task<CategoryDTO> FindExactAsync(string searchString);

        Task<ResponseLogic> CreateAsync(CategoryCreateDTO category);

        Task<ResponseLogic> UpdateAsync(CategoryDTO category);

        Task<ResponseLogic> RemoveAsync(CategoryDTO category);

        Task<ResponseLogic> DeleteAsync(int categoryId);
    }
}
