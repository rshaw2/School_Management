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
    /// The historyService responsible for managing history related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting history information.
    /// </remarks>
    public interface IHistoryService
    {
        /// <summary>Retrieves a specific history by its primary key</summary>
        /// <param name="id">The primary key of the history</param>
        /// <returns>The history data</returns>
        History GetById(Guid id);

        /// <summary>Retrieves a list of historys based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of historys</returns>
        List<History> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new history</summary>
        /// <param name="model">The history data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(History model);

        /// <summary>Updates a specific history by its primary key</summary>
        /// <param name="id">The primary key of the history</param>
        /// <param name="updatedEntity">The history data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, History updatedEntity);

        /// <summary>Updates a specific history by its primary key</summary>
        /// <param name="id">The primary key of the history</param>
        /// <param name="updatedEntity">The history data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<History> updatedEntity);

        /// <summary>Deletes a specific history by its primary key</summary>
        /// <param name="id">The primary key of the history</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The historyService responsible for managing history related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting history information.
    /// </remarks>
    public class HistoryService : IHistoryService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the History class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public HistoryService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific history by its primary key</summary>
        /// <param name="id">The primary key of the history</param>
        /// <returns>The history data</returns>
        public History GetById(Guid id)
        {
            var entityData = _dbContext.History.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of historys based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of historys</returns>/// <exception cref="Exception"></exception>
        public List<History> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetHistory(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new history</summary>
        /// <param name="model">The history data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(History model)
        {
            model.Id = CreateHistory(model);
            return model.Id;
        }

        /// <summary>Updates a specific history by its primary key</summary>
        /// <param name="id">The primary key of the history</param>
        /// <param name="updatedEntity">The history data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, History updatedEntity)
        {
            UpdateHistory(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific history by its primary key</summary>
        /// <param name="id">The primary key of the history</param>
        /// <param name="updatedEntity">The history data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<History> updatedEntity)
        {
            PatchHistory(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific history by its primary key</summary>
        /// <param name="id">The primary key of the history</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteHistory(id);
            return true;
        }
        #region
        private List<History> GetHistory(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.History.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<History>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(History), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<History, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateHistory(History model)
        {
            _dbContext.History.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateHistory(Guid id, History updatedEntity)
        {
            _dbContext.History.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteHistory(Guid id)
        {
            var entityData = _dbContext.History.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.History.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchHistory(Guid id, JsonPatchDocument<History> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.History.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.History.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}