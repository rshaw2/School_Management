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
    /// The schoolService responsible for managing school related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting school information.
    /// </remarks>
    public interface ISchoolService
    {
        /// <summary>Retrieves a specific school by its primary key</summary>
        /// <param name="id">The primary key of the school</param>
        /// <returns>The school data</returns>
        School GetById(Guid id);

        /// <summary>Retrieves a list of schools based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of schools</returns>
        List<School> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new school</summary>
        /// <param name="model">The school data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(School model);

        /// <summary>Updates a specific school by its primary key</summary>
        /// <param name="id">The primary key of the school</param>
        /// <param name="updatedEntity">The school data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, School updatedEntity);

        /// <summary>Updates a specific school by its primary key</summary>
        /// <param name="id">The primary key of the school</param>
        /// <param name="updatedEntity">The school data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<School> updatedEntity);

        /// <summary>Deletes a specific school by its primary key</summary>
        /// <param name="id">The primary key of the school</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The schoolService responsible for managing school related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting school information.
    /// </remarks>
    public class SchoolService : ISchoolService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the School class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public SchoolService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific school by its primary key</summary>
        /// <param name="id">The primary key of the school</param>
        /// <returns>The school data</returns>
        public School GetById(Guid id)
        {
            var entityData = _dbContext.School.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of schools based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of schools</returns>/// <exception cref="Exception"></exception>
        public List<School> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetSchool(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new school</summary>
        /// <param name="model">The school data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(School model)
        {
            model.Id = CreateSchool(model);
            return model.Id;
        }

        /// <summary>Updates a specific school by its primary key</summary>
        /// <param name="id">The primary key of the school</param>
        /// <param name="updatedEntity">The school data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, School updatedEntity)
        {
            UpdateSchool(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific school by its primary key</summary>
        /// <param name="id">The primary key of the school</param>
        /// <param name="updatedEntity">The school data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<School> updatedEntity)
        {
            PatchSchool(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific school by its primary key</summary>
        /// <param name="id">The primary key of the school</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteSchool(id);
            return true;
        }
        #region
        private List<School> GetSchool(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.School.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<School>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(School), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<School, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateSchool(School model)
        {
            _dbContext.School.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateSchool(Guid id, School updatedEntity)
        {
            _dbContext.School.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteSchool(Guid id)
        {
            var entityData = _dbContext.School.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.School.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchSchool(Guid id, JsonPatchDocument<School> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.School.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.School.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}