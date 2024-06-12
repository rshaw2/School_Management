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
    /// The eventService responsible for managing event related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting event information.
    /// </remarks>
    public interface IEventService
    {
        /// <summary>Retrieves a specific event by its primary key</summary>
        /// <param name="id">The primary key of the event</param>
        /// <returns>The event data</returns>
        Event GetById(Guid id);

        /// <summary>Retrieves a list of events based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of events</returns>
        List<Event> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new event</summary>
        /// <param name="model">The event data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Event model);

        /// <summary>Updates a specific event by its primary key</summary>
        /// <param name="id">The primary key of the event</param>
        /// <param name="updatedEntity">The event data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Event updatedEntity);

        /// <summary>Updates a specific event by its primary key</summary>
        /// <param name="id">The primary key of the event</param>
        /// <param name="updatedEntity">The event data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Event> updatedEntity);

        /// <summary>Deletes a specific event by its primary key</summary>
        /// <param name="id">The primary key of the event</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The eventService responsible for managing event related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting event information.
    /// </remarks>
    public class EventService : IEventService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Event class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public EventService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific event by its primary key</summary>
        /// <param name="id">The primary key of the event</param>
        /// <returns>The event data</returns>
        public Event GetById(Guid id)
        {
            var entityData = _dbContext.Event.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of events based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of events</returns>/// <exception cref="Exception"></exception>
        public List<Event> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetEvent(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new event</summary>
        /// <param name="model">The event data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Event model)
        {
            model.Id = CreateEvent(model);
            return model.Id;
        }

        /// <summary>Updates a specific event by its primary key</summary>
        /// <param name="id">The primary key of the event</param>
        /// <param name="updatedEntity">The event data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Event updatedEntity)
        {
            UpdateEvent(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific event by its primary key</summary>
        /// <param name="id">The primary key of the event</param>
        /// <param name="updatedEntity">The event data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Event> updatedEntity)
        {
            PatchEvent(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific event by its primary key</summary>
        /// <param name="id">The primary key of the event</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteEvent(id);
            return true;
        }
        #region
        private List<Event> GetEvent(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Event.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Event>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Event), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Event, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateEvent(Event model)
        {
            _dbContext.Event.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateEvent(Guid id, Event updatedEntity)
        {
            _dbContext.Event.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteEvent(Guid id)
        {
            var entityData = _dbContext.Event.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Event.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchEvent(Guid id, JsonPatchDocument<Event> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Event.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Event.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}