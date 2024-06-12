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
    /// The scheduleService responsible for managing schedule related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting schedule information.
    /// </remarks>
    public interface IScheduleService
    {
        /// <summary>Retrieves a specific schedule by its primary key</summary>
        /// <param name="id">The primary key of the schedule</param>
        /// <returns>The schedule data</returns>
        Schedule GetById(Guid id);

        /// <summary>Retrieves a list of schedules based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of schedules</returns>
        List<Schedule> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new schedule</summary>
        /// <param name="model">The schedule data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Schedule model);

        /// <summary>Updates a specific schedule by its primary key</summary>
        /// <param name="id">The primary key of the schedule</param>
        /// <param name="updatedEntity">The schedule data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Schedule updatedEntity);

        /// <summary>Updates a specific schedule by its primary key</summary>
        /// <param name="id">The primary key of the schedule</param>
        /// <param name="updatedEntity">The schedule data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Schedule> updatedEntity);

        /// <summary>Deletes a specific schedule by its primary key</summary>
        /// <param name="id">The primary key of the schedule</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The scheduleService responsible for managing schedule related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting schedule information.
    /// </remarks>
    public class ScheduleService : IScheduleService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Schedule class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public ScheduleService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific schedule by its primary key</summary>
        /// <param name="id">The primary key of the schedule</param>
        /// <returns>The schedule data</returns>
        public Schedule GetById(Guid id)
        {
            var entityData = _dbContext.Schedule.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of schedules based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of schedules</returns>/// <exception cref="Exception"></exception>
        public List<Schedule> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetSchedule(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new schedule</summary>
        /// <param name="model">The schedule data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Schedule model)
        {
            model.Id = CreateSchedule(model);
            return model.Id;
        }

        /// <summary>Updates a specific schedule by its primary key</summary>
        /// <param name="id">The primary key of the schedule</param>
        /// <param name="updatedEntity">The schedule data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Schedule updatedEntity)
        {
            UpdateSchedule(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific schedule by its primary key</summary>
        /// <param name="id">The primary key of the schedule</param>
        /// <param name="updatedEntity">The schedule data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Schedule> updatedEntity)
        {
            PatchSchedule(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific schedule by its primary key</summary>
        /// <param name="id">The primary key of the schedule</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteSchedule(id);
            return true;
        }
        #region
        private List<Schedule> GetSchedule(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Schedule.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Schedule>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Schedule), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Schedule, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateSchedule(Schedule model)
        {
            _dbContext.Schedule.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateSchedule(Guid id, Schedule updatedEntity)
        {
            _dbContext.Schedule.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteSchedule(Guid id)
        {
            var entityData = _dbContext.Schedule.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Schedule.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchSchedule(Guid id, JsonPatchDocument<Schedule> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Schedule.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Schedule.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}