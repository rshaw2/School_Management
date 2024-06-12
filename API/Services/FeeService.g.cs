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
    /// The feeService responsible for managing fee related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting fee information.
    /// </remarks>
    public interface IFeeService
    {
        /// <summary>Retrieves a specific fee by its primary key</summary>
        /// <param name="id">The primary key of the fee</param>
        /// <returns>The fee data</returns>
        Fee GetById(Guid id);

        /// <summary>Retrieves a list of fees based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of fees</returns>
        List<Fee> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new fee</summary>
        /// <param name="model">The fee data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Fee model);

        /// <summary>Updates a specific fee by its primary key</summary>
        /// <param name="id">The primary key of the fee</param>
        /// <param name="updatedEntity">The fee data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Fee updatedEntity);

        /// <summary>Updates a specific fee by its primary key</summary>
        /// <param name="id">The primary key of the fee</param>
        /// <param name="updatedEntity">The fee data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Fee> updatedEntity);

        /// <summary>Deletes a specific fee by its primary key</summary>
        /// <param name="id">The primary key of the fee</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The feeService responsible for managing fee related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting fee information.
    /// </remarks>
    public class FeeService : IFeeService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Fee class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public FeeService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific fee by its primary key</summary>
        /// <param name="id">The primary key of the fee</param>
        /// <returns>The fee data</returns>
        public Fee GetById(Guid id)
        {
            var entityData = _dbContext.Fee.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of fees based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of fees</returns>/// <exception cref="Exception"></exception>
        public List<Fee> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetFee(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new fee</summary>
        /// <param name="model">The fee data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Fee model)
        {
            model.Id = CreateFee(model);
            return model.Id;
        }

        /// <summary>Updates a specific fee by its primary key</summary>
        /// <param name="id">The primary key of the fee</param>
        /// <param name="updatedEntity">The fee data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Fee updatedEntity)
        {
            UpdateFee(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific fee by its primary key</summary>
        /// <param name="id">The primary key of the fee</param>
        /// <param name="updatedEntity">The fee data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Fee> updatedEntity)
        {
            PatchFee(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific fee by its primary key</summary>
        /// <param name="id">The primary key of the fee</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteFee(id);
            return true;
        }
        #region
        private List<Fee> GetFee(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Fee.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Fee>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Fee), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Fee, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateFee(Fee model)
        {
            _dbContext.Fee.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateFee(Guid id, Fee updatedEntity)
        {
            _dbContext.Fee.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteFee(Guid id)
        {
            var entityData = _dbContext.Fee.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Fee.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchFee(Guid id, JsonPatchDocument<Fee> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Fee.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Fee.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}