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
    /// The audittrailService responsible for managing audittrail related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting audittrail information.
    /// </remarks>
    public interface IAuditTrailService
    {
        /// <summary>Retrieves a specific audittrail by its primary key</summary>
        /// <param name="id">The primary key of the audittrail</param>
        /// <returns>The audittrail data</returns>
        AuditTrail GetById(Guid id);

        /// <summary>Retrieves a list of audittrails based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of audittrails</returns>
        List<AuditTrail> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new audittrail</summary>
        /// <param name="model">The audittrail data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(AuditTrail model);

        /// <summary>Updates a specific audittrail by its primary key</summary>
        /// <param name="id">The primary key of the audittrail</param>
        /// <param name="updatedEntity">The audittrail data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, AuditTrail updatedEntity);

        /// <summary>Updates a specific audittrail by its primary key</summary>
        /// <param name="id">The primary key of the audittrail</param>
        /// <param name="updatedEntity">The audittrail data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<AuditTrail> updatedEntity);

        /// <summary>Deletes a specific audittrail by its primary key</summary>
        /// <param name="id">The primary key of the audittrail</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The audittrailService responsible for managing audittrail related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting audittrail information.
    /// </remarks>
    public class AuditTrailService : IAuditTrailService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the AuditTrail class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public AuditTrailService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific audittrail by its primary key</summary>
        /// <param name="id">The primary key of the audittrail</param>
        /// <returns>The audittrail data</returns>
        public AuditTrail GetById(Guid id)
        {
            var entityData = _dbContext.AuditTrail.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of audittrails based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of audittrails</returns>/// <exception cref="Exception"></exception>
        public List<AuditTrail> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetAuditTrail(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new audittrail</summary>
        /// <param name="model">The audittrail data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(AuditTrail model)
        {
            model.Id = CreateAuditTrail(model);
            return model.Id;
        }

        /// <summary>Updates a specific audittrail by its primary key</summary>
        /// <param name="id">The primary key of the audittrail</param>
        /// <param name="updatedEntity">The audittrail data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, AuditTrail updatedEntity)
        {
            UpdateAuditTrail(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific audittrail by its primary key</summary>
        /// <param name="id">The primary key of the audittrail</param>
        /// <param name="updatedEntity">The audittrail data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<AuditTrail> updatedEntity)
        {
            PatchAuditTrail(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific audittrail by its primary key</summary>
        /// <param name="id">The primary key of the audittrail</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteAuditTrail(id);
            return true;
        }
        #region
        private List<AuditTrail> GetAuditTrail(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.AuditTrail.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<AuditTrail>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(AuditTrail), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<AuditTrail, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateAuditTrail(AuditTrail model)
        {
            _dbContext.AuditTrail.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateAuditTrail(Guid id, AuditTrail updatedEntity)
        {
            _dbContext.AuditTrail.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteAuditTrail(Guid id)
        {
            var entityData = _dbContext.AuditTrail.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.AuditTrail.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchAuditTrail(Guid id, JsonPatchDocument<AuditTrail> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.AuditTrail.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.AuditTrail.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}