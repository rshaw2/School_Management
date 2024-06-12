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
    /// The calendarService responsible for managing calendar related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting calendar information.
    /// </remarks>
    public interface ICalendarService
    {
        /// <summary>Retrieves a specific calendar by its primary key</summary>
        /// <param name="id">The primary key of the calendar</param>
        /// <returns>The calendar data</returns>
        Calendar GetById(Guid id);

        /// <summary>Retrieves a list of calendars based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of calendars</returns>
        List<Calendar> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new calendar</summary>
        /// <param name="model">The calendar data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Calendar model);

        /// <summary>Updates a specific calendar by its primary key</summary>
        /// <param name="id">The primary key of the calendar</param>
        /// <param name="updatedEntity">The calendar data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Calendar updatedEntity);

        /// <summary>Updates a specific calendar by its primary key</summary>
        /// <param name="id">The primary key of the calendar</param>
        /// <param name="updatedEntity">The calendar data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Calendar> updatedEntity);

        /// <summary>Deletes a specific calendar by its primary key</summary>
        /// <param name="id">The primary key of the calendar</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The calendarService responsible for managing calendar related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting calendar information.
    /// </remarks>
    public class CalendarService : ICalendarService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Calendar class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public CalendarService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific calendar by its primary key</summary>
        /// <param name="id">The primary key of the calendar</param>
        /// <returns>The calendar data</returns>
        public Calendar GetById(Guid id)
        {
            var entityData = _dbContext.Calendar.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of calendars based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of calendars</returns>/// <exception cref="Exception"></exception>
        public List<Calendar> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetCalendar(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new calendar</summary>
        /// <param name="model">The calendar data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Calendar model)
        {
            model.Id = CreateCalendar(model);
            return model.Id;
        }

        /// <summary>Updates a specific calendar by its primary key</summary>
        /// <param name="id">The primary key of the calendar</param>
        /// <param name="updatedEntity">The calendar data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Calendar updatedEntity)
        {
            UpdateCalendar(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific calendar by its primary key</summary>
        /// <param name="id">The primary key of the calendar</param>
        /// <param name="updatedEntity">The calendar data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Calendar> updatedEntity)
        {
            PatchCalendar(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific calendar by its primary key</summary>
        /// <param name="id">The primary key of the calendar</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteCalendar(id);
            return true;
        }
        #region
        private List<Calendar> GetCalendar(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Calendar.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Calendar>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Calendar), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Calendar, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateCalendar(Calendar model)
        {
            _dbContext.Calendar.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateCalendar(Guid id, Calendar updatedEntity)
        {
            _dbContext.Calendar.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteCalendar(Guid id)
        {
            var entityData = _dbContext.Calendar.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Calendar.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchCalendar(Guid id, JsonPatchDocument<Calendar> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Calendar.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Calendar.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}