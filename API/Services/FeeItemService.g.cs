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
    /// The feeitemService responsible for managing feeitem related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting feeitem information.
    /// </remarks>
    public interface IFeeItemService
    {
        /// <summary>Retrieves a specific feeitem by its primary key</summary>
        /// <param name="id">The primary key of the feeitem</param>
        /// <returns>The feeitem data</returns>
        FeeItem GetById(Guid id);

        /// <summary>Retrieves a list of feeitems based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of feeitems</returns>
        List<FeeItem> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new feeitem</summary>
        /// <param name="model">The feeitem data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(FeeItem model);

        /// <summary>Updates a specific feeitem by its primary key</summary>
        /// <param name="id">The primary key of the feeitem</param>
        /// <param name="updatedEntity">The feeitem data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, FeeItem updatedEntity);

        /// <summary>Updates a specific feeitem by its primary key</summary>
        /// <param name="id">The primary key of the feeitem</param>
        /// <param name="updatedEntity">The feeitem data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<FeeItem> updatedEntity);

        /// <summary>Deletes a specific feeitem by its primary key</summary>
        /// <param name="id">The primary key of the feeitem</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The feeitemService responsible for managing feeitem related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting feeitem information.
    /// </remarks>
    public class FeeItemService : IFeeItemService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the FeeItem class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public FeeItemService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific feeitem by its primary key</summary>
        /// <param name="id">The primary key of the feeitem</param>
        /// <returns>The feeitem data</returns>
        public FeeItem GetById(Guid id)
        {
            var entityData = _dbContext.FeeItem.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of feeitems based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of feeitems</returns>/// <exception cref="Exception"></exception>
        public List<FeeItem> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetFeeItem(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new feeitem</summary>
        /// <param name="model">The feeitem data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(FeeItem model)
        {
            model.Id = CreateFeeItem(model);
            return model.Id;
        }

        /// <summary>Updates a specific feeitem by its primary key</summary>
        /// <param name="id">The primary key of the feeitem</param>
        /// <param name="updatedEntity">The feeitem data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, FeeItem updatedEntity)
        {
            UpdateFeeItem(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific feeitem by its primary key</summary>
        /// <param name="id">The primary key of the feeitem</param>
        /// <param name="updatedEntity">The feeitem data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<FeeItem> updatedEntity)
        {
            PatchFeeItem(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific feeitem by its primary key</summary>
        /// <param name="id">The primary key of the feeitem</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteFeeItem(id);
            return true;
        }
        #region
        private List<FeeItem> GetFeeItem(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.FeeItem.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<FeeItem>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(FeeItem), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<FeeItem, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateFeeItem(FeeItem model)
        {
            _dbContext.FeeItem.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateFeeItem(Guid id, FeeItem updatedEntity)
        {
            _dbContext.FeeItem.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteFeeItem(Guid id)
        {
            var entityData = _dbContext.FeeItem.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.FeeItem.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchFeeItem(Guid id, JsonPatchDocument<FeeItem> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.FeeItem.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.FeeItem.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}