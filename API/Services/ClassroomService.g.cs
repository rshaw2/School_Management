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
    /// The classroomService responsible for managing classroom related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting classroom information.
    /// </remarks>
    public interface IClassroomService
    {
        /// <summary>Retrieves a specific classroom by its primary key</summary>
        /// <param name="id">The primary key of the classroom</param>
        /// <returns>The classroom data</returns>
        Classroom GetById(Guid id);

        /// <summary>Retrieves a list of classrooms based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of classrooms</returns>
        List<Classroom> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new classroom</summary>
        /// <param name="model">The classroom data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Classroom model);

        /// <summary>Updates a specific classroom by its primary key</summary>
        /// <param name="id">The primary key of the classroom</param>
        /// <param name="updatedEntity">The classroom data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Classroom updatedEntity);

        /// <summary>Updates a specific classroom by its primary key</summary>
        /// <param name="id">The primary key of the classroom</param>
        /// <param name="updatedEntity">The classroom data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Classroom> updatedEntity);

        /// <summary>Deletes a specific classroom by its primary key</summary>
        /// <param name="id">The primary key of the classroom</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The classroomService responsible for managing classroom related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting classroom information.
    /// </remarks>
    public class ClassroomService : IClassroomService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Classroom class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public ClassroomService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific classroom by its primary key</summary>
        /// <param name="id">The primary key of the classroom</param>
        /// <returns>The classroom data</returns>
        public Classroom GetById(Guid id)
        {
            var entityData = _dbContext.Classroom.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of classrooms based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of classrooms</returns>/// <exception cref="Exception"></exception>
        public List<Classroom> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetClassroom(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new classroom</summary>
        /// <param name="model">The classroom data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Classroom model)
        {
            model.Id = CreateClassroom(model);
            return model.Id;
        }

        /// <summary>Updates a specific classroom by its primary key</summary>
        /// <param name="id">The primary key of the classroom</param>
        /// <param name="updatedEntity">The classroom data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Classroom updatedEntity)
        {
            UpdateClassroom(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific classroom by its primary key</summary>
        /// <param name="id">The primary key of the classroom</param>
        /// <param name="updatedEntity">The classroom data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Classroom> updatedEntity)
        {
            PatchClassroom(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific classroom by its primary key</summary>
        /// <param name="id">The primary key of the classroom</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteClassroom(id);
            return true;
        }
        #region
        private List<Classroom> GetClassroom(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Classroom.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Classroom>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Classroom), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Classroom, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateClassroom(Classroom model)
        {
            _dbContext.Classroom.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateClassroom(Guid id, Classroom updatedEntity)
        {
            _dbContext.Classroom.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteClassroom(Guid id)
        {
            var entityData = _dbContext.Classroom.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Classroom.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchClassroom(Guid id, JsonPatchDocument<Classroom> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Classroom.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Classroom.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}