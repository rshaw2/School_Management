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
    /// The schooleventsService responsible for managing schoolevents related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting schoolevents information.
    /// </remarks>
    public interface ISchoolEventsService
    {
        /// <summary>Retrieves a specific schoolevents by its primary key</summary>
        /// <param name="id">The primary key of the schoolevents</param>
        /// <returns>The schoolevents data</returns>
        SchoolEvents GetById(Guid id);

        /// <summary>Retrieves a list of schooleventss based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of schooleventss</returns>
        List<SchoolEvents> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new schoolevents</summary>
        /// <param name="model">The schoolevents data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(SchoolEvents model);

        /// <summary>Updates a specific schoolevents by its primary key</summary>
        /// <param name="id">The primary key of the schoolevents</param>
        /// <param name="updatedEntity">The schoolevents data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, SchoolEvents updatedEntity);

        /// <summary>Updates a specific schoolevents by its primary key</summary>
        /// <param name="id">The primary key of the schoolevents</param>
        /// <param name="updatedEntity">The schoolevents data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<SchoolEvents> updatedEntity);

        /// <summary>Deletes a specific schoolevents by its primary key</summary>
        /// <param name="id">The primary key of the schoolevents</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The schooleventsService responsible for managing schoolevents related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting schoolevents information.
    /// </remarks>
    public class SchoolEventsService : ISchoolEventsService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the SchoolEvents class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public SchoolEventsService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific schoolevents by its primary key</summary>
        /// <param name="id">The primary key of the schoolevents</param>
        /// <returns>The schoolevents data</returns>
        public SchoolEvents GetById(Guid id)
        {
            var entityData = _dbContext.SchoolEvents.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of schooleventss based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of schooleventss</returns>/// <exception cref="Exception"></exception>
        public List<SchoolEvents> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetSchoolEvents(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new schoolevents</summary>
        /// <param name="model">The schoolevents data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(SchoolEvents model)
        {
            model.Id = CreateSchoolEvents(model);
            return model.Id;
        }

        /// <summary>Updates a specific schoolevents by its primary key</summary>
        /// <param name="id">The primary key of the schoolevents</param>
        /// <param name="updatedEntity">The schoolevents data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, SchoolEvents updatedEntity)
        {
            UpdateSchoolEvents(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific schoolevents by its primary key</summary>
        /// <param name="id">The primary key of the schoolevents</param>
        /// <param name="updatedEntity">The schoolevents data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<SchoolEvents> updatedEntity)
        {
            PatchSchoolEvents(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific schoolevents by its primary key</summary>
        /// <param name="id">The primary key of the schoolevents</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteSchoolEvents(id);
            return true;
        }
        #region
        private List<SchoolEvents> GetSchoolEvents(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.SchoolEvents.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<SchoolEvents>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(SchoolEvents), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<SchoolEvents, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateSchoolEvents(SchoolEvents model)
        {
            _dbContext.SchoolEvents.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateSchoolEvents(Guid id, SchoolEvents updatedEntity)
        {
            _dbContext.SchoolEvents.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteSchoolEvents(Guid id)
        {
            var entityData = _dbContext.SchoolEvents.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.SchoolEvents.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchSchoolEvents(Guid id, JsonPatchDocument<SchoolEvents> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.SchoolEvents.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.SchoolEvents.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}