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

            _context.Categorys.Add(categoryToCreate);
            if (await _context.SaveChangesAsync() > 0)
            {
                return categoryToCreate.Id;
            }
            else throw new DbUpdateException("Error creating category", (Exception)null);
        }

        public async Task<bool> DeleteAsync(int categoryId)
        {
            var category = await _context.Categorys.FindAsync(categoryId);
            _context.Categorys.Remove(category);

            return ( await _context.SaveChangesAsync() > 0 );
        }

        public async Task<Category> FindAsync(int categoryId)
        {
            return await _context.Categorys.FindAsync(categoryId);
        }

        public async Task<IReadOnlyCollection<Category>> ReadAsync()
        {
            return await _context.Categorys.ToListAsync();
        }

        public async Task<bool> UpdateAsync(Category details)
        {
            var categoryToUpdate = await _context.Categorys.FindAsync(details.Id);
            _context.Categorys.Update(categoryToUpdate);

            categoryToUpdate.Name = details.Name;

            return (await _context.SaveChangesAsync() > 0);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
