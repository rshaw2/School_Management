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
    /// The documenttemplateService responsible for managing documenttemplate related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting documenttemplate information.
    /// </remarks>
    public interface IDocumentTemplateService
    {
        /// <summary>Retrieves a specific documenttemplate by its primary key</summary>
        /// <param name="id">The primary key of the documenttemplate</param>
        /// <returns>The documenttemplate data</returns>
        DocumentTemplate GetById(Guid id);

        /// <summary>Retrieves a list of documenttemplates based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of documenttemplates</returns>
        List<DocumentTemplate> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new documenttemplate</summary>
        /// <param name="model">The documenttemplate data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(DocumentTemplate model);

        /// <summary>Updates a specific documenttemplate by its primary key</summary>
        /// <param name="id">The primary key of the documenttemplate</param>
        /// <param name="updatedEntity">The documenttemplate data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, DocumentTemplate updatedEntity);

        /// <summary>Updates a specific documenttemplate by its primary key</summary>
        /// <param name="id">The primary key of the documenttemplate</param>
        /// <param name="updatedEntity">The documenttemplate data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<DocumentTemplate> updatedEntity);

        /// <summary>Deletes a specific documenttemplate by its primary key</summary>
        /// <param name="id">The primary key of the documenttemplate</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The documenttemplateService responsible for managing documenttemplate related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting documenttemplate information.
    /// </remarks>
    public class DocumentTemplateService : IDocumentTemplateService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the DocumentTemplate class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public DocumentTemplateService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific documenttemplate by its primary key</summary>
        /// <param name="id">The primary key of the documenttemplate</param>
        /// <returns>The documenttemplate data</returns>
        public DocumentTemplate GetById(Guid id)
        {
            var entityData = _dbContext.DocumentTemplate.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of documenttemplates based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of documenttemplates</returns>/// <exception cref="Exception"></exception>
        public List<DocumentTemplate> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetDocumentTemplate(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new documenttemplate</summary>
        /// <param name="model">The documenttemplate data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(DocumentTemplate model)
        {
            model.Id = CreateDocumentTemplate(model);
            return model.Id;
        }

        /// <summary>Updates a specific documenttemplate by its primary key</summary>
        /// <param name="id">The primary key of the documenttemplate</param>
        /// <param name="updatedEntity">The documenttemplate data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, DocumentTemplate updatedEntity)
        {
            UpdateDocumentTemplate(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific documenttemplate by its primary key</summary>
        /// <param name="id">The primary key of the documenttemplate</param>
        /// <param name="updatedEntity">The documenttemplate data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<DocumentTemplate> updatedEntity)
        {
            PatchDocumentTemplate(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific documenttemplate by its primary key</summary>
        /// <param name="id">The primary key of the documenttemplate</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteDocumentTemplate(id);
            return true;
        }
        #region
        private List<DocumentTemplate> GetDocumentTemplate(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.DocumentTemplate.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<DocumentTemplate>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(DocumentTemplate), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<DocumentTemplate, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateDocumentTemplate(DocumentTemplate model)
        {
            _dbContext.DocumentTemplate.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateDocumentTemplate(Guid id, DocumentTemplate updatedEntity)
        {
            _dbContext.DocumentTemplate.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteDocumentTemplate(Guid id)
        {
            var entityData = _dbContext.DocumentTemplate.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.DocumentTemplate.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchDocumentTemplate(Guid id, JsonPatchDocument<DocumentTemplate> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.DocumentTemplate.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.DocumentTemplate.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}