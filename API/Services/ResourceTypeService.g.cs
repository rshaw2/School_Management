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
    /// The resourcetypeService responsible for managing resourcetype related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting resourcetype information.
    /// </remarks>
    public interface IResourceTypeService
    {
        /// <summary>Retrieves a specific resourcetype by its primary key</summary>
        /// <param name="id">The primary key of the resourcetype</param>
        /// <returns>The resourcetype data</returns>
        ResourceType GetById(Guid id);

        /// <summary>Retrieves a list of resourcetypes based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of resourcetypes</returns>
        List<ResourceType> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new resourcetype</summary>
        /// <param name="model">The resourcetype data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(ResourceType model);

        /// <summary>Updates a specific resourcetype by its primary key</summary>
        /// <param name="id">The primary key of the resourcetype</param>
        /// <param name="updatedEntity">The resourcetype data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, ResourceType updatedEntity);

        /// <summary>Updates a specific resourcetype by its primary key</summary>
        /// <param name="id">The primary key of the resourcetype</param>
        /// <param name="updatedEntity">The resourcetype data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<ResourceType> updatedEntity);

        /// <summary>Deletes a specific resourcetype by its primary key</summary>
        /// <param name="id">The primary key of the resourcetype</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The resourcetypeService responsible for managing resourcetype related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting resourcetype information.
    /// </remarks>
    public class ResourceTypeService : IResourceTypeService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the ResourceType class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public ResourceTypeService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific resourcetype by its primary key</summary>
        /// <param name="id">The primary key of the resourcetype</param>
        /// <returns>The resourcetype data</returns>
        public ResourceType GetById(Guid id)
        {
            var entityData = _dbContext.ResourceType.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of resourcetypes based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of resourcetypes</returns>/// <exception cref="Exception"></exception>
        public List<ResourceType> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetResourceType(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new resourcetype</summary>
        /// <param name="model">The resourcetype data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(ResourceType model)
        {
            model.Id = CreateResourceType(model);
            return model.Id;
        }

        /// <summary>Updates a specific resourcetype by its primary key</summary>
        /// <param name="id">The primary key of the resourcetype</param>
        /// <param name="updatedEntity">The resourcetype data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, ResourceType updatedEntity)
        {
            UpdateResourceType(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific resourcetype by its primary key</summary>
        /// <param name="id">The primary key of the resourcetype</param>
        /// <param name="updatedEntity">The resourcetype data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<ResourceType> updatedEntity)
        {
            PatchResourceType(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific resourcetype by its primary key</summary>
        /// <param name="id">The primary key of the resourcetype</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteResourceType(id);
            return true;
        }
        #region
        private List<ResourceType> GetResourceType(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.ResourceType.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<ResourceType>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(ResourceType), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<ResourceType, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateResourceType(ResourceType model)
        {
            _dbContext.ResourceType.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateResourceType(Guid id, ResourceType updatedEntity)
        {
            _dbContext.ResourceType.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteResourceType(Guid id)
        {
            var entityData = _dbContext.ResourceType.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.ResourceType.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchResourceType(Guid id, JsonPatchDocument<ResourceType> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.ResourceType.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.ResourceType.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}