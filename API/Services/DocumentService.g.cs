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
    /// The documentService responsible for managing document related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting document information.
    /// </remarks>
    public interface IDocumentService
    {
        /// <summary>Retrieves a specific document by its primary key</summary>
        /// <param name="id">The primary key of the document</param>
        /// <returns>The document data</returns>
        Document GetById(Guid id);

        /// <summary>Retrieves a list of documents based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of documents</returns>
        List<Document> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new document</summary>
        /// <param name="model">The document data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Document model);

        /// <summary>Updates a specific document by its primary key</summary>
        /// <param name="id">The primary key of the document</param>
        /// <param name="updatedEntity">The document data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Document updatedEntity);

        /// <summary>Updates a specific document by its primary key</summary>
        /// <param name="id">The primary key of the document</param>
        /// <param name="updatedEntity">The document data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Document> updatedEntity);

        /// <summary>Deletes a specific document by its primary key</summary>
        /// <param name="id">The primary key of the document</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The documentService responsible for managing document related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting document information.
    /// </remarks>
    public class DocumentService : IDocumentService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Document class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public DocumentService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific document by its primary key</summary>
        /// <param name="id">The primary key of the document</param>
        /// <returns>The document data</returns>
        public Document GetById(Guid id)
        {
            var entityData = _dbContext.Document.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of documents based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of documents</returns>/// <exception cref="Exception"></exception>
        public List<Document> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetDocument(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new document</summary>
        /// <param name="model">The document data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Document model)
        {
            model.Id = CreateDocument(model);
            return model.Id;
        }

        /// <summary>Updates a specific document by its primary key</summary>
        /// <param name="id">The primary key of the document</param>
        /// <param name="updatedEntity">The document data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Document updatedEntity)
        {
            UpdateDocument(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific document by its primary key</summary>
        /// <param name="id">The primary key of the document</param>
        /// <param name="updatedEntity">The document data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Document> updatedEntity)
        {
            PatchDocument(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific document by its primary key</summary>
        /// <param name="id">The primary key of the document</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteDocument(id);
            return true;
        }
        #region
        private List<Document> GetDocument(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Document.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Document>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Document), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Document, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateDocument(Document model)
        {
            _dbContext.Document.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateDocument(Guid id, Document updatedEntity)
        {
            _dbContext.Document.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteDocument(Guid id)
        {
            var entityData = _dbContext.Document.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Document.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchDocument(Guid id, JsonPatchDocument<Document> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Document.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Document.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}