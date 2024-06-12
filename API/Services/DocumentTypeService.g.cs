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
    /// The documenttypeService responsible for managing documenttype related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting documenttype information.
    /// </remarks>
    public interface IDocumentTypeService
    {
        /// <summary>Retrieves a specific documenttype by its primary key</summary>
        /// <param name="id">The primary key of the documenttype</param>
        /// <returns>The documenttype data</returns>
        DocumentType GetById(Guid id);

        /// <summary>Retrieves a list of documenttypes based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of documenttypes</returns>
        List<DocumentType> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new documenttype</summary>
        /// <param name="model">The documenttype data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(DocumentType model);

        /// <summary>Updates a specific documenttype by its primary key</summary>
        /// <param name="id">The primary key of the documenttype</param>
        /// <param name="updatedEntity">The documenttype data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, DocumentType updatedEntity);

        /// <summary>Updates a specific documenttype by its primary key</summary>
        /// <param name="id">The primary key of the documenttype</param>
        /// <param name="updatedEntity">The documenttype data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<DocumentType> updatedEntity);

        /// <summary>Deletes a specific documenttype by its primary key</summary>
        /// <param name="id">The primary key of the documenttype</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The documenttypeService responsible for managing documenttype related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting documenttype information.
    /// </remarks>
    public class DocumentTypeService : IDocumentTypeService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the DocumentType class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public DocumentTypeService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific documenttype by its primary key</summary>
        /// <param name="id">The primary key of the documenttype</param>
        /// <returns>The documenttype data</returns>
        public DocumentType GetById(Guid id)
        {
            var entityData = _dbContext.DocumentType.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of documenttypes based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of documenttypes</returns>/// <exception cref="Exception"></exception>
        public List<DocumentType> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetDocumentType(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new documenttype</summary>
        /// <param name="model">The documenttype data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(DocumentType model)
        {
            model.Id = CreateDocumentType(model);
            return model.Id;
        }

        /// <summary>Updates a specific documenttype by its primary key</summary>
        /// <param name="id">The primary key of the documenttype</param>
        /// <param name="updatedEntity">The documenttype data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, DocumentType updatedEntity)
        {
            UpdateDocumentType(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific documenttype by its primary key</summary>
        /// <param name="id">The primary key of the documenttype</param>
        /// <param name="updatedEntity">The documenttype data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<DocumentType> updatedEntity)
        {
            PatchDocumentType(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific documenttype by its primary key</summary>
        /// <param name="id">The primary key of the documenttype</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteDocumentType(id);
            return true;
        }
        #region
        private List<DocumentType> GetDocumentType(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.DocumentType.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<DocumentType>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(DocumentType), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<DocumentType, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateDocumentType(DocumentType model)
        {
            _dbContext.DocumentType.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateDocumentType(Guid id, DocumentType updatedEntity)
        {
            _dbContext.DocumentType.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteDocumentType(Guid id)
        {
            var entityData = _dbContext.DocumentType.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.DocumentType.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchDocumentType(Guid id, JsonPatchDocument<DocumentType> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.DocumentType.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.DocumentType.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}