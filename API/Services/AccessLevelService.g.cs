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
    /// The accesslevelService responsible for managing accesslevel related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting accesslevel information.
    /// </remarks>
    public interface IAccessLevelService
    {
        /// <summary>Retrieves a specific accesslevel by its primary key</summary>
        /// <param name="id">The primary key of the accesslevel</param>
        /// <returns>The accesslevel data</returns>
        AccessLevel GetById(Guid id);

        /// <summary>Retrieves a list of accesslevels based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of accesslevels</returns>
        List<AccessLevel> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new accesslevel</summary>
        /// <param name="model">The accesslevel data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(AccessLevel model);

        /// <summary>Updates a specific accesslevel by its primary key</summary>
        /// <param name="id">The primary key of the accesslevel</param>
        /// <param name="updatedEntity">The accesslevel data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, AccessLevel updatedEntity);

        /// <summary>Updates a specific accesslevel by its primary key</summary>
        /// <param name="id">The primary key of the accesslevel</param>
        /// <param name="updatedEntity">The accesslevel data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<AccessLevel> updatedEntity);

        /// <summary>Deletes a specific accesslevel by its primary key</summary>
        /// <param name="id">The primary key of the accesslevel</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The accesslevelService responsible for managing accesslevel related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting accesslevel information.
    /// </remarks>
    public class AccessLevelService : IAccessLevelService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the AccessLevel class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public AccessLevelService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific accesslevel by its primary key</summary>
        /// <param name="id">The primary key of the accesslevel</param>
        /// <returns>The accesslevel data</returns>
        public AccessLevel GetById(Guid id)
        {
            var entityData = _dbContext.AccessLevel.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of accesslevels based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of accesslevels</returns>/// <exception cref="Exception"></exception>
        public List<AccessLevel> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetAccessLevel(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new accesslevel</summary>
        /// <param name="model">The accesslevel data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(AccessLevel model)
        {
            model.Id = CreateAccessLevel(model);
            return model.Id;
        }

        /// <summary>Updates a specific accesslevel by its primary key</summary>
        /// <param name="id">The primary key of the accesslevel</param>
        /// <param name="updatedEntity">The accesslevel data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, AccessLevel updatedEntity)
        {
            UpdateAccessLevel(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific accesslevel by its primary key</summary>
        /// <param name="id">The primary key of the accesslevel</param>
        /// <param name="updatedEntity">The accesslevel data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<AccessLevel> updatedEntity)
        {
            PatchAccessLevel(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific accesslevel by its primary key</summary>
        /// <param name="id">The primary key of the accesslevel</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteAccessLevel(id);
            return true;
        }
        #region
        private List<AccessLevel> GetAccessLevel(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.AccessLevel.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<AccessLevel>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(AccessLevel), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<AccessLevel, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateAccessLevel(AccessLevel model)
        {
            _dbContext.AccessLevel.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateAccessLevel(Guid id, AccessLevel updatedEntity)
        {
            _dbContext.AccessLevel.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteAccessLevel(Guid id)
        {
            var entityData = _dbContext.AccessLevel.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.AccessLevel.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchAccessLevel(Guid id, JsonPatchDocument<AccessLevel> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.AccessLevel.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.AccessLevel.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}