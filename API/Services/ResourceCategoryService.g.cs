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
    /// The resourcecategoryService responsible for managing resourcecategory related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting resourcecategory information.
    /// </remarks>
    public interface IResourceCategoryService
    {
        /// <summary>Retrieves a specific resourcecategory by its primary key</summary>
        /// <param name="id">The primary key of the resourcecategory</param>
        /// <returns>The resourcecategory data</returns>
        ResourceCategory GetById(Guid id);

        /// <summary>Retrieves a list of resourcecategorys based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of resourcecategorys</returns>
        List<ResourceCategory> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new resourcecategory</summary>
        /// <param name="model">The resourcecategory data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(ResourceCategory model);

        /// <summary>Updates a specific resourcecategory by its primary key</summary>
        /// <param name="id">The primary key of the resourcecategory</param>
        /// <param name="updatedEntity">The resourcecategory data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, ResourceCategory updatedEntity);

        /// <summary>Updates a specific resourcecategory by its primary key</summary>
        /// <param name="id">The primary key of the resourcecategory</param>
        /// <param name="updatedEntity">The resourcecategory data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<ResourceCategory> updatedEntity);

        /// <summary>Deletes a specific resourcecategory by its primary key</summary>
        /// <param name="id">The primary key of the resourcecategory</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The resourcecategoryService responsible for managing resourcecategory related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting resourcecategory information.
    /// </remarks>
    public class ResourceCategoryService : IResourceCategoryService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the ResourceCategory class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public ResourceCategoryService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific resourcecategory by its primary key</summary>
        /// <param name="id">The primary key of the resourcecategory</param>
        /// <returns>The resourcecategory data</returns>
        public ResourceCategory GetById(Guid id)
        {
            var entityData = _dbContext.ResourceCategory.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of resourcecategorys based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of resourcecategorys</returns>/// <exception cref="Exception"></exception>
        public List<ResourceCategory> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetResourceCategory(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new resourcecategory</summary>
        /// <param name="model">The resourcecategory data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(ResourceCategory model)
        {
            model.Id = CreateResourceCategory(model);
            return model.Id;
        }

        /// <summary>Updates a specific resourcecategory by its primary key</summary>
        /// <param name="id">The primary key of the resourcecategory</param>
        /// <param name="updatedEntity">The resourcecategory data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, ResourceCategory updatedEntity)
        {
            UpdateResourceCategory(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific resourcecategory by its primary key</summary>
        /// <param name="id">The primary key of the resourcecategory</param>
        /// <param name="updatedEntity">The resourcecategory data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<ResourceCategory> updatedEntity)
        {
            PatchResourceCategory(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific resourcecategory by its primary key</summary>
        /// <param name="id">The primary key of the resourcecategory</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteResourceCategory(id);
            return true;
        }
        #region
        private List<ResourceCategory> GetResourceCategory(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.ResourceCategory.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<ResourceCategory>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(ResourceCategory), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<ResourceCategory, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateResourceCategory(ResourceCategory model)
        {
            _dbContext.ResourceCategory.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateResourceCategory(Guid id, ResourceCategory updatedEntity)
        {
            _dbContext.ResourceCategory.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteResourceCategory(Guid id)
        {
            var entityData = _dbContext.ResourceCategory.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.ResourceCategory.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchResourceCategory(Guid id, JsonPatchDocument<ResourceCategory> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.ResourceCategory.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.ResourceCategory.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}