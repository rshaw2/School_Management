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
    /// The libraryService responsible for managing library related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting library information.
    /// </remarks>
    public interface ILibraryService
    {
        /// <summary>Retrieves a specific library by its primary key</summary>
        /// <param name="id">The primary key of the library</param>
        /// <returns>The library data</returns>
        Library GetById(Guid id);

        /// <summary>Retrieves a list of librarys based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of librarys</returns>
        List<Library> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new library</summary>
        /// <param name="model">The library data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Library model);

        /// <summary>Updates a specific library by its primary key</summary>
        /// <param name="id">The primary key of the library</param>
        /// <param name="updatedEntity">The library data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Library updatedEntity);

        /// <summary>Updates a specific library by its primary key</summary>
        /// <param name="id">The primary key of the library</param>
        /// <param name="updatedEntity">The library data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Library> updatedEntity);

        /// <summary>Deletes a specific library by its primary key</summary>
        /// <param name="id">The primary key of the library</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The libraryService responsible for managing library related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting library information.
    /// </remarks>
    public class LibraryService : ILibraryService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Library class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public LibraryService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific library by its primary key</summary>
        /// <param name="id">The primary key of the library</param>
        /// <returns>The library data</returns>
        public Library GetById(Guid id)
        {
            var entityData = _dbContext.Library.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of librarys based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of librarys</returns>/// <exception cref="Exception"></exception>
        public List<Library> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetLibrary(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new library</summary>
        /// <param name="model">The library data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Library model)
        {
            model.Id = CreateLibrary(model);
            return model.Id;
        }

        /// <summary>Updates a specific library by its primary key</summary>
        /// <param name="id">The primary key of the library</param>
        /// <param name="updatedEntity">The library data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Library updatedEntity)
        {
            UpdateLibrary(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific library by its primary key</summary>
        /// <param name="id">The primary key of the library</param>
        /// <param name="updatedEntity">The library data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Library> updatedEntity)
        {
            PatchLibrary(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific library by its primary key</summary>
        /// <param name="id">The primary key of the library</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteLibrary(id);
            return true;
        }
        #region
        private List<Library> GetLibrary(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Library.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Library>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Library), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Library, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateLibrary(Library model)
        {
            _dbContext.Library.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateLibrary(Guid id, Library updatedEntity)
        {
            _dbContext.Library.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteLibrary(Guid id)
        {
            var entityData = _dbContext.Library.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Library.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchLibrary(Guid id, JsonPatchDocument<Library> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Library.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Library.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}