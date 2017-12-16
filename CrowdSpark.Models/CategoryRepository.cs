using System;
using CrowdSpark.Entitites;
using CrowdSpark.Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CrowdSpark.Models
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ICrowdSparkContext _context;

        public CategoryRepository(ICrowdSparkContext context)
        {
            _context = context;
        }

        public async Task<int> CreateAsync(CategoryCreateDTO category)
        {
            var categoryToCreate = new Category
            {
                Name = category.Name
            };

            _context.Categories.Add(categoryToCreate);
            if (await saveContextChanges() > 0)
            {
                return categoryToCreate.Id;
            }
            else throw new DbUpdateException("Error creating category", (Exception)null);
        }

        public async Task<bool> DeleteAsync(int categoryId)
        {
            var category = await _context.Categories.FindAsync(categoryId);
            if (category is null) return false;
            _context.Categories.Remove(category);

            return ( await saveContextChanges() > 0 );
        }

        public async Task<CategoryDTO> FindAsync(int categoryId)
        {
            var category = await _context.Categories.FindAsync(categoryId);

            return new CategoryDTO() { Id = category.Id, Name = category.Name};
        }

        public async Task<CategoryDTO> FindAsync(string searchString)
        {
            return await _context.Categories.Where(c => c.Name.ToLower() == searchString.ToLower()).Select(c => new CategoryDTO() { Id = c.Id, Name = c.Name }).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<CategoryDTO>> FindWildcardAsync(string searchString)
        {
            return await _context.Categories.Where(c => c.Name.ToLower().Contains(searchString.ToLower())).Select(c => new CategoryDTO() { Id = c.Id, Name = c.Name }).ToArrayAsync();
        }

        public async Task<IReadOnlyCollection<CategoryDTO>> ReadAsync()
        {
            return await _context.Categories.OrderBy(item => item.Name).Select(c => new CategoryDTO() { Id = c.Id, Name = c.Name }).ToListAsync();
        }

        public async Task<bool> UpdateAsync(CategoryDTO details)
        {
            var categoryToUpdate = await _context.Categories.FindAsync(details.Id);
            if (categoryToUpdate == null) return false;

            _context.Categories.Update(categoryToUpdate);

            categoryToUpdate.Name = details.Name;

            return (await saveContextChanges() > 0);
        }

        async Task<int> saveContextChanges()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (System.Data.DataException e)
            {
                throw new DbUpdateException("Error modifying category collection", e);
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
