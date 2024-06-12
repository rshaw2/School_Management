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
    /// The feecategoryService responsible for managing feecategory related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting feecategory information.
    /// </remarks>
    public interface IFeeCategoryService
    {
        /// <summary>Retrieves a specific feecategory by its primary key</summary>
        /// <param name="id">The primary key of the feecategory</param>
        /// <returns>The feecategory data</returns>
        FeeCategory GetById(Guid id);

        /// <summary>Retrieves a list of feecategorys based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of feecategorys</returns>
        List<FeeCategory> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new feecategory</summary>
        /// <param name="model">The feecategory data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(FeeCategory model);

        /// <summary>Updates a specific feecategory by its primary key</summary>
        /// <param name="id">The primary key of the feecategory</param>
        /// <param name="updatedEntity">The feecategory data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, FeeCategory updatedEntity);

        /// <summary>Updates a specific feecategory by its primary key</summary>
        /// <param name="id">The primary key of the feecategory</param>
        /// <param name="updatedEntity">The feecategory data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<FeeCategory> updatedEntity);

        /// <summary>Deletes a specific feecategory by its primary key</summary>
        /// <param name="id">The primary key of the feecategory</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The feecategoryService responsible for managing feecategory related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting feecategory information.
    /// </remarks>
    public class FeeCategoryService : IFeeCategoryService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the FeeCategory class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public FeeCategoryService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific feecategory by its primary key</summary>
        /// <param name="id">The primary key of the feecategory</param>
        /// <returns>The feecategory data</returns>
        public FeeCategory GetById(Guid id)
        {
            var entityData = _dbContext.FeeCategory.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of feecategorys based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of feecategorys</returns>/// <exception cref="Exception"></exception>
        public List<FeeCategory> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetFeeCategory(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new feecategory</summary>
        /// <param name="model">The feecategory data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(FeeCategory model)
        {
            model.Id = CreateFeeCategory(model);
            return model.Id;
        }

        /// <summary>Updates a specific feecategory by its primary key</summary>
        /// <param name="id">The primary key of the feecategory</param>
        /// <param name="updatedEntity">The feecategory data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, FeeCategory updatedEntity)
        {
            UpdateFeeCategory(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific feecategory by its primary key</summary>
        /// <param name="id">The primary key of the feecategory</param>
        /// <param name="updatedEntity">The feecategory data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<FeeCategory> updatedEntity)
        {
            PatchFeeCategory(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific feecategory by its primary key</summary>
        /// <param name="id">The primary key of the feecategory</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteFeeCategory(id);
            return true;
        }
        #region
        private List<FeeCategory> GetFeeCategory(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.FeeCategory.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<FeeCategory>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(FeeCategory), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<FeeCategory, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateFeeCategory(FeeCategory model)
        {
            _dbContext.FeeCategory.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateFeeCategory(Guid id, FeeCategory updatedEntity)
        {
            _dbContext.FeeCategory.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteFeeCategory(Guid id)
        {
            var entityData = _dbContext.FeeCategory.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.FeeCategory.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchFeeCategory(Guid id, JsonPatchDocument<FeeCategory> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.FeeCategory.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.FeeCategory.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}