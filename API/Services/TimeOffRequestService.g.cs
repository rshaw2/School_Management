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
    /// The timeoffrequestService responsible for managing timeoffrequest related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting timeoffrequest information.
    /// </remarks>
    public interface ITimeOffRequestService
    {
        /// <summary>Retrieves a specific timeoffrequest by its primary key</summary>
        /// <param name="id">The primary key of the timeoffrequest</param>
        /// <returns>The timeoffrequest data</returns>
        TimeOffRequest GetById(Guid id);

        /// <summary>Retrieves a list of timeoffrequests based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of timeoffrequests</returns>
        List<TimeOffRequest> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new timeoffrequest</summary>
        /// <param name="model">The timeoffrequest data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(TimeOffRequest model);

        /// <summary>Updates a specific timeoffrequest by its primary key</summary>
        /// <param name="id">The primary key of the timeoffrequest</param>
        /// <param name="updatedEntity">The timeoffrequest data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, TimeOffRequest updatedEntity);

        /// <summary>Updates a specific timeoffrequest by its primary key</summary>
        /// <param name="id">The primary key of the timeoffrequest</param>
        /// <param name="updatedEntity">The timeoffrequest data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<TimeOffRequest> updatedEntity);

        /// <summary>Deletes a specific timeoffrequest by its primary key</summary>
        /// <param name="id">The primary key of the timeoffrequest</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The timeoffrequestService responsible for managing timeoffrequest related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting timeoffrequest information.
    /// </remarks>
    public class TimeOffRequestService : ITimeOffRequestService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the TimeOffRequest class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public TimeOffRequestService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific timeoffrequest by its primary key</summary>
        /// <param name="id">The primary key of the timeoffrequest</param>
        /// <returns>The timeoffrequest data</returns>
        public TimeOffRequest GetById(Guid id)
        {
            var entityData = _dbContext.TimeOffRequest.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of timeoffrequests based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of timeoffrequests</returns>/// <exception cref="Exception"></exception>
        public List<TimeOffRequest> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetTimeOffRequest(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new timeoffrequest</summary>
        /// <param name="model">The timeoffrequest data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(TimeOffRequest model)
        {
            model.Id = CreateTimeOffRequest(model);
            return model.Id;
        }

        /// <summary>Updates a specific timeoffrequest by its primary key</summary>
        /// <param name="id">The primary key of the timeoffrequest</param>
        /// <param name="updatedEntity">The timeoffrequest data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, TimeOffRequest updatedEntity)
        {
            UpdateTimeOffRequest(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific timeoffrequest by its primary key</summary>
        /// <param name="id">The primary key of the timeoffrequest</param>
        /// <param name="updatedEntity">The timeoffrequest data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<TimeOffRequest> updatedEntity)
        {
            PatchTimeOffRequest(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific timeoffrequest by its primary key</summary>
        /// <param name="id">The primary key of the timeoffrequest</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteTimeOffRequest(id);
            return true;
        }
        #region
        private List<TimeOffRequest> GetTimeOffRequest(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.TimeOffRequest.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<TimeOffRequest>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(TimeOffRequest), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<TimeOffRequest, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateTimeOffRequest(TimeOffRequest model)
        {
            _dbContext.TimeOffRequest.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateTimeOffRequest(Guid id, TimeOffRequest updatedEntity)
        {
            _dbContext.TimeOffRequest.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteTimeOffRequest(Guid id)
        {
            var entityData = _dbContext.TimeOffRequest.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.TimeOffRequest.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchTimeOffRequest(Guid id, JsonPatchDocument<TimeOffRequest> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.TimeOffRequest.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.TimeOffRequest.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}