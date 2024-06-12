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
    /// The enrollmentService responsible for managing enrollment related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting enrollment information.
    /// </remarks>
    public interface IEnrollmentService
    {
        /// <summary>Retrieves a specific enrollment by its primary key</summary>
        /// <param name="id">The primary key of the enrollment</param>
        /// <returns>The enrollment data</returns>
        Enrollment GetById(Guid id);

        /// <summary>Retrieves a list of enrollments based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of enrollments</returns>
        List<Enrollment> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new enrollment</summary>
        /// <param name="model">The enrollment data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Enrollment model);

        /// <summary>Updates a specific enrollment by its primary key</summary>
        /// <param name="id">The primary key of the enrollment</param>
        /// <param name="updatedEntity">The enrollment data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Enrollment updatedEntity);

        /// <summary>Updates a specific enrollment by its primary key</summary>
        /// <param name="id">The primary key of the enrollment</param>
        /// <param name="updatedEntity">The enrollment data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Enrollment> updatedEntity);

        /// <summary>Deletes a specific enrollment by its primary key</summary>
        /// <param name="id">The primary key of the enrollment</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The enrollmentService responsible for managing enrollment related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting enrollment information.
    /// </remarks>
    public class EnrollmentService : IEnrollmentService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Enrollment class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public EnrollmentService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific enrollment by its primary key</summary>
        /// <param name="id">The primary key of the enrollment</param>
        /// <returns>The enrollment data</returns>
        public Enrollment GetById(Guid id)
        {
            var entityData = _dbContext.Enrollment.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of enrollments based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of enrollments</returns>/// <exception cref="Exception"></exception>
        public List<Enrollment> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetEnrollment(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new enrollment</summary>
        /// <param name="model">The enrollment data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Enrollment model)
        {
            model.Id = CreateEnrollment(model);
            return model.Id;
        }

        /// <summary>Updates a specific enrollment by its primary key</summary>
        /// <param name="id">The primary key of the enrollment</param>
        /// <param name="updatedEntity">The enrollment data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Enrollment updatedEntity)
        {
            UpdateEnrollment(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific enrollment by its primary key</summary>
        /// <param name="id">The primary key of the enrollment</param>
        /// <param name="updatedEntity">The enrollment data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Enrollment> updatedEntity)
        {
            PatchEnrollment(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific enrollment by its primary key</summary>
        /// <param name="id">The primary key of the enrollment</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteEnrollment(id);
            return true;
        }
        #region
        private List<Enrollment> GetEnrollment(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Enrollment.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Enrollment>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Enrollment), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Enrollment, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateEnrollment(Enrollment model)
        {
            _dbContext.Enrollment.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateEnrollment(Guid id, Enrollment updatedEntity)
        {
            _dbContext.Enrollment.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteEnrollment(Guid id)
        {
            var entityData = _dbContext.Enrollment.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Enrollment.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchEnrollment(Guid id, JsonPatchDocument<Enrollment> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Enrollment.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Enrollment.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}