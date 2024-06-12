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
    /// The teacherService responsible for managing teacher related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting teacher information.
    /// </remarks>
    public interface ITeacherService
    {
        /// <summary>Retrieves a specific teacher by its primary key</summary>
        /// <param name="id">The primary key of the teacher</param>
        /// <returns>The teacher data</returns>
        Teacher GetById(Guid id);

        /// <summary>Retrieves a list of teachers based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of teachers</returns>
        List<Teacher> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new teacher</summary>
        /// <param name="model">The teacher data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Teacher model);

        /// <summary>Updates a specific teacher by its primary key</summary>
        /// <param name="id">The primary key of the teacher</param>
        /// <param name="updatedEntity">The teacher data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Teacher updatedEntity);

        /// <summary>Updates a specific teacher by its primary key</summary>
        /// <param name="id">The primary key of the teacher</param>
        /// <param name="updatedEntity">The teacher data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Teacher> updatedEntity);

        /// <summary>Deletes a specific teacher by its primary key</summary>
        /// <param name="id">The primary key of the teacher</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The teacherService responsible for managing teacher related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting teacher information.
    /// </remarks>
    public class TeacherService : ITeacherService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Teacher class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public TeacherService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific teacher by its primary key</summary>
        /// <param name="id">The primary key of the teacher</param>
        /// <returns>The teacher data</returns>
        public Teacher GetById(Guid id)
        {
            var entityData = _dbContext.Teacher.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of teachers based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of teachers</returns>/// <exception cref="Exception"></exception>
        public List<Teacher> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetTeacher(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new teacher</summary>
        /// <param name="model">The teacher data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Teacher model)
        {
            model.Id = CreateTeacher(model);
            return model.Id;
        }

        /// <summary>Updates a specific teacher by its primary key</summary>
        /// <param name="id">The primary key of the teacher</param>
        /// <param name="updatedEntity">The teacher data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Teacher updatedEntity)
        {
            UpdateTeacher(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific teacher by its primary key</summary>
        /// <param name="id">The primary key of the teacher</param>
        /// <param name="updatedEntity">The teacher data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Teacher> updatedEntity)
        {
            PatchTeacher(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific teacher by its primary key</summary>
        /// <param name="id">The primary key of the teacher</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteTeacher(id);
            return true;
        }
        #region
        private List<Teacher> GetTeacher(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Teacher.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Teacher>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Teacher), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Teacher, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateTeacher(Teacher model)
        {
            _dbContext.Teacher.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateTeacher(Guid id, Teacher updatedEntity)
        {
            _dbContext.Teacher.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteTeacher(Guid id)
        {
            var entityData = _dbContext.Teacher.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Teacher.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchTeacher(Guid id, JsonPatchDocument<Teacher> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Teacher.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Teacher.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}