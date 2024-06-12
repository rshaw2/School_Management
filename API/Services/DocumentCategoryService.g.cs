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
    /// The documentcategoryService responsible for managing documentcategory related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting documentcategory information.
    /// </remarks>
    public interface IDocumentCategoryService
    {
        /// <summary>Retrieves a specific documentcategory by its primary key</summary>
        /// <param name="id">The primary key of the documentcategory</param>
        /// <returns>The documentcategory data</returns>
        DocumentCategory GetById(Guid id);

        /// <summary>Retrieves a list of documentcategorys based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of documentcategorys</returns>
        List<DocumentCategory> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new documentcategory</summary>
        /// <param name="model">The documentcategory data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(DocumentCategory model);

        /// <summary>Updates a specific documentcategory by its primary key</summary>
        /// <param name="id">The primary key of the documentcategory</param>
        /// <param name="updatedEntity">The documentcategory data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, DocumentCategory updatedEntity);

        /// <summary>Updates a specific documentcategory by its primary key</summary>
        /// <param name="id">The primary key of the documentcategory</param>
        /// <param name="updatedEntity">The documentcategory data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<DocumentCategory> updatedEntity);

        /// <summary>Deletes a specific documentcategory by its primary key</summary>
        /// <param name="id">The primary key of the documentcategory</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The documentcategoryService responsible for managing documentcategory related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting documentcategory information.
    /// </remarks>
    public class DocumentCategoryService : IDocumentCategoryService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the DocumentCategory class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public DocumentCategoryService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific documentcategory by its primary key</summary>
        /// <param name="id">The primary key of the documentcategory</param>
        /// <returns>The documentcategory data</returns>
        public DocumentCategory GetById(Guid id)
        {
            var entityData = _dbContext.DocumentCategory.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of documentcategorys based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of documentcategorys</returns>/// <exception cref="Exception"></exception>
        public List<DocumentCategory> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetDocumentCategory(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new documentcategory</summary>
        /// <param name="model">The documentcategory data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(DocumentCategory model)
        {
            model.Id = CreateDocumentCategory(model);
            return model.Id;
        }

        /// <summary>Updates a specific documentcategory by its primary key</summary>
        /// <param name="id">The primary key of the documentcategory</param>
        /// <param name="updatedEntity">The documentcategory data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, DocumentCategory updatedEntity)
        {
            UpdateDocumentCategory(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific documentcategory by its primary key</summary>
        /// <param name="id">The primary key of the documentcategory</param>
        /// <param name="updatedEntity">The documentcategory data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<DocumentCategory> updatedEntity)
        {
            PatchDocumentCategory(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific documentcategory by its primary key</summary>
        /// <param name="id">The primary key of the documentcategory</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteDocumentCategory(id);
            return true;
        }
        #region
        private List<DocumentCategory> GetDocumentCategory(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.DocumentCategory.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<DocumentCategory>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(DocumentCategory), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<DocumentCategory, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateDocumentCategory(DocumentCategory model)
        {
            _dbContext.DocumentCategory.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateDocumentCategory(Guid id, DocumentCategory updatedEntity)
        {
            _dbContext.DocumentCategory.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteDocumentCategory(Guid id)
        {
            var entityData = _dbContext.DocumentCategory.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.DocumentCategory.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchDocumentCategory(Guid id, JsonPatchDocument<DocumentCategory> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.DocumentCategory.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.DocumentCategory.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}