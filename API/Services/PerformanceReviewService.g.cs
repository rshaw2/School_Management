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
    /// The performancereviewService responsible for managing performancereview related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting performancereview information.
    /// </remarks>
    public interface IPerformanceReviewService
    {
        /// <summary>Retrieves a specific performancereview by its primary key</summary>
        /// <param name="id">The primary key of the performancereview</param>
        /// <returns>The performancereview data</returns>
        PerformanceReview GetById(Guid id);

        /// <summary>Retrieves a list of performancereviews based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of performancereviews</returns>
        List<PerformanceReview> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new performancereview</summary>
        /// <param name="model">The performancereview data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(PerformanceReview model);

        /// <summary>Updates a specific performancereview by its primary key</summary>
        /// <param name="id">The primary key of the performancereview</param>
        /// <param name="updatedEntity">The performancereview data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, PerformanceReview updatedEntity);

        /// <summary>Updates a specific performancereview by its primary key</summary>
        /// <param name="id">The primary key of the performancereview</param>
        /// <param name="updatedEntity">The performancereview data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<PerformanceReview> updatedEntity);

        /// <summary>Deletes a specific performancereview by its primary key</summary>
        /// <param name="id">The primary key of the performancereview</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The performancereviewService responsible for managing performancereview related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting performancereview information.
    /// </remarks>
    public class PerformanceReviewService : IPerformanceReviewService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the PerformanceReview class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public PerformanceReviewService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific performancereview by its primary key</summary>
        /// <param name="id">The primary key of the performancereview</param>
        /// <returns>The performancereview data</returns>
        public PerformanceReview GetById(Guid id)
        {
            var entityData = _dbContext.PerformanceReview.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of performancereviews based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of performancereviews</returns>/// <exception cref="Exception"></exception>
        public List<PerformanceReview> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetPerformanceReview(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new performancereview</summary>
        /// <param name="model">The performancereview data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(PerformanceReview model)
        {
            model.Id = CreatePerformanceReview(model);
            return model.Id;
        }

        /// <summary>Updates a specific performancereview by its primary key</summary>
        /// <param name="id">The primary key of the performancereview</param>
        /// <param name="updatedEntity">The performancereview data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, PerformanceReview updatedEntity)
        {
            UpdatePerformanceReview(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific performancereview by its primary key</summary>
        /// <param name="id">The primary key of the performancereview</param>
        /// <param name="updatedEntity">The performancereview data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<PerformanceReview> updatedEntity)
        {
            PatchPerformanceReview(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific performancereview by its primary key</summary>
        /// <param name="id">The primary key of the performancereview</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeletePerformanceReview(id);
            return true;
        }
        #region
        private List<PerformanceReview> GetPerformanceReview(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.PerformanceReview.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<PerformanceReview>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(PerformanceReview), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<PerformanceReview, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreatePerformanceReview(PerformanceReview model)
        {
            _dbContext.PerformanceReview.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdatePerformanceReview(Guid id, PerformanceReview updatedEntity)
        {
            _dbContext.PerformanceReview.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeletePerformanceReview(Guid id)
        {
            var entityData = _dbContext.PerformanceReview.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.PerformanceReview.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchPerformanceReview(Guid id, JsonPatchDocument<PerformanceReview> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.PerformanceReview.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.PerformanceReview.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}