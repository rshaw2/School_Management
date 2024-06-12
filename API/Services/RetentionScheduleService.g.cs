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
    /// The retentionscheduleService responsible for managing retentionschedule related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting retentionschedule information.
    /// </remarks>
    public interface IRetentionScheduleService
    {
        /// <summary>Retrieves a specific retentionschedule by its primary key</summary>
        /// <param name="id">The primary key of the retentionschedule</param>
        /// <returns>The retentionschedule data</returns>
        RetentionSchedule GetById(Guid id);

        /// <summary>Retrieves a list of retentionschedules based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of retentionschedules</returns>
        List<RetentionSchedule> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new retentionschedule</summary>
        /// <param name="model">The retentionschedule data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(RetentionSchedule model);

        /// <summary>Updates a specific retentionschedule by its primary key</summary>
        /// <param name="id">The primary key of the retentionschedule</param>
        /// <param name="updatedEntity">The retentionschedule data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, RetentionSchedule updatedEntity);

        /// <summary>Updates a specific retentionschedule by its primary key</summary>
        /// <param name="id">The primary key of the retentionschedule</param>
        /// <param name="updatedEntity">The retentionschedule data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<RetentionSchedule> updatedEntity);

        /// <summary>Deletes a specific retentionschedule by its primary key</summary>
        /// <param name="id">The primary key of the retentionschedule</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The retentionscheduleService responsible for managing retentionschedule related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting retentionschedule information.
    /// </remarks>
    public class RetentionScheduleService : IRetentionScheduleService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the RetentionSchedule class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public RetentionScheduleService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific retentionschedule by its primary key</summary>
        /// <param name="id">The primary key of the retentionschedule</param>
        /// <returns>The retentionschedule data</returns>
        public RetentionSchedule GetById(Guid id)
        {
            var entityData = _dbContext.RetentionSchedule.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of retentionschedules based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of retentionschedules</returns>/// <exception cref="Exception"></exception>
        public List<RetentionSchedule> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetRetentionSchedule(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new retentionschedule</summary>
        /// <param name="model">The retentionschedule data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(RetentionSchedule model)
        {
            model.Id = CreateRetentionSchedule(model);
            return model.Id;
        }

        /// <summary>Updates a specific retentionschedule by its primary key</summary>
        /// <param name="id">The primary key of the retentionschedule</param>
        /// <param name="updatedEntity">The retentionschedule data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, RetentionSchedule updatedEntity)
        {
            UpdateRetentionSchedule(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific retentionschedule by its primary key</summary>
        /// <param name="id">The primary key of the retentionschedule</param>
        /// <param name="updatedEntity">The retentionschedule data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<RetentionSchedule> updatedEntity)
        {
            PatchRetentionSchedule(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific retentionschedule by its primary key</summary>
        /// <param name="id">The primary key of the retentionschedule</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteRetentionSchedule(id);
            return true;
        }
        #region
        private List<RetentionSchedule> GetRetentionSchedule(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.RetentionSchedule.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<RetentionSchedule>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(RetentionSchedule), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<RetentionSchedule, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateRetentionSchedule(RetentionSchedule model)
        {
            _dbContext.RetentionSchedule.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateRetentionSchedule(Guid id, RetentionSchedule updatedEntity)
        {
            _dbContext.RetentionSchedule.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteRetentionSchedule(Guid id)
        {
            var entityData = _dbContext.RetentionSchedule.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.RetentionSchedule.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchRetentionSchedule(Guid id, JsonPatchDocument<RetentionSchedule> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.RetentionSchedule.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.RetentionSchedule.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}