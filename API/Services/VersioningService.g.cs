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
    /// The versioningService responsible for managing versioning related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting versioning information.
    /// </remarks>
    public interface IVersioningService
    {
        /// <summary>Retrieves a specific versioning by its primary key</summary>
        /// <param name="id">The primary key of the versioning</param>
        /// <returns>The versioning data</returns>
        Versioning GetById(Guid id);

        /// <summary>Retrieves a list of versionings based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of versionings</returns>
        List<Versioning> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new versioning</summary>
        /// <param name="model">The versioning data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Versioning model);

        /// <summary>Updates a specific versioning by its primary key</summary>
        /// <param name="id">The primary key of the versioning</param>
        /// <param name="updatedEntity">The versioning data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Versioning updatedEntity);

        /// <summary>Updates a specific versioning by its primary key</summary>
        /// <param name="id">The primary key of the versioning</param>
        /// <param name="updatedEntity">The versioning data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Versioning> updatedEntity);

        /// <summary>Deletes a specific versioning by its primary key</summary>
        /// <param name="id">The primary key of the versioning</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The versioningService responsible for managing versioning related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting versioning information.
    /// </remarks>
    public class VersioningService : IVersioningService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Versioning class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public VersioningService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific versioning by its primary key</summary>
        /// <param name="id">The primary key of the versioning</param>
        /// <returns>The versioning data</returns>
        public Versioning GetById(Guid id)
        {
            var entityData = _dbContext.Versioning.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of versionings based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of versionings</returns>/// <exception cref="Exception"></exception>
        public List<Versioning> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetVersioning(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new versioning</summary>
        /// <param name="model">The versioning data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Versioning model)
        {
            model.Id = CreateVersioning(model);
            return model.Id;
        }

        /// <summary>Updates a specific versioning by its primary key</summary>
        /// <param name="id">The primary key of the versioning</param>
        /// <param name="updatedEntity">The versioning data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Versioning updatedEntity)
        {
            UpdateVersioning(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific versioning by its primary key</summary>
        /// <param name="id">The primary key of the versioning</param>
        /// <param name="updatedEntity">The versioning data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Versioning> updatedEntity)
        {
            PatchVersioning(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific versioning by its primary key</summary>
        /// <param name="id">The primary key of the versioning</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteVersioning(id);
            return true;
        }
        #region
        private List<Versioning> GetVersioning(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Versioning.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Versioning>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Versioning), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Versioning, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateVersioning(Versioning model)
        {
            _dbContext.Versioning.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateVersioning(Guid id, Versioning updatedEntity)
        {
            _dbContext.Versioning.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteVersioning(Guid id)
        {
            var entityData = _dbContext.Versioning.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Versioning.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchVersioning(Guid id, JsonPatchDocument<Versioning> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Versioning.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Versioning.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}