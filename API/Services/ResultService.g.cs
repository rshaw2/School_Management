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
    /// The resultService responsible for managing result related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting result information.
    /// </remarks>
    public interface IResultService
    {
        /// <summary>Retrieves a specific result by its primary key</summary>
        /// <param name="id">The primary key of the result</param>
        /// <returns>The result data</returns>
        Result GetById(Guid id);

        /// <summary>Retrieves a list of results based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of results</returns>
        List<Result> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new result</summary>
        /// <param name="model">The result data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Result model);

        /// <summary>Updates a specific result by its primary key</summary>
        /// <param name="id">The primary key of the result</param>
        /// <param name="updatedEntity">The result data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Result updatedEntity);

        /// <summary>Updates a specific result by its primary key</summary>
        /// <param name="id">The primary key of the result</param>
        /// <param name="updatedEntity">The result data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Result> updatedEntity);

        /// <summary>Deletes a specific result by its primary key</summary>
        /// <param name="id">The primary key of the result</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The resultService responsible for managing result related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting result information.
    /// </remarks>
    public class ResultService : IResultService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Result class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public ResultService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific result by its primary key</summary>
        /// <param name="id">The primary key of the result</param>
        /// <returns>The result data</returns>
        public Result GetById(Guid id)
        {
            var entityData = _dbContext.Result.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of results based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of results</returns>/// <exception cref="Exception"></exception>
        public List<Result> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetResult(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new result</summary>
        /// <param name="model">The result data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Result model)
        {
            model.Id = CreateResult(model);
            return model.Id;
        }

        /// <summary>Updates a specific result by its primary key</summary>
        /// <param name="id">The primary key of the result</param>
        /// <param name="updatedEntity">The result data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Result updatedEntity)
        {
            UpdateResult(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific result by its primary key</summary>
        /// <param name="id">The primary key of the result</param>
        /// <param name="updatedEntity">The result data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Result> updatedEntity)
        {
            PatchResult(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific result by its primary key</summary>
        /// <param name="id">The primary key of the result</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteResult(id);
            return true;
        }
        #region
        private List<Result> GetResult(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Result.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Result>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Result), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Result, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateResult(Result model)
        {
            _dbContext.Result.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateResult(Guid id, Result updatedEntity)
        {
            _dbContext.Result.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteResult(Guid id)
        {
            var entityData = _dbContext.Result.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Result.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchResult(Guid id, JsonPatchDocument<Result> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Result.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Result.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}