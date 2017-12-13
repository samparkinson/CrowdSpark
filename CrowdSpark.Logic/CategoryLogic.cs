using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CrowdSpark.Common;
using CrowdSpark.Entitites;

namespace CrowdSpark.Logic
{
    public class CategoryLogic : ICategoryLogic
    {
        ICategoryRepository _repository;
        IProjectRepository _projectRepository;

        public CategoryLogic(ICategoryRepository repository, IProjectRepository projectRepository)
        {
            _repository = repository;
            _projectRepository = projectRepository;
        }

        public async Task<ResponseLogic> CreateAsync(CategoryCreateDTO category)
        {
            var currentCategory = await _repository.FindAsync(category.Name);
            if (currentCategory != null)
            {
                return ResponseLogic.SUCCESS;
            }

            var createdId = await _repository.CreateAsync(category);
            if (createdId > 0)
            {
                return ResponseLogic.SUCCESS;
            }
            else return ResponseLogic.ERROR_CREATING;
        }

        public async Task<IEnumerable<Category>> FindAsync(string searchString)
        {
            return await _repository.FindWildcardAsync(searchString);
        }

        public async Task<Category> FindExactAsync(string searchString)
        {
            return await _repository.FindAsync(searchString);
        }

        public async Task<IEnumerable<Category>> GetAsync()
        {
            return await _repository.ReadAsync();
        }

        public async Task<Category> GetAsync(int categoryId)
        {
            return await _repository.FindAsync(categoryId);
        }

        public async Task<ResponseLogic> UpdateAsync(Category category)
        {
            var currentCategory = await _repository.FindAsync(category.Id);

            if (currentCategory is null)
            {
                return ResponseLogic.NOT_FOUND;
            }

            currentCategory.Name = category.Name;

            var success = await _repository.UpdateAsync(currentCategory);

            if (success)
            {
                return ResponseLogic.SUCCESS;
            }
            else return ResponseLogic.ERROR_UPDATING;
        }

        public async Task<ResponseLogic> RemoveAsync(Category category)
        {
            var foundCategory = await _repository.FindAsync(category.Id);

            if (foundCategory is null)
            {
                return ResponseLogic.NOT_FOUND;
            }

            var projects = await _projectRepository.ReadAsync();
            var occurrences = 0;

            foreach (var project in projects) //TODO, make this run parallel
            {
                if (project.Category.Id == category.Id)
                    occurrences++;
            }

            if (occurrences > 1)
            {
                return ResponseLogic.SUCCESS;
            }

            var success = await _repository.DeleteAsync(category.Id);
            if (success)
            {
                return ResponseLogic.SUCCESS;
            }
            else return ResponseLogic.ERROR_DELETING;
        }

        public async Task<ResponseLogic> DeleteAsync(int categoryId)
        {
            var category = await _repository.FindAsync(categoryId);

            if (category is null)
            {
                return ResponseLogic.NOT_FOUND;
            }

            var projects = await _projectRepository.ReadAsync();
            var occurrences = 0;

            foreach (var project in projects) //TODO, make this run parallel
            {
                if (project.Category.Id == categoryId)
                    occurrences++;
            }

            if (occurrences > 1)
            {
                return ResponseLogic.SUCCESS;
            }

            var success = await _repository.DeleteAsync(categoryId);
            if (success)
            {
                return ResponseLogic.SUCCESS;
            }
            else return ResponseLogic.ERROR_DELETING;
        }

        public void Dispose()
        {
            _repository.Dispose();
            _projectRepository.Dispose();
        }
    }
}
