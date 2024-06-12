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
    /// The permissionsService responsible for managing permissions related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting permissions information.
    /// </remarks>
    public interface IPermissionsService
    {
        /// <summary>Retrieves a specific permissions by its primary key</summary>
        /// <param name="id">The primary key of the permissions</param>
        /// <returns>The permissions data</returns>
        Permissions GetById(Guid id);

        /// <summary>Retrieves a list of permissionss based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of permissionss</returns>
        List<Permissions> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new permissions</summary>
        /// <param name="model">The permissions data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Permissions model);

        /// <summary>Updates a specific permissions by its primary key</summary>
        /// <param name="id">The primary key of the permissions</param>
        /// <param name="updatedEntity">The permissions data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Permissions updatedEntity);

        /// <summary>Updates a specific permissions by its primary key</summary>
        /// <param name="id">The primary key of the permissions</param>
        /// <param name="updatedEntity">The permissions data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Permissions> updatedEntity);

        /// <summary>Deletes a specific permissions by its primary key</summary>
        /// <param name="id">The primary key of the permissions</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The permissionsService responsible for managing permissions related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting permissions information.
    /// </remarks>
    public class PermissionsService : IPermissionsService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Permissions class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public PermissionsService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific permissions by its primary key</summary>
        /// <param name="id">The primary key of the permissions</param>
        /// <returns>The permissions data</returns>
        public Permissions GetById(Guid id)
        {
            var entityData = _dbContext.Permissions.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of permissionss based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of permissionss</returns>/// <exception cref="Exception"></exception>
        public List<Permissions> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetPermissions(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new permissions</summary>
        /// <param name="model">The permissions data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Permissions model)
        {
            model.Id = CreatePermissions(model);
            return model.Id;
        }

        /// <summary>Updates a specific permissions by its primary key</summary>
        /// <param name="id">The primary key of the permissions</param>
        /// <param name="updatedEntity">The permissions data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Permissions updatedEntity)
        {
            UpdatePermissions(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific permissions by its primary key</summary>
        /// <param name="id">The primary key of the permissions</param>
        /// <param name="updatedEntity">The permissions data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Permissions> updatedEntity)
        {
            PatchPermissions(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific permissions by its primary key</summary>
        /// <param name="id">The primary key of the permissions</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeletePermissions(id);
            return true;
        }
        #region
        private List<Permissions> GetPermissions(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Permissions.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Permissions>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Permissions), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Permissions, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreatePermissions(Permissions model)
        {
            _dbContext.Permissions.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdatePermissions(Guid id, Permissions updatedEntity)
        {
            _dbContext.Permissions.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeletePermissions(Guid id)
        {
            var entityData = _dbContext.Permissions.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Permissions.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchPermissions(Guid id, JsonPatchDocument<Permissions> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Permissions.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Permissions.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}