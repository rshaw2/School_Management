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
    /// The minutesService responsible for managing minutes related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting minutes information.
    /// </remarks>
    public interface IMinutesService
    {
        /// <summary>Retrieves a specific minutes by its primary key</summary>
        /// <param name="id">The primary key of the minutes</param>
        /// <returns>The minutes data</returns>
        Minutes GetById(Guid id);

        /// <summary>Retrieves a list of minutess based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of minutess</returns>
        List<Minutes> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new minutes</summary>
        /// <param name="model">The minutes data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Minutes model);

        /// <summary>Updates a specific minutes by its primary key</summary>
        /// <param name="id">The primary key of the minutes</param>
        /// <param name="updatedEntity">The minutes data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Minutes updatedEntity);

        /// <summary>Updates a specific minutes by its primary key</summary>
        /// <param name="id">The primary key of the minutes</param>
        /// <param name="updatedEntity">The minutes data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Minutes> updatedEntity);

        /// <summary>Deletes a specific minutes by its primary key</summary>
        /// <param name="id">The primary key of the minutes</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The minutesService responsible for managing minutes related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting minutes information.
    /// </remarks>
    public class MinutesService : IMinutesService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Minutes class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public MinutesService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific minutes by its primary key</summary>
        /// <param name="id">The primary key of the minutes</param>
        /// <returns>The minutes data</returns>
        public Minutes GetById(Guid id)
        {
            var entityData = _dbContext.Minutes.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of minutess based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of minutess</returns>/// <exception cref="Exception"></exception>
        public List<Minutes> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetMinutes(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new minutes</summary>
        /// <param name="model">The minutes data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Minutes model)
        {
            model.Id = CreateMinutes(model);
            return model.Id;
        }

        /// <summary>Updates a specific minutes by its primary key</summary>
        /// <param name="id">The primary key of the minutes</param>
        /// <param name="updatedEntity">The minutes data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Minutes updatedEntity)
        {
            UpdateMinutes(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific minutes by its primary key</summary>
        /// <param name="id">The primary key of the minutes</param>
        /// <param name="updatedEntity">The minutes data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Minutes> updatedEntity)
        {
            PatchMinutes(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific minutes by its primary key</summary>
        /// <param name="id">The primary key of the minutes</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteMinutes(id);
            return true;
        }
        #region
        private List<Minutes> GetMinutes(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Minutes.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Minutes>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Minutes), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Minutes, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateMinutes(Minutes model)
        {
            _dbContext.Minutes.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateMinutes(Guid id, Minutes updatedEntity)
        {
            _dbContext.Minutes.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteMinutes(Guid id)
        {
            var entityData = _dbContext.Minutes.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Minutes.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchMinutes(Guid id, JsonPatchDocument<Minutes> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Minutes.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Minutes.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}