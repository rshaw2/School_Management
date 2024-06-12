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
    /// The conflictresolutionService responsible for managing conflictresolution related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting conflictresolution information.
    /// </remarks>
    public interface IConflictResolutionService
    {
        /// <summary>Retrieves a specific conflictresolution by its primary key</summary>
        /// <param name="id">The primary key of the conflictresolution</param>
        /// <returns>The conflictresolution data</returns>
        ConflictResolution GetById(Guid id);

        /// <summary>Retrieves a list of conflictresolutions based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of conflictresolutions</returns>
        List<ConflictResolution> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new conflictresolution</summary>
        /// <param name="model">The conflictresolution data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(ConflictResolution model);

        /// <summary>Updates a specific conflictresolution by its primary key</summary>
        /// <param name="id">The primary key of the conflictresolution</param>
        /// <param name="updatedEntity">The conflictresolution data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, ConflictResolution updatedEntity);

        /// <summary>Updates a specific conflictresolution by its primary key</summary>
        /// <param name="id">The primary key of the conflictresolution</param>
        /// <param name="updatedEntity">The conflictresolution data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<ConflictResolution> updatedEntity);

        /// <summary>Deletes a specific conflictresolution by its primary key</summary>
        /// <param name="id">The primary key of the conflictresolution</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The conflictresolutionService responsible for managing conflictresolution related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting conflictresolution information.
    /// </remarks>
    public class ConflictResolutionService : IConflictResolutionService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the ConflictResolution class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public ConflictResolutionService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific conflictresolution by its primary key</summary>
        /// <param name="id">The primary key of the conflictresolution</param>
        /// <returns>The conflictresolution data</returns>
        public ConflictResolution GetById(Guid id)
        {
            var entityData = _dbContext.ConflictResolution.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of conflictresolutions based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of conflictresolutions</returns>/// <exception cref="Exception"></exception>
        public List<ConflictResolution> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetConflictResolution(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new conflictresolution</summary>
        /// <param name="model">The conflictresolution data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(ConflictResolution model)
        {
            model.Id = CreateConflictResolution(model);
            return model.Id;
        }

        /// <summary>Updates a specific conflictresolution by its primary key</summary>
        /// <param name="id">The primary key of the conflictresolution</param>
        /// <param name="updatedEntity">The conflictresolution data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, ConflictResolution updatedEntity)
        {
            UpdateConflictResolution(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific conflictresolution by its primary key</summary>
        /// <param name="id">The primary key of the conflictresolution</param>
        /// <param name="updatedEntity">The conflictresolution data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<ConflictResolution> updatedEntity)
        {
            PatchConflictResolution(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific conflictresolution by its primary key</summary>
        /// <param name="id">The primary key of the conflictresolution</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteConflictResolution(id);
            return true;
        }
        #region
        private List<ConflictResolution> GetConflictResolution(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.ConflictResolution.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<ConflictResolution>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(ConflictResolution), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<ConflictResolution, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateConflictResolution(ConflictResolution model)
        {
            _dbContext.ConflictResolution.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateConflictResolution(Guid id, ConflictResolution updatedEntity)
        {
            _dbContext.ConflictResolution.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteConflictResolution(Guid id)
        {
            var entityData = _dbContext.ConflictResolution.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.ConflictResolution.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchConflictResolution(Guid id, JsonPatchDocument<ConflictResolution> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.ConflictResolution.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.ConflictResolution.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}