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
    /// The storageproviderService responsible for managing storageprovider related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting storageprovider information.
    /// </remarks>
    public interface IStorageProviderService
    {
        /// <summary>Retrieves a specific storageprovider by its primary key</summary>
        /// <param name="id">The primary key of the storageprovider</param>
        /// <returns>The storageprovider data</returns>
        StorageProvider GetById(Guid id);

        /// <summary>Retrieves a list of storageproviders based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of storageproviders</returns>
        List<StorageProvider> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new storageprovider</summary>
        /// <param name="model">The storageprovider data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(StorageProvider model);

        /// <summary>Updates a specific storageprovider by its primary key</summary>
        /// <param name="id">The primary key of the storageprovider</param>
        /// <param name="updatedEntity">The storageprovider data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, StorageProvider updatedEntity);

        /// <summary>Updates a specific storageprovider by its primary key</summary>
        /// <param name="id">The primary key of the storageprovider</param>
        /// <param name="updatedEntity">The storageprovider data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<StorageProvider> updatedEntity);

        /// <summary>Deletes a specific storageprovider by its primary key</summary>
        /// <param name="id">The primary key of the storageprovider</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The storageproviderService responsible for managing storageprovider related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting storageprovider information.
    /// </remarks>
    public class StorageProviderService : IStorageProviderService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the StorageProvider class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public StorageProviderService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific storageprovider by its primary key</summary>
        /// <param name="id">The primary key of the storageprovider</param>
        /// <returns>The storageprovider data</returns>
        public StorageProvider GetById(Guid id)
        {
            var entityData = _dbContext.StorageProvider.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of storageproviders based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of storageproviders</returns>/// <exception cref="Exception"></exception>
        public List<StorageProvider> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetStorageProvider(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new storageprovider</summary>
        /// <param name="model">The storageprovider data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(StorageProvider model)
        {
            model.Id = CreateStorageProvider(model);
            return model.Id;
        }

        /// <summary>Updates a specific storageprovider by its primary key</summary>
        /// <param name="id">The primary key of the storageprovider</param>
        /// <param name="updatedEntity">The storageprovider data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, StorageProvider updatedEntity)
        {
            UpdateStorageProvider(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific storageprovider by its primary key</summary>
        /// <param name="id">The primary key of the storageprovider</param>
        /// <param name="updatedEntity">The storageprovider data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<StorageProvider> updatedEntity)
        {
            PatchStorageProvider(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific storageprovider by its primary key</summary>
        /// <param name="id">The primary key of the storageprovider</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteStorageProvider(id);
            return true;
        }
        #region
        private List<StorageProvider> GetStorageProvider(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.StorageProvider.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<StorageProvider>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(StorageProvider), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<StorageProvider, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateStorageProvider(StorageProvider model)
        {
            _dbContext.StorageProvider.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateStorageProvider(Guid id, StorageProvider updatedEntity)
        {
            _dbContext.StorageProvider.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteStorageProvider(Guid id)
        {
            var entityData = _dbContext.StorageProvider.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.StorageProvider.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchStorageProvider(Guid id, JsonPatchDocument<StorageProvider> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.StorageProvider.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.StorageProvider.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}