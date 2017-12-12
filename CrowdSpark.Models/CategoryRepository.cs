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
            _context.Categories.Remove(category);

            return ( await saveContextChanges() > 0 );
        }

        public async Task<Category> FindAsync(int categoryId)
        {
            return await _context.Categories.FindAsync(categoryId);
        }

        public async Task<Category> FindAsync(string searchString)
        {
            return await _context.Categories.Where(c => c.Name.ToLower() == searchString.ToLower()).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Category>> FindWildcardAsync(string searchString)
        {
            return await _context.Categories.Where(c => c.Name.ToLower().Contains(searchString.ToLower())).ToArrayAsync();
        }

        public async Task<IReadOnlyCollection<Category>> ReadAsync()
        {
            return await _context.Categories.OrderBy(item => item.Name).ToListAsync();
        }

        public async Task<bool> UpdateAsync(Category details)
        {
            var categoryToUpdate = await _context.Categories.FindAsync(details.Id);
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
