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
    /// The billingService responsible for managing billing related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting billing information.
    /// </remarks>
    public interface IBillingService
    {
        /// <summary>Retrieves a specific billing by its primary key</summary>
        /// <param name="id">The primary key of the billing</param>
        /// <returns>The billing data</returns>
        Billing GetById(Guid id);

        /// <summary>Retrieves a list of billings based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of billings</returns>
        List<Billing> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new billing</summary>
        /// <param name="model">The billing data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Billing model);

        /// <summary>Updates a specific billing by its primary key</summary>
        /// <param name="id">The primary key of the billing</param>
        /// <param name="updatedEntity">The billing data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Billing updatedEntity);

        /// <summary>Updates a specific billing by its primary key</summary>
        /// <param name="id">The primary key of the billing</param>
        /// <param name="updatedEntity">The billing data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Billing> updatedEntity);

        /// <summary>Deletes a specific billing by its primary key</summary>
        /// <param name="id">The primary key of the billing</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The billingService responsible for managing billing related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting billing information.
    /// </remarks>
    public class BillingService : IBillingService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Billing class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public BillingService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific billing by its primary key</summary>
        /// <param name="id">The primary key of the billing</param>
        /// <returns>The billing data</returns>
        public Billing GetById(Guid id)
        {
            var entityData = _dbContext.Billing.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of billings based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of billings</returns>/// <exception cref="Exception"></exception>
        public List<Billing> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetBilling(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new billing</summary>
        /// <param name="model">The billing data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Billing model)
        {
            model.Id = CreateBilling(model);
            return model.Id;
        }

        /// <summary>Updates a specific billing by its primary key</summary>
        /// <param name="id">The primary key of the billing</param>
        /// <param name="updatedEntity">The billing data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Billing updatedEntity)
        {
            UpdateBilling(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific billing by its primary key</summary>
        /// <param name="id">The primary key of the billing</param>
        /// <param name="updatedEntity">The billing data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Billing> updatedEntity)
        {
            PatchBilling(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific billing by its primary key</summary>
        /// <param name="id">The primary key of the billing</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteBilling(id);
            return true;
        }
        #region
        private List<Billing> GetBilling(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Billing.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Billing>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Billing), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Billing, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateBilling(Billing model)
        {
            _dbContext.Billing.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateBilling(Guid id, Billing updatedEntity)
        {
            _dbContext.Billing.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteBilling(Guid id)
        {
            var entityData = _dbContext.Billing.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Billing.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchBilling(Guid id, JsonPatchDocument<Billing> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Billing.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Billing.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}