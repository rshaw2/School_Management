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
    /// The timeoffService responsible for managing timeoff related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting timeoff information.
    /// </remarks>
    public interface ITimeOffService
    {
        /// <summary>Retrieves a specific timeoff by its primary key</summary>
        /// <param name="id">The primary key of the timeoff</param>
        /// <returns>The timeoff data</returns>
        TimeOff GetById(Guid id);

        /// <summary>Retrieves a list of timeoffs based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of timeoffs</returns>
        List<TimeOff> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new timeoff</summary>
        /// <param name="model">The timeoff data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(TimeOff model);

        /// <summary>Updates a specific timeoff by its primary key</summary>
        /// <param name="id">The primary key of the timeoff</param>
        /// <param name="updatedEntity">The timeoff data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, TimeOff updatedEntity);

        /// <summary>Updates a specific timeoff by its primary key</summary>
        /// <param name="id">The primary key of the timeoff</param>
        /// <param name="updatedEntity">The timeoff data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<TimeOff> updatedEntity);

        /// <summary>Deletes a specific timeoff by its primary key</summary>
        /// <param name="id">The primary key of the timeoff</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The timeoffService responsible for managing timeoff related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting timeoff information.
    /// </remarks>
    public class TimeOffService : ITimeOffService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the TimeOff class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public TimeOffService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific timeoff by its primary key</summary>
        /// <param name="id">The primary key of the timeoff</param>
        /// <returns>The timeoff data</returns>
        public TimeOff GetById(Guid id)
        {
            var entityData = _dbContext.TimeOff.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of timeoffs based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of timeoffs</returns>/// <exception cref="Exception"></exception>
        public List<TimeOff> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetTimeOff(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new timeoff</summary>
        /// <param name="model">The timeoff data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(TimeOff model)
        {
            model.Id = CreateTimeOff(model);
            return model.Id;
        }

        /// <summary>Updates a specific timeoff by its primary key</summary>
        /// <param name="id">The primary key of the timeoff</param>
        /// <param name="updatedEntity">The timeoff data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, TimeOff updatedEntity)
        {
            UpdateTimeOff(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific timeoff by its primary key</summary>
        /// <param name="id">The primary key of the timeoff</param>
        /// <param name="updatedEntity">The timeoff data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<TimeOff> updatedEntity)
        {
            PatchTimeOff(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific timeoff by its primary key</summary>
        /// <param name="id">The primary key of the timeoff</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteTimeOff(id);
            return true;
        }
        #region
        private List<TimeOff> GetTimeOff(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.TimeOff.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<TimeOff>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(TimeOff), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<TimeOff, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateTimeOff(TimeOff model)
        {
            _dbContext.TimeOff.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateTimeOff(Guid id, TimeOff updatedEntity)
        {
            _dbContext.TimeOff.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteTimeOff(Guid id)
        {
            var entityData = _dbContext.TimeOff.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.TimeOff.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchTimeOff(Guid id, JsonPatchDocument<TimeOff> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.TimeOff.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.TimeOff.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}