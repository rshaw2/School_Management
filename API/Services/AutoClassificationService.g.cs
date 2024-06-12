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
    /// The autoclassificationService responsible for managing autoclassification related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting autoclassification information.
    /// </remarks>
    public interface IAutoClassificationService
    {
        /// <summary>Retrieves a specific autoclassification by its primary key</summary>
        /// <param name="id">The primary key of the autoclassification</param>
        /// <returns>The autoclassification data</returns>
        AutoClassification GetById(Guid id);

        /// <summary>Retrieves a list of autoclassifications based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of autoclassifications</returns>
        List<AutoClassification> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new autoclassification</summary>
        /// <param name="model">The autoclassification data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(AutoClassification model);

        /// <summary>Updates a specific autoclassification by its primary key</summary>
        /// <param name="id">The primary key of the autoclassification</param>
        /// <param name="updatedEntity">The autoclassification data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, AutoClassification updatedEntity);

        /// <summary>Updates a specific autoclassification by its primary key</summary>
        /// <param name="id">The primary key of the autoclassification</param>
        /// <param name="updatedEntity">The autoclassification data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<AutoClassification> updatedEntity);

        /// <summary>Deletes a specific autoclassification by its primary key</summary>
        /// <param name="id">The primary key of the autoclassification</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The autoclassificationService responsible for managing autoclassification related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting autoclassification information.
    /// </remarks>
    public class AutoClassificationService : IAutoClassificationService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the AutoClassification class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public AutoClassificationService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific autoclassification by its primary key</summary>
        /// <param name="id">The primary key of the autoclassification</param>
        /// <returns>The autoclassification data</returns>
        public AutoClassification GetById(Guid id)
        {
            var entityData = _dbContext.AutoClassification.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of autoclassifications based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of autoclassifications</returns>/// <exception cref="Exception"></exception>
        public List<AutoClassification> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetAutoClassification(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new autoclassification</summary>
        /// <param name="model">The autoclassification data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(AutoClassification model)
        {
            model.Id = CreateAutoClassification(model);
            return model.Id;
        }

        /// <summary>Updates a specific autoclassification by its primary key</summary>
        /// <param name="id">The primary key of the autoclassification</param>
        /// <param name="updatedEntity">The autoclassification data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, AutoClassification updatedEntity)
        {
            UpdateAutoClassification(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific autoclassification by its primary key</summary>
        /// <param name="id">The primary key of the autoclassification</param>
        /// <param name="updatedEntity">The autoclassification data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<AutoClassification> updatedEntity)
        {
            PatchAutoClassification(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific autoclassification by its primary key</summary>
        /// <param name="id">The primary key of the autoclassification</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteAutoClassification(id);
            return true;
        }
        #region
        private List<AutoClassification> GetAutoClassification(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.AutoClassification.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<AutoClassification>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(AutoClassification), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<AutoClassification, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateAutoClassification(AutoClassification model)
        {
            _dbContext.AutoClassification.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateAutoClassification(Guid id, AutoClassification updatedEntity)
        {
            _dbContext.AutoClassification.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteAutoClassification(Guid id)
        {
            var entityData = _dbContext.AutoClassification.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.AutoClassification.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchAutoClassification(Guid id, JsonPatchDocument<AutoClassification> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.AutoClassification.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.AutoClassification.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}