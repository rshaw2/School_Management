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
    /// The filestorageService responsible for managing filestorage related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting filestorage information.
    /// </remarks>
    public interface IFileStorageService
    {
        /// <summary>Retrieves a specific filestorage by its primary key</summary>
        /// <param name="id">The primary key of the filestorage</param>
        /// <returns>The filestorage data</returns>
        FileStorage GetById(Guid id);

        /// <summary>Retrieves a list of filestorages based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of filestorages</returns>
        List<FileStorage> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new filestorage</summary>
        /// <param name="model">The filestorage data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(FileStorage model);

        /// <summary>Updates a specific filestorage by its primary key</summary>
        /// <param name="id">The primary key of the filestorage</param>
        /// <param name="updatedEntity">The filestorage data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, FileStorage updatedEntity);

        /// <summary>Updates a specific filestorage by its primary key</summary>
        /// <param name="id">The primary key of the filestorage</param>
        /// <param name="updatedEntity">The filestorage data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<FileStorage> updatedEntity);

        /// <summary>Deletes a specific filestorage by its primary key</summary>
        /// <param name="id">The primary key of the filestorage</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The filestorageService responsible for managing filestorage related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting filestorage information.
    /// </remarks>
    public class FileStorageService : IFileStorageService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the FileStorage class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public FileStorageService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific filestorage by its primary key</summary>
        /// <param name="id">The primary key of the filestorage</param>
        /// <returns>The filestorage data</returns>
        public FileStorage GetById(Guid id)
        {
            var entityData = _dbContext.FileStorage.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of filestorages based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of filestorages</returns>/// <exception cref="Exception"></exception>
        public List<FileStorage> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetFileStorage(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new filestorage</summary>
        /// <param name="model">The filestorage data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(FileStorage model)
        {
            model.Id = CreateFileStorage(model);
            return model.Id;
        }

        /// <summary>Updates a specific filestorage by its primary key</summary>
        /// <param name="id">The primary key of the filestorage</param>
        /// <param name="updatedEntity">The filestorage data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, FileStorage updatedEntity)
        {
            UpdateFileStorage(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific filestorage by its primary key</summary>
        /// <param name="id">The primary key of the filestorage</param>
        /// <param name="updatedEntity">The filestorage data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<FileStorage> updatedEntity)
        {
            PatchFileStorage(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific filestorage by its primary key</summary>
        /// <param name="id">The primary key of the filestorage</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteFileStorage(id);
            return true;
        }
        #region
        private List<FileStorage> GetFileStorage(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.FileStorage.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<FileStorage>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(FileStorage), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<FileStorage, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateFileStorage(FileStorage model)
        {
            _dbContext.FileStorage.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateFileStorage(Guid id, FileStorage updatedEntity)
        {
            _dbContext.FileStorage.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteFileStorage(Guid id)
        {
            var entityData = _dbContext.FileStorage.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.FileStorage.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchFileStorage(Guid id, JsonPatchDocument<FileStorage> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.FileStorage.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.FileStorage.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}