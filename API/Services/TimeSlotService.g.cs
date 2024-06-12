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
    /// The timeslotService responsible for managing timeslot related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting timeslot information.
    /// </remarks>
    public interface ITimeSlotService
    {
        /// <summary>Retrieves a specific timeslot by its primary key</summary>
        /// <param name="id">The primary key of the timeslot</param>
        /// <returns>The timeslot data</returns>
        TimeSlot GetById(Guid id);

        /// <summary>Retrieves a list of timeslots based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of timeslots</returns>
        List<TimeSlot> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new timeslot</summary>
        /// <param name="model">The timeslot data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(TimeSlot model);

        /// <summary>Updates a specific timeslot by its primary key</summary>
        /// <param name="id">The primary key of the timeslot</param>
        /// <param name="updatedEntity">The timeslot data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, TimeSlot updatedEntity);

        /// <summary>Updates a specific timeslot by its primary key</summary>
        /// <param name="id">The primary key of the timeslot</param>
        /// <param name="updatedEntity">The timeslot data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<TimeSlot> updatedEntity);

        /// <summary>Deletes a specific timeslot by its primary key</summary>
        /// <param name="id">The primary key of the timeslot</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The timeslotService responsible for managing timeslot related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting timeslot information.
    /// </remarks>
    public class TimeSlotService : ITimeSlotService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the TimeSlot class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public TimeSlotService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific timeslot by its primary key</summary>
        /// <param name="id">The primary key of the timeslot</param>
        /// <returns>The timeslot data</returns>
        public TimeSlot GetById(Guid id)
        {
            var entityData = _dbContext.TimeSlot.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of timeslots based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of timeslots</returns>/// <exception cref="Exception"></exception>
        public List<TimeSlot> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetTimeSlot(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new timeslot</summary>
        /// <param name="model">The timeslot data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(TimeSlot model)
        {
            model.Id = CreateTimeSlot(model);
            return model.Id;
        }

        /// <summary>Updates a specific timeslot by its primary key</summary>
        /// <param name="id">The primary key of the timeslot</param>
        /// <param name="updatedEntity">The timeslot data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, TimeSlot updatedEntity)
        {
            UpdateTimeSlot(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific timeslot by its primary key</summary>
        /// <param name="id">The primary key of the timeslot</param>
        /// <param name="updatedEntity">The timeslot data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<TimeSlot> updatedEntity)
        {
            PatchTimeSlot(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific timeslot by its primary key</summary>
        /// <param name="id">The primary key of the timeslot</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteTimeSlot(id);
            return true;
        }
        #region
        private List<TimeSlot> GetTimeSlot(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.TimeSlot.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<TimeSlot>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(TimeSlot), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<TimeSlot, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateTimeSlot(TimeSlot model)
        {
            _dbContext.TimeSlot.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateTimeSlot(Guid id, TimeSlot updatedEntity)
        {
            _dbContext.TimeSlot.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteTimeSlot(Guid id)
        {
            var entityData = _dbContext.TimeSlot.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.TimeSlot.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchTimeSlot(Guid id, JsonPatchDocument<TimeSlot> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.TimeSlot.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.TimeSlot.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}