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
    /// The gradeService responsible for managing grade related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting grade information.
    /// </remarks>
    public interface IGradeService
    {
        /// <summary>Retrieves a specific grade by its primary key</summary>
        /// <param name="id">The primary key of the grade</param>
        /// <returns>The grade data</returns>
        Grade GetById(Guid id);

        /// <summary>Retrieves a list of grades based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of grades</returns>
        List<Grade> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new grade</summary>
        /// <param name="model">The grade data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Grade model);

        /// <summary>Updates a specific grade by its primary key</summary>
        /// <param name="id">The primary key of the grade</param>
        /// <param name="updatedEntity">The grade data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Grade updatedEntity);

        /// <summary>Updates a specific grade by its primary key</summary>
        /// <param name="id">The primary key of the grade</param>
        /// <param name="updatedEntity">The grade data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Grade> updatedEntity);

        /// <summary>Deletes a specific grade by its primary key</summary>
        /// <param name="id">The primary key of the grade</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The gradeService responsible for managing grade related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting grade information.
    /// </remarks>
    public class GradeService : IGradeService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Grade class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public GradeService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific grade by its primary key</summary>
        /// <param name="id">The primary key of the grade</param>
        /// <returns>The grade data</returns>
        public Grade GetById(Guid id)
        {
            var entityData = _dbContext.Grade.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of grades based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of grades</returns>/// <exception cref="Exception"></exception>
        public List<Grade> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetGrade(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new grade</summary>
        /// <param name="model">The grade data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Grade model)
        {
            model.Id = CreateGrade(model);
            return model.Id;
        }

        /// <summary>Updates a specific grade by its primary key</summary>
        /// <param name="id">The primary key of the grade</param>
        /// <param name="updatedEntity">The grade data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Grade updatedEntity)
        {
            UpdateGrade(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific grade by its primary key</summary>
        /// <param name="id">The primary key of the grade</param>
        /// <param name="updatedEntity">The grade data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Grade> updatedEntity)
        {
            PatchGrade(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific grade by its primary key</summary>
        /// <param name="id">The primary key of the grade</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteGrade(id);
            return true;
        }
        #region
        private List<Grade> GetGrade(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Grade.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Grade>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Grade), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Grade, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateGrade(Grade model)
        {
            _dbContext.Grade.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateGrade(Guid id, Grade updatedEntity)
        {
            _dbContext.Grade.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteGrade(Guid id)
        {
            var entityData = _dbContext.Grade.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Grade.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchGrade(Guid id, JsonPatchDocument<Grade> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Grade.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Grade.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}