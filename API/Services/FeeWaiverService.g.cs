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
    /// The feewaiverService responsible for managing feewaiver related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting feewaiver information.
    /// </remarks>
    public interface IFeeWaiverService
    {
        /// <summary>Retrieves a specific feewaiver by its primary key</summary>
        /// <param name="id">The primary key of the feewaiver</param>
        /// <returns>The feewaiver data</returns>
        FeeWaiver GetById(Guid id);

        /// <summary>Retrieves a list of feewaivers based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of feewaivers</returns>
        List<FeeWaiver> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new feewaiver</summary>
        /// <param name="model">The feewaiver data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(FeeWaiver model);

        /// <summary>Updates a specific feewaiver by its primary key</summary>
        /// <param name="id">The primary key of the feewaiver</param>
        /// <param name="updatedEntity">The feewaiver data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, FeeWaiver updatedEntity);

        /// <summary>Updates a specific feewaiver by its primary key</summary>
        /// <param name="id">The primary key of the feewaiver</param>
        /// <param name="updatedEntity">The feewaiver data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<FeeWaiver> updatedEntity);

        /// <summary>Deletes a specific feewaiver by its primary key</summary>
        /// <param name="id">The primary key of the feewaiver</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The feewaiverService responsible for managing feewaiver related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting feewaiver information.
    /// </remarks>
    public class FeeWaiverService : IFeeWaiverService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the FeeWaiver class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public FeeWaiverService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific feewaiver by its primary key</summary>
        /// <param name="id">The primary key of the feewaiver</param>
        /// <returns>The feewaiver data</returns>
        public FeeWaiver GetById(Guid id)
        {
            var entityData = _dbContext.FeeWaiver.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of feewaivers based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of feewaivers</returns>/// <exception cref="Exception"></exception>
        public List<FeeWaiver> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetFeeWaiver(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new feewaiver</summary>
        /// <param name="model">The feewaiver data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(FeeWaiver model)
        {
            model.Id = CreateFeeWaiver(model);
            return model.Id;
        }

        /// <summary>Updates a specific feewaiver by its primary key</summary>
        /// <param name="id">The primary key of the feewaiver</param>
        /// <param name="updatedEntity">The feewaiver data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, FeeWaiver updatedEntity)
        {
            UpdateFeeWaiver(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific feewaiver by its primary key</summary>
        /// <param name="id">The primary key of the feewaiver</param>
        /// <param name="updatedEntity">The feewaiver data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<FeeWaiver> updatedEntity)
        {
            PatchFeeWaiver(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific feewaiver by its primary key</summary>
        /// <param name="id">The primary key of the feewaiver</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteFeeWaiver(id);
            return true;
        }
        #region
        private List<FeeWaiver> GetFeeWaiver(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.FeeWaiver.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<FeeWaiver>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(FeeWaiver), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<FeeWaiver, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateFeeWaiver(FeeWaiver model)
        {
            _dbContext.FeeWaiver.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateFeeWaiver(Guid id, FeeWaiver updatedEntity)
        {
            _dbContext.FeeWaiver.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteFeeWaiver(Guid id)
        {
            var entityData = _dbContext.FeeWaiver.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.FeeWaiver.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchFeeWaiver(Guid id, JsonPatchDocument<FeeWaiver> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.FeeWaiver.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.FeeWaiver.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}