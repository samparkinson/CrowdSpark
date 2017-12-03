using CrowdSpark.Entitites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrowdSpark.Common
{
    public interface ICategoryRepository : IDisposable
    {
        Task<int> CreateAsync(CategoryCreateDTO category);
     
        Task<Category> FindAsync(int categoryId);

        Task<IReadOnlyCollection<Category>> ReadAsync();

        Task<bool> UpdateAsync(Category details);

        Task<bool> DeleteAsync(int categoryId);
    }
}
