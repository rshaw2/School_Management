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
    /// The folderService responsible for managing folder related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting folder information.
    /// </remarks>
    public interface IFolderService
    {
        /// <summary>Retrieves a specific folder by its primary key</summary>
        /// <param name="id">The primary key of the folder</param>
        /// <returns>The folder data</returns>
        Folder GetById(Guid id);

        /// <summary>Retrieves a list of folders based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of folders</returns>
        List<Folder> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new folder</summary>
        /// <param name="model">The folder data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Folder model);

        /// <summary>Updates a specific folder by its primary key</summary>
        /// <param name="id">The primary key of the folder</param>
        /// <param name="updatedEntity">The folder data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Folder updatedEntity);

        /// <summary>Updates a specific folder by its primary key</summary>
        /// <param name="id">The primary key of the folder</param>
        /// <param name="updatedEntity">The folder data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Folder> updatedEntity);

        /// <summary>Deletes a specific folder by its primary key</summary>
        /// <param name="id">The primary key of the folder</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The folderService responsible for managing folder related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting folder information.
    /// </remarks>
    public class FolderService : IFolderService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Folder class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public FolderService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific folder by its primary key</summary>
        /// <param name="id">The primary key of the folder</param>
        /// <returns>The folder data</returns>
        public Folder GetById(Guid id)
        {
            var entityData = _dbContext.Folder.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of folders based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of folders</returns>/// <exception cref="Exception"></exception>
        public List<Folder> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetFolder(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new folder</summary>
        /// <param name="model">The folder data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Folder model)
        {
            model.Id = CreateFolder(model);
            return model.Id;
        }

        /// <summary>Updates a specific folder by its primary key</summary>
        /// <param name="id">The primary key of the folder</param>
        /// <param name="updatedEntity">The folder data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Folder updatedEntity)
        {
            UpdateFolder(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific folder by its primary key</summary>
        /// <param name="id">The primary key of the folder</param>
        /// <param name="updatedEntity">The folder data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Folder> updatedEntity)
        {
            PatchFolder(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific folder by its primary key</summary>
        /// <param name="id">The primary key of the folder</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteFolder(id);
            return true;
        }
        #region
        private List<Folder> GetFolder(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Folder.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Folder>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Folder), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Folder, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateFolder(Folder model)
        {
            _dbContext.Folder.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateFolder(Guid id, Folder updatedEntity)
        {
            _dbContext.Folder.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteFolder(Guid id)
        {
            var entityData = _dbContext.Folder.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Folder.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchFolder(Guid id, JsonPatchDocument<Folder> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Folder.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Folder.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}