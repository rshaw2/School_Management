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
    /// The documentversionService responsible for managing documentversion related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting documentversion information.
    /// </remarks>
    public interface IDocumentVersionService
    {
        /// <summary>Retrieves a specific documentversion by its primary key</summary>
        /// <param name="id">The primary key of the documentversion</param>
        /// <returns>The documentversion data</returns>
        DocumentVersion GetById(Guid id);

        /// <summary>Retrieves a list of documentversions based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of documentversions</returns>
        List<DocumentVersion> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new documentversion</summary>
        /// <param name="model">The documentversion data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(DocumentVersion model);

        /// <summary>Updates a specific documentversion by its primary key</summary>
        /// <param name="id">The primary key of the documentversion</param>
        /// <param name="updatedEntity">The documentversion data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, DocumentVersion updatedEntity);

        /// <summary>Updates a specific documentversion by its primary key</summary>
        /// <param name="id">The primary key of the documentversion</param>
        /// <param name="updatedEntity">The documentversion data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<DocumentVersion> updatedEntity);

        /// <summary>Deletes a specific documentversion by its primary key</summary>
        /// <param name="id">The primary key of the documentversion</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The documentversionService responsible for managing documentversion related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting documentversion information.
    /// </remarks>
    public class DocumentVersionService : IDocumentVersionService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the DocumentVersion class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public DocumentVersionService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific documentversion by its primary key</summary>
        /// <param name="id">The primary key of the documentversion</param>
        /// <returns>The documentversion data</returns>
        public DocumentVersion GetById(Guid id)
        {
            var entityData = _dbContext.DocumentVersion.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of documentversions based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of documentversions</returns>/// <exception cref="Exception"></exception>
        public List<DocumentVersion> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetDocumentVersion(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new documentversion</summary>
        /// <param name="model">The documentversion data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(DocumentVersion model)
        {
            model.Id = CreateDocumentVersion(model);
            return model.Id;
        }

        /// <summary>Updates a specific documentversion by its primary key</summary>
        /// <param name="id">The primary key of the documentversion</param>
        /// <param name="updatedEntity">The documentversion data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, DocumentVersion updatedEntity)
        {
            UpdateDocumentVersion(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific documentversion by its primary key</summary>
        /// <param name="id">The primary key of the documentversion</param>
        /// <param name="updatedEntity">The documentversion data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<DocumentVersion> updatedEntity)
        {
            PatchDocumentVersion(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific documentversion by its primary key</summary>
        /// <param name="id">The primary key of the documentversion</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteDocumentVersion(id);
            return true;
        }
        #region
        private List<DocumentVersion> GetDocumentVersion(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.DocumentVersion.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<DocumentVersion>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(DocumentVersion), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<DocumentVersion, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateDocumentVersion(DocumentVersion model)
        {
            _dbContext.DocumentVersion.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateDocumentVersion(Guid id, DocumentVersion updatedEntity)
        {
            _dbContext.DocumentVersion.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteDocumentVersion(Guid id)
        {
            var entityData = _dbContext.DocumentVersion.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.DocumentVersion.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchDocumentVersion(Guid id, JsonPatchDocument<DocumentVersion> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.DocumentVersion.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.DocumentVersion.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}