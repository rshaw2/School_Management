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
    /// The courseService responsible for managing course related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting course information.
    /// </remarks>
    public interface ICourseService
    {
        /// <summary>Retrieves a specific course by its primary key</summary>
        /// <param name="id">The primary key of the course</param>
        /// <returns>The course data</returns>
        Course GetById(Guid id);

        /// <summary>Retrieves a list of courses based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of courses</returns>
        List<Course> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new course</summary>
        /// <param name="model">The course data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Course model);

        /// <summary>Updates a specific course by its primary key</summary>
        /// <param name="id">The primary key of the course</param>
        /// <param name="updatedEntity">The course data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Course updatedEntity);

        /// <summary>Updates a specific course by its primary key</summary>
        /// <param name="id">The primary key of the course</param>
        /// <param name="updatedEntity">The course data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Course> updatedEntity);

        /// <summary>Deletes a specific course by its primary key</summary>
        /// <param name="id">The primary key of the course</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The courseService responsible for managing course related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting course information.
    /// </remarks>
    public class CourseService : ICourseService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Course class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public CourseService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific course by its primary key</summary>
        /// <param name="id">The primary key of the course</param>
        /// <returns>The course data</returns>
        public Course GetById(Guid id)
        {
            var entityData = _dbContext.Course.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of courses based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of courses</returns>/// <exception cref="Exception"></exception>
        public List<Course> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetCourse(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new course</summary>
        /// <param name="model">The course data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Course model)
        {
            model.Id = CreateCourse(model);
            return model.Id;
        }

        /// <summary>Updates a specific course by its primary key</summary>
        /// <param name="id">The primary key of the course</param>
        /// <param name="updatedEntity">The course data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Course updatedEntity)
        {
            UpdateCourse(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific course by its primary key</summary>
        /// <param name="id">The primary key of the course</param>
        /// <param name="updatedEntity">The course data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Course> updatedEntity)
        {
            PatchCourse(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific course by its primary key</summary>
        /// <param name="id">The primary key of the course</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteCourse(id);
            return true;
        }
        #region
        private List<Course> GetCourse(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Course.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Course>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Course), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Course, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateCourse(Course model)
        {
            _dbContext.Course.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateCourse(Guid id, Course updatedEntity)
        {
            _dbContext.Course.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteCourse(Guid id)
        {
            var entityData = _dbContext.Course.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Course.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchCourse(Guid id, JsonPatchDocument<Course> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Course.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Course.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}