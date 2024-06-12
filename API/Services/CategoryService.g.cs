using SchoolManagement.Models;
using SchoolManagement.Data;
using SchoolManagement.Filter;
using SchoolManagement.Entities;
using SchoolManagement.Logger;
using Microsoft.AspNetCore.JsonPatch;
using System.Linq.Expressions;

namespace SchoolManagement.Services
{
    /// <summary>
    /// The categoryService responsible for managing category related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting category information.
    /// </remarks>
    public interface ICategoryService
    {
        /// <summary>Retrieves a specific category by its primary key</summary>
        /// <param name="id">The primary key of the category</param>
        /// <returns>The category data</returns>
        Category GetById(Guid id);

        /// <summary>Retrieves a list of categorys based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of categorys</returns>
        List<Category> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new category</summary>
        /// <param name="model">The category data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Category model);

        /// <summary>Updates a specific category by its primary key</summary>
        /// <param name="id">The primary key of the category</param>
        /// <param name="updatedEntity">The category data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Category updatedEntity);

        /// <summary>Updates a specific category by its primary key</summary>
        /// <param name="id">The primary key of the category</param>
        /// <param name="updatedEntity">The category data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Category> updatedEntity);

        /// <summary>Deletes a specific category by its primary key</summary>
        /// <param name="id">The primary key of the category</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The categoryService responsible for managing category related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting category information.
    /// </remarks>
    public class CategoryService : ICategoryService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Category class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public CategoryService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific category by its primary key</summary>
        /// <param name="id">The primary key of the category</param>
        /// <returns>The category data</returns>
        public Category GetById(Guid id)
        {
            var entityData = _dbContext.Category.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of categorys based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of categorys</returns>/// <exception cref="Exception"></exception>
        public List<Category> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetCategory(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new category</summary>
        /// <param name="model">The category data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Category model)
        {
            model.Id = CreateCategory(model);
            return model.Id;
        }

        /// <summary>Updates a specific category by its primary key</summary>
        /// <param name="id">The primary key of the category</param>
        /// <param name="updatedEntity">The category data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Category updatedEntity)
        {
            UpdateCategory(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific category by its primary key</summary>
        /// <param name="id">The primary key of the category</param>
        /// <param name="updatedEntity">The category data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Category> updatedEntity)
        {
            PatchCategory(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific category by its primary key</summary>
        /// <param name="id">The primary key of the category</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteCategory(id);
            return true;
        }
        #region
        private List<Category> GetCategory(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Category.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Category>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Category), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Category, object>>(Expression.Convert(property, typeof(object)), parameter);
                if (sortOrder.Equals("asc", StringComparison.OrdinalIgnoreCase))
                {
                    result = result.OrderBy(lambda);
                }
                else if (sortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase))
                {
                    result = result.OrderByDescending(lambda);
                }
                else
                {
                    throw new ApplicationException("Invalid sort order. Use 'asc' or 'desc'");
                }
            }

            var paginatedResult = result.Skip(skip).Take(pageSize).ToList();
            return paginatedResult;
        }

        private Guid CreateCategory(Category model)
        {
            _dbContext.Category.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateCategory(Guid id, Category updatedEntity)
        {
            _dbContext.Category.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteCategory(Guid id)
        {
            var entityData = _dbContext.Category.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Category.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchCategory(Guid id, JsonPatchDocument<Category> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Category.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Category.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}