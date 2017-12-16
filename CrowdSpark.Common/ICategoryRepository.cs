using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrowdSpark.Common
{
    public interface ICategoryRepository : IDisposable
    {
        Task<int> CreateAsync(CategoryCreateDTO category);
     
        Task<CategoryDTO> FindAsync(int categoryId);

        Task<CategoryDTO> FindAsync(string categoryName);

        Task<IEnumerable<CategoryDTO>> FindWildcardAsync(string categoryName);

        Task<IReadOnlyCollection<CategoryDTO>> ReadAsync();

        Task<bool> UpdateAsync(CategoryDTO details);

        Task<bool> DeleteAsync(int categoryId);
    }
}
