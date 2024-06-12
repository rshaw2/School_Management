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
    /// The instructorService responsible for managing instructor related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting instructor information.
    /// </remarks>
    public interface IInstructorService
    {
        /// <summary>Retrieves a specific instructor by its primary key</summary>
        /// <param name="id">The primary key of the instructor</param>
        /// <returns>The instructor data</returns>
        Instructor GetById(Guid id);

        /// <summary>Retrieves a list of instructors based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of instructors</returns>
        List<Instructor> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new instructor</summary>
        /// <param name="model">The instructor data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Instructor model);

        /// <summary>Updates a specific instructor by its primary key</summary>
        /// <param name="id">The primary key of the instructor</param>
        /// <param name="updatedEntity">The instructor data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Instructor updatedEntity);

        /// <summary>Updates a specific instructor by its primary key</summary>
        /// <param name="id">The primary key of the instructor</param>
        /// <param name="updatedEntity">The instructor data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Instructor> updatedEntity);

        /// <summary>Deletes a specific instructor by its primary key</summary>
        /// <param name="id">The primary key of the instructor</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The instructorService responsible for managing instructor related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting instructor information.
    /// </remarks>
    public class InstructorService : IInstructorService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Instructor class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public InstructorService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific instructor by its primary key</summary>
        /// <param name="id">The primary key of the instructor</param>
        /// <returns>The instructor data</returns>
        public Instructor GetById(Guid id)
        {
            var entityData = _dbContext.Instructor.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of instructors based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of instructors</returns>/// <exception cref="Exception"></exception>
        public List<Instructor> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetInstructor(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new instructor</summary>
        /// <param name="model">The instructor data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Instructor model)
        {
            model.Id = CreateInstructor(model);
            return model.Id;
        }

        /// <summary>Updates a specific instructor by its primary key</summary>
        /// <param name="id">The primary key of the instructor</param>
        /// <param name="updatedEntity">The instructor data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Instructor updatedEntity)
        {
            UpdateInstructor(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific instructor by its primary key</summary>
        /// <param name="id">The primary key of the instructor</param>
        /// <param name="updatedEntity">The instructor data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Instructor> updatedEntity)
        {
            PatchInstructor(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific instructor by its primary key</summary>
        /// <param name="id">The primary key of the instructor</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteInstructor(id);
            return true;
        }
        #region
        private List<Instructor> GetInstructor(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Instructor.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Instructor>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Instructor), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Instructor, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateInstructor(Instructor model)
        {
            _dbContext.Instructor.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateInstructor(Guid id, Instructor updatedEntity)
        {
            _dbContext.Instructor.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteInstructor(Guid id)
        {
            var entityData = _dbContext.Instructor.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Instructor.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchInstructor(Guid id, JsonPatchDocument<Instructor> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Instructor.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Instructor.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}