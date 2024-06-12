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
    /// The studentsService responsible for managing students related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting students information.
    /// </remarks>
    public interface IStudentsService
    {
        /// <summary>Retrieves a specific students by its primary key</summary>
        /// <param name="id">The primary key of the students</param>
        /// <returns>The students data</returns>
        Students GetById(Guid id);

        /// <summary>Retrieves a list of studentss based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of studentss</returns>
        List<Students> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new students</summary>
        /// <param name="model">The students data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Students model);

        /// <summary>Updates a specific students by its primary key</summary>
        /// <param name="id">The primary key of the students</param>
        /// <param name="updatedEntity">The students data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Students updatedEntity);

        /// <summary>Updates a specific students by its primary key</summary>
        /// <param name="id">The primary key of the students</param>
        /// <param name="updatedEntity">The students data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Students> updatedEntity);

        /// <summary>Deletes a specific students by its primary key</summary>
        /// <param name="id">The primary key of the students</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The studentsService responsible for managing students related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting students information.
    /// </remarks>
    public class StudentsService : IStudentsService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Students class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public StudentsService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific students by its primary key</summary>
        /// <param name="id">The primary key of the students</param>
        /// <returns>The students data</returns>
        public Students GetById(Guid id)
        {
            var entityData = _dbContext.Students.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of studentss based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of studentss</returns>/// <exception cref="Exception"></exception>
        public List<Students> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetStudents(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new students</summary>
        /// <param name="model">The students data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Students model)
        {
            model.Id = CreateStudents(model);
            return model.Id;
        }

        /// <summary>Updates a specific students by its primary key</summary>
        /// <param name="id">The primary key of the students</param>
        /// <param name="updatedEntity">The students data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Students updatedEntity)
        {
            UpdateStudents(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific students by its primary key</summary>
        /// <param name="id">The primary key of the students</param>
        /// <param name="updatedEntity">The students data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Students> updatedEntity)
        {
            PatchStudents(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific students by its primary key</summary>
        /// <param name="id">The primary key of the students</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteStudents(id);
            return true;
        }
        #region
        private List<Students> GetStudents(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Students.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Students>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Students), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Students, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateStudents(Students model)
        {
            _dbContext.Students.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateStudents(Guid id, Students updatedEntity)
        {
            _dbContext.Students.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteStudents(Guid id)
        {
            var entityData = _dbContext.Students.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Students.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchStudents(Guid id, JsonPatchDocument<Students> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Students.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Students.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}