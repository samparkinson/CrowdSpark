using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CrowdSpark.Common;
using CrowdSpark.Entitites;

namespace CrowdSpark.Logic
{
    public interface ICategoryLogic : IDisposable
    {
        Task<IEnumerable<Category>> GetAsync();

        Task<Category> GetAsync(int categoryId);

        Task<IEnumerable<Category>> FindAsync(string searchString);

        Task<Category> FindExactAsync(string searchString);

        Task<ResponseLogic> CreateAsync(CategoryCreateDTO category);

        Task<ResponseLogic> UpdateAsync(Category category);

        Task<ResponseLogic> RemoveAsync(Category category);

        Task<ResponseLogic> DeleteAsync(int categoryId);
    }
}
