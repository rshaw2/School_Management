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
    /// The resourceService responsible for managing resource related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting resource information.
    /// </remarks>
    public interface IResourceService
    {
        /// <summary>Retrieves a specific resource by its primary key</summary>
        /// <param name="id">The primary key of the resource</param>
        /// <returns>The resource data</returns>
        Resource GetById(Guid id);

        /// <summary>Retrieves a list of resources based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of resources</returns>
        List<Resource> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new resource</summary>
        /// <param name="model">The resource data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Resource model);

        /// <summary>Updates a specific resource by its primary key</summary>
        /// <param name="id">The primary key of the resource</param>
        /// <param name="updatedEntity">The resource data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Resource updatedEntity);

        /// <summary>Updates a specific resource by its primary key</summary>
        /// <param name="id">The primary key of the resource</param>
        /// <param name="updatedEntity">The resource data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Resource> updatedEntity);

        /// <summary>Deletes a specific resource by its primary key</summary>
        /// <param name="id">The primary key of the resource</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The resourceService responsible for managing resource related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting resource information.
    /// </remarks>
    public class ResourceService : IResourceService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Resource class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public ResourceService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific resource by its primary key</summary>
        /// <param name="id">The primary key of the resource</param>
        /// <returns>The resource data</returns>
        public Resource GetById(Guid id)
        {
            var entityData = _dbContext.Resource.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of resources based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of resources</returns>/// <exception cref="Exception"></exception>
        public List<Resource> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetResource(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new resource</summary>
        /// <param name="model">The resource data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Resource model)
        {
            model.Id = CreateResource(model);
            return model.Id;
        }

        /// <summary>Updates a specific resource by its primary key</summary>
        /// <param name="id">The primary key of the resource</param>
        /// <param name="updatedEntity">The resource data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Resource updatedEntity)
        {
            UpdateResource(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific resource by its primary key</summary>
        /// <param name="id">The primary key of the resource</param>
        /// <param name="updatedEntity">The resource data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Resource> updatedEntity)
        {
            PatchResource(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific resource by its primary key</summary>
        /// <param name="id">The primary key of the resource</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteResource(id);
            return true;
        }
        #region
        private List<Resource> GetResource(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Resource.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Resource>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Resource), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Resource, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateResource(Resource model)
        {
            _dbContext.Resource.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateResource(Guid id, Resource updatedEntity)
        {
            _dbContext.Resource.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteResource(Guid id)
        {
            var entityData = _dbContext.Resource.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Resource.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchResource(Guid id, JsonPatchDocument<Resource> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Resource.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Resource.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}