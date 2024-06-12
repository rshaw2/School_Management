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
    /// The documentstatusService responsible for managing documentstatus related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting documentstatus information.
    /// </remarks>
    public interface IDocumentStatusService
    {
        /// <summary>Retrieves a specific documentstatus by its primary key</summary>
        /// <param name="id">The primary key of the documentstatus</param>
        /// <returns>The documentstatus data</returns>
        DocumentStatus GetById(Guid id);

        /// <summary>Retrieves a list of documentstatuss based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of documentstatuss</returns>
        List<DocumentStatus> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new documentstatus</summary>
        /// <param name="model">The documentstatus data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(DocumentStatus model);

        /// <summary>Updates a specific documentstatus by its primary key</summary>
        /// <param name="id">The primary key of the documentstatus</param>
        /// <param name="updatedEntity">The documentstatus data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, DocumentStatus updatedEntity);

        /// <summary>Updates a specific documentstatus by its primary key</summary>
        /// <param name="id">The primary key of the documentstatus</param>
        /// <param name="updatedEntity">The documentstatus data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<DocumentStatus> updatedEntity);

        /// <summary>Deletes a specific documentstatus by its primary key</summary>
        /// <param name="id">The primary key of the documentstatus</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The documentstatusService responsible for managing documentstatus related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting documentstatus information.
    /// </remarks>
    public class DocumentStatusService : IDocumentStatusService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the DocumentStatus class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public DocumentStatusService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific documentstatus by its primary key</summary>
        /// <param name="id">The primary key of the documentstatus</param>
        /// <returns>The documentstatus data</returns>
        public DocumentStatus GetById(Guid id)
        {
            var entityData = _dbContext.DocumentStatus.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of documentstatuss based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of documentstatuss</returns>/// <exception cref="Exception"></exception>
        public List<DocumentStatus> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetDocumentStatus(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new documentstatus</summary>
        /// <param name="model">The documentstatus data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(DocumentStatus model)
        {
            model.Id = CreateDocumentStatus(model);
            return model.Id;
        }

        /// <summary>Updates a specific documentstatus by its primary key</summary>
        /// <param name="id">The primary key of the documentstatus</param>
        /// <param name="updatedEntity">The documentstatus data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, DocumentStatus updatedEntity)
        {
            UpdateDocumentStatus(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific documentstatus by its primary key</summary>
        /// <param name="id">The primary key of the documentstatus</param>
        /// <param name="updatedEntity">The documentstatus data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<DocumentStatus> updatedEntity)
        {
            PatchDocumentStatus(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific documentstatus by its primary key</summary>
        /// <param name="id">The primary key of the documentstatus</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteDocumentStatus(id);
            return true;
        }
        #region
        private List<DocumentStatus> GetDocumentStatus(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.DocumentStatus.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<DocumentStatus>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(DocumentStatus), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<DocumentStatus, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateDocumentStatus(DocumentStatus model)
        {
            _dbContext.DocumentStatus.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateDocumentStatus(Guid id, DocumentStatus updatedEntity)
        {
            _dbContext.DocumentStatus.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteDocumentStatus(Guid id)
        {
            var entityData = _dbContext.DocumentStatus.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.DocumentStatus.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchDocumentStatus(Guid id, JsonPatchDocument<DocumentStatus> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.DocumentStatus.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.DocumentStatus.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}