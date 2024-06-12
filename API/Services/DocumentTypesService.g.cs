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
    /// The documenttypesService responsible for managing documenttypes related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting documenttypes information.
    /// </remarks>
    public interface IDocumentTypesService
    {
        /// <summary>Retrieves a specific documenttypes by its primary key</summary>
        /// <param name="id">The primary key of the documenttypes</param>
        /// <returns>The documenttypes data</returns>
        DocumentTypes GetById(Guid id);

        /// <summary>Retrieves a list of documenttypess based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of documenttypess</returns>
        List<DocumentTypes> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new documenttypes</summary>
        /// <param name="model">The documenttypes data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(DocumentTypes model);

        /// <summary>Updates a specific documenttypes by its primary key</summary>
        /// <param name="id">The primary key of the documenttypes</param>
        /// <param name="updatedEntity">The documenttypes data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, DocumentTypes updatedEntity);

        /// <summary>Updates a specific documenttypes by its primary key</summary>
        /// <param name="id">The primary key of the documenttypes</param>
        /// <param name="updatedEntity">The documenttypes data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<DocumentTypes> updatedEntity);

        /// <summary>Deletes a specific documenttypes by its primary key</summary>
        /// <param name="id">The primary key of the documenttypes</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The documenttypesService responsible for managing documenttypes related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting documenttypes information.
    /// </remarks>
    public class DocumentTypesService : IDocumentTypesService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the DocumentTypes class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public DocumentTypesService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific documenttypes by its primary key</summary>
        /// <param name="id">The primary key of the documenttypes</param>
        /// <returns>The documenttypes data</returns>
        public DocumentTypes GetById(Guid id)
        {
            var entityData = _dbContext.DocumentTypes.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of documenttypess based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of documenttypess</returns>/// <exception cref="Exception"></exception>
        public List<DocumentTypes> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetDocumentTypes(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new documenttypes</summary>
        /// <param name="model">The documenttypes data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(DocumentTypes model)
        {
            model.Id = CreateDocumentTypes(model);
            return model.Id;
        }

        /// <summary>Updates a specific documenttypes by its primary key</summary>
        /// <param name="id">The primary key of the documenttypes</param>
        /// <param name="updatedEntity">The documenttypes data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, DocumentTypes updatedEntity)
        {
            UpdateDocumentTypes(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific documenttypes by its primary key</summary>
        /// <param name="id">The primary key of the documenttypes</param>
        /// <param name="updatedEntity">The documenttypes data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<DocumentTypes> updatedEntity)
        {
            PatchDocumentTypes(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific documenttypes by its primary key</summary>
        /// <param name="id">The primary key of the documenttypes</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteDocumentTypes(id);
            return true;
        }
        #region
        private List<DocumentTypes> GetDocumentTypes(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.DocumentTypes.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<DocumentTypes>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(DocumentTypes), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<DocumentTypes, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateDocumentTypes(DocumentTypes model)
        {
            _dbContext.DocumentTypes.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateDocumentTypes(Guid id, DocumentTypes updatedEntity)
        {
            _dbContext.DocumentTypes.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteDocumentTypes(Guid id)
        {
            var entityData = _dbContext.DocumentTypes.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.DocumentTypes.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchDocumentTypes(Guid id, JsonPatchDocument<DocumentTypes> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.DocumentTypes.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.DocumentTypes.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}