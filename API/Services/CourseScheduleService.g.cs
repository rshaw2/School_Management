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
    /// The coursescheduleService responsible for managing courseschedule related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting courseschedule information.
    /// </remarks>
    public interface ICourseScheduleService
    {
        /// <summary>Retrieves a specific courseschedule by its primary key</summary>
        /// <param name="id">The primary key of the courseschedule</param>
        /// <returns>The courseschedule data</returns>
        CourseSchedule GetById(Guid id);

        /// <summary>Retrieves a list of courseschedules based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of courseschedules</returns>
        List<CourseSchedule> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new courseschedule</summary>
        /// <param name="model">The courseschedule data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(CourseSchedule model);

        /// <summary>Updates a specific courseschedule by its primary key</summary>
        /// <param name="id">The primary key of the courseschedule</param>
        /// <param name="updatedEntity">The courseschedule data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, CourseSchedule updatedEntity);

        /// <summary>Updates a specific courseschedule by its primary key</summary>
        /// <param name="id">The primary key of the courseschedule</param>
        /// <param name="updatedEntity">The courseschedule data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<CourseSchedule> updatedEntity);

        /// <summary>Deletes a specific courseschedule by its primary key</summary>
        /// <param name="id">The primary key of the courseschedule</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The coursescheduleService responsible for managing courseschedule related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting courseschedule information.
    /// </remarks>
    public class CourseScheduleService : ICourseScheduleService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the CourseSchedule class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public CourseScheduleService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific courseschedule by its primary key</summary>
        /// <param name="id">The primary key of the courseschedule</param>
        /// <returns>The courseschedule data</returns>
        public CourseSchedule GetById(Guid id)
        {
            var entityData = _dbContext.CourseSchedule.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of courseschedules based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of courseschedules</returns>/// <exception cref="Exception"></exception>
        public List<CourseSchedule> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetCourseSchedule(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new courseschedule</summary>
        /// <param name="model">The courseschedule data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(CourseSchedule model)
        {
            model.Id = CreateCourseSchedule(model);
            return model.Id;
        }

        /// <summary>Updates a specific courseschedule by its primary key</summary>
        /// <param name="id">The primary key of the courseschedule</param>
        /// <param name="updatedEntity">The courseschedule data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, CourseSchedule updatedEntity)
        {
            UpdateCourseSchedule(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific courseschedule by its primary key</summary>
        /// <param name="id">The primary key of the courseschedule</param>
        /// <param name="updatedEntity">The courseschedule data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<CourseSchedule> updatedEntity)
        {
            PatchCourseSchedule(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific courseschedule by its primary key</summary>
        /// <param name="id">The primary key of the courseschedule</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteCourseSchedule(id);
            return true;
        }
        #region
        private List<CourseSchedule> GetCourseSchedule(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.CourseSchedule.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<CourseSchedule>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(CourseSchedule), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<CourseSchedule, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateCourseSchedule(CourseSchedule model)
        {
            _dbContext.CourseSchedule.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateCourseSchedule(Guid id, CourseSchedule updatedEntity)
        {
            _dbContext.CourseSchedule.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteCourseSchedule(Guid id)
        {
            var entityData = _dbContext.CourseSchedule.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.CourseSchedule.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchCourseSchedule(Guid id, JsonPatchDocument<CourseSchedule> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.CourseSchedule.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.CourseSchedule.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}