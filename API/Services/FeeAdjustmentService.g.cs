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
    /// The feeadjustmentService responsible for managing feeadjustment related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting feeadjustment information.
    /// </remarks>
    public interface IFeeAdjustmentService
    {
        /// <summary>Retrieves a specific feeadjustment by its primary key</summary>
        /// <param name="id">The primary key of the feeadjustment</param>
        /// <returns>The feeadjustment data</returns>
        FeeAdjustment GetById(Guid id);

        /// <summary>Retrieves a list of feeadjustments based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of feeadjustments</returns>
        List<FeeAdjustment> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new feeadjustment</summary>
        /// <param name="model">The feeadjustment data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(FeeAdjustment model);

        /// <summary>Updates a specific feeadjustment by its primary key</summary>
        /// <param name="id">The primary key of the feeadjustment</param>
        /// <param name="updatedEntity">The feeadjustment data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, FeeAdjustment updatedEntity);

        /// <summary>Updates a specific feeadjustment by its primary key</summary>
        /// <param name="id">The primary key of the feeadjustment</param>
        /// <param name="updatedEntity">The feeadjustment data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<FeeAdjustment> updatedEntity);

        /// <summary>Deletes a specific feeadjustment by its primary key</summary>
        /// <param name="id">The primary key of the feeadjustment</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The feeadjustmentService responsible for managing feeadjustment related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting feeadjustment information.
    /// </remarks>
    public class FeeAdjustmentService : IFeeAdjustmentService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the FeeAdjustment class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public FeeAdjustmentService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific feeadjustment by its primary key</summary>
        /// <param name="id">The primary key of the feeadjustment</param>
        /// <returns>The feeadjustment data</returns>
        public FeeAdjustment GetById(Guid id)
        {
            var entityData = _dbContext.FeeAdjustment.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of feeadjustments based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of feeadjustments</returns>/// <exception cref="Exception"></exception>
        public List<FeeAdjustment> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetFeeAdjustment(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new feeadjustment</summary>
        /// <param name="model">The feeadjustment data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(FeeAdjustment model)
        {
            model.Id = CreateFeeAdjustment(model);
            return model.Id;
        }

        /// <summary>Updates a specific feeadjustment by its primary key</summary>
        /// <param name="id">The primary key of the feeadjustment</param>
        /// <param name="updatedEntity">The feeadjustment data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, FeeAdjustment updatedEntity)
        {
            UpdateFeeAdjustment(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific feeadjustment by its primary key</summary>
        /// <param name="id">The primary key of the feeadjustment</param>
        /// <param name="updatedEntity">The feeadjustment data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<FeeAdjustment> updatedEntity)
        {
            PatchFeeAdjustment(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific feeadjustment by its primary key</summary>
        /// <param name="id">The primary key of the feeadjustment</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteFeeAdjustment(id);
            return true;
        }
        #region
        private List<FeeAdjustment> GetFeeAdjustment(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.FeeAdjustment.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<FeeAdjustment>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(FeeAdjustment), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<FeeAdjustment, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateFeeAdjustment(FeeAdjustment model)
        {
            _dbContext.FeeAdjustment.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateFeeAdjustment(Guid id, FeeAdjustment updatedEntity)
        {
            _dbContext.FeeAdjustment.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteFeeAdjustment(Guid id)
        {
            var entityData = _dbContext.FeeAdjustment.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.FeeAdjustment.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchFeeAdjustment(Guid id, JsonPatchDocument<FeeAdjustment> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.FeeAdjustment.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.FeeAdjustment.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}