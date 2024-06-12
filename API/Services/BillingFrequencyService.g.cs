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
    /// The billingfrequencyService responsible for managing billingfrequency related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting billingfrequency information.
    /// </remarks>
    public interface IBillingFrequencyService
    {
        /// <summary>Retrieves a specific billingfrequency by its primary key</summary>
        /// <param name="id">The primary key of the billingfrequency</param>
        /// <returns>The billingfrequency data</returns>
        BillingFrequency GetById(Guid id);

        /// <summary>Retrieves a list of billingfrequencys based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of billingfrequencys</returns>
        List<BillingFrequency> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new billingfrequency</summary>
        /// <param name="model">The billingfrequency data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(BillingFrequency model);

        /// <summary>Updates a specific billingfrequency by its primary key</summary>
        /// <param name="id">The primary key of the billingfrequency</param>
        /// <param name="updatedEntity">The billingfrequency data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, BillingFrequency updatedEntity);

        /// <summary>Updates a specific billingfrequency by its primary key</summary>
        /// <param name="id">The primary key of the billingfrequency</param>
        /// <param name="updatedEntity">The billingfrequency data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<BillingFrequency> updatedEntity);

        /// <summary>Deletes a specific billingfrequency by its primary key</summary>
        /// <param name="id">The primary key of the billingfrequency</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The billingfrequencyService responsible for managing billingfrequency related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting billingfrequency information.
    /// </remarks>
    public class BillingFrequencyService : IBillingFrequencyService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the BillingFrequency class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public BillingFrequencyService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific billingfrequency by its primary key</summary>
        /// <param name="id">The primary key of the billingfrequency</param>
        /// <returns>The billingfrequency data</returns>
        public BillingFrequency GetById(Guid id)
        {
            var entityData = _dbContext.BillingFrequency.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of billingfrequencys based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of billingfrequencys</returns>/// <exception cref="Exception"></exception>
        public List<BillingFrequency> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetBillingFrequency(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new billingfrequency</summary>
        /// <param name="model">The billingfrequency data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(BillingFrequency model)
        {
            model.Id = CreateBillingFrequency(model);
            return model.Id;
        }

        /// <summary>Updates a specific billingfrequency by its primary key</summary>
        /// <param name="id">The primary key of the billingfrequency</param>
        /// <param name="updatedEntity">The billingfrequency data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, BillingFrequency updatedEntity)
        {
            UpdateBillingFrequency(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific billingfrequency by its primary key</summary>
        /// <param name="id">The primary key of the billingfrequency</param>
        /// <param name="updatedEntity">The billingfrequency data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<BillingFrequency> updatedEntity)
        {
            PatchBillingFrequency(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific billingfrequency by its primary key</summary>
        /// <param name="id">The primary key of the billingfrequency</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteBillingFrequency(id);
            return true;
        }
        #region
        private List<BillingFrequency> GetBillingFrequency(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.BillingFrequency.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<BillingFrequency>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(BillingFrequency), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<BillingFrequency, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateBillingFrequency(BillingFrequency model)
        {
            _dbContext.BillingFrequency.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateBillingFrequency(Guid id, BillingFrequency updatedEntity)
        {
            _dbContext.BillingFrequency.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteBillingFrequency(Guid id)
        {
            var entityData = _dbContext.BillingFrequency.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.BillingFrequency.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchBillingFrequency(Guid id, JsonPatchDocument<BillingFrequency> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.BillingFrequency.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.BillingFrequency.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}