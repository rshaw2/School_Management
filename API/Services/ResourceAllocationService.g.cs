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
    /// The resourceallocationService responsible for managing resourceallocation related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting resourceallocation information.
    /// </remarks>
    public interface IResourceAllocationService
    {
        /// <summary>Retrieves a specific resourceallocation by its primary key</summary>
        /// <param name="id">The primary key of the resourceallocation</param>
        /// <returns>The resourceallocation data</returns>
        ResourceAllocation GetById(Guid id);

        /// <summary>Retrieves a list of resourceallocations based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of resourceallocations</returns>
        List<ResourceAllocation> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new resourceallocation</summary>
        /// <param name="model">The resourceallocation data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(ResourceAllocation model);

        /// <summary>Updates a specific resourceallocation by its primary key</summary>
        /// <param name="id">The primary key of the resourceallocation</param>
        /// <param name="updatedEntity">The resourceallocation data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, ResourceAllocation updatedEntity);

        /// <summary>Updates a specific resourceallocation by its primary key</summary>
        /// <param name="id">The primary key of the resourceallocation</param>
        /// <param name="updatedEntity">The resourceallocation data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<ResourceAllocation> updatedEntity);

        /// <summary>Deletes a specific resourceallocation by its primary key</summary>
        /// <param name="id">The primary key of the resourceallocation</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The resourceallocationService responsible for managing resourceallocation related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting resourceallocation information.
    /// </remarks>
    public class ResourceAllocationService : IResourceAllocationService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the ResourceAllocation class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public ResourceAllocationService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific resourceallocation by its primary key</summary>
        /// <param name="id">The primary key of the resourceallocation</param>
        /// <returns>The resourceallocation data</returns>
        public ResourceAllocation GetById(Guid id)
        {
            var entityData = _dbContext.ResourceAllocation.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of resourceallocations based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of resourceallocations</returns>/// <exception cref="Exception"></exception>
        public List<ResourceAllocation> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetResourceAllocation(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new resourceallocation</summary>
        /// <param name="model">The resourceallocation data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(ResourceAllocation model)
        {
            model.Id = CreateResourceAllocation(model);
            return model.Id;
        }

        /// <summary>Updates a specific resourceallocation by its primary key</summary>
        /// <param name="id">The primary key of the resourceallocation</param>
        /// <param name="updatedEntity">The resourceallocation data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, ResourceAllocation updatedEntity)
        {
            UpdateResourceAllocation(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific resourceallocation by its primary key</summary>
        /// <param name="id">The primary key of the resourceallocation</param>
        /// <param name="updatedEntity">The resourceallocation data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<ResourceAllocation> updatedEntity)
        {
            PatchResourceAllocation(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific resourceallocation by its primary key</summary>
        /// <param name="id">The primary key of the resourceallocation</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteResourceAllocation(id);
            return true;
        }
        #region
        private List<ResourceAllocation> GetResourceAllocation(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.ResourceAllocation.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<ResourceAllocation>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(ResourceAllocation), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<ResourceAllocation, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateResourceAllocation(ResourceAllocation model)
        {
            _dbContext.ResourceAllocation.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateResourceAllocation(Guid id, ResourceAllocation updatedEntity)
        {
            _dbContext.ResourceAllocation.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteResourceAllocation(Guid id)
        {
            var entityData = _dbContext.ResourceAllocation.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.ResourceAllocation.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchResourceAllocation(Guid id, JsonPatchDocument<ResourceAllocation> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.ResourceAllocation.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.ResourceAllocation.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}