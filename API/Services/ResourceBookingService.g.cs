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
    /// The resourcebookingService responsible for managing resourcebooking related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting resourcebooking information.
    /// </remarks>
    public interface IResourceBookingService
    {
        /// <summary>Retrieves a specific resourcebooking by its primary key</summary>
        /// <param name="id">The primary key of the resourcebooking</param>
        /// <returns>The resourcebooking data</returns>
        ResourceBooking GetById(Guid id);

        /// <summary>Retrieves a list of resourcebookings based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of resourcebookings</returns>
        List<ResourceBooking> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new resourcebooking</summary>
        /// <param name="model">The resourcebooking data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(ResourceBooking model);

        /// <summary>Updates a specific resourcebooking by its primary key</summary>
        /// <param name="id">The primary key of the resourcebooking</param>
        /// <param name="updatedEntity">The resourcebooking data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, ResourceBooking updatedEntity);

        /// <summary>Updates a specific resourcebooking by its primary key</summary>
        /// <param name="id">The primary key of the resourcebooking</param>
        /// <param name="updatedEntity">The resourcebooking data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<ResourceBooking> updatedEntity);

        /// <summary>Deletes a specific resourcebooking by its primary key</summary>
        /// <param name="id">The primary key of the resourcebooking</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The resourcebookingService responsible for managing resourcebooking related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting resourcebooking information.
    /// </remarks>
    public class ResourceBookingService : IResourceBookingService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the ResourceBooking class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public ResourceBookingService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific resourcebooking by its primary key</summary>
        /// <param name="id">The primary key of the resourcebooking</param>
        /// <returns>The resourcebooking data</returns>
        public ResourceBooking GetById(Guid id)
        {
            var entityData = _dbContext.ResourceBooking.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of resourcebookings based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of resourcebookings</returns>/// <exception cref="Exception"></exception>
        public List<ResourceBooking> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetResourceBooking(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new resourcebooking</summary>
        /// <param name="model">The resourcebooking data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(ResourceBooking model)
        {
            model.Id = CreateResourceBooking(model);
            return model.Id;
        }

        /// <summary>Updates a specific resourcebooking by its primary key</summary>
        /// <param name="id">The primary key of the resourcebooking</param>
        /// <param name="updatedEntity">The resourcebooking data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, ResourceBooking updatedEntity)
        {
            UpdateResourceBooking(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific resourcebooking by its primary key</summary>
        /// <param name="id">The primary key of the resourcebooking</param>
        /// <param name="updatedEntity">The resourcebooking data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<ResourceBooking> updatedEntity)
        {
            PatchResourceBooking(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific resourcebooking by its primary key</summary>
        /// <param name="id">The primary key of the resourcebooking</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteResourceBooking(id);
            return true;
        }
        #region
        private List<ResourceBooking> GetResourceBooking(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.ResourceBooking.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<ResourceBooking>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(ResourceBooking), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<ResourceBooking, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateResourceBooking(ResourceBooking model)
        {
            _dbContext.ResourceBooking.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateResourceBooking(Guid id, ResourceBooking updatedEntity)
        {
            _dbContext.ResourceBooking.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteResourceBooking(Guid id)
        {
            var entityData = _dbContext.ResourceBooking.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.ResourceBooking.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchResourceBooking(Guid id, JsonPatchDocument<ResourceBooking> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.ResourceBooking.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.ResourceBooking.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}