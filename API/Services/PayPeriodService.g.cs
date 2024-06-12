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
    /// The payperiodService responsible for managing payperiod related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting payperiod information.
    /// </remarks>
    public interface IPayPeriodService
    {
        /// <summary>Retrieves a specific payperiod by its primary key</summary>
        /// <param name="id">The primary key of the payperiod</param>
        /// <returns>The payperiod data</returns>
        PayPeriod GetById(Guid id);

        /// <summary>Retrieves a list of payperiods based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of payperiods</returns>
        List<PayPeriod> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new payperiod</summary>
        /// <param name="model">The payperiod data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(PayPeriod model);

        /// <summary>Updates a specific payperiod by its primary key</summary>
        /// <param name="id">The primary key of the payperiod</param>
        /// <param name="updatedEntity">The payperiod data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, PayPeriod updatedEntity);

        /// <summary>Updates a specific payperiod by its primary key</summary>
        /// <param name="id">The primary key of the payperiod</param>
        /// <param name="updatedEntity">The payperiod data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<PayPeriod> updatedEntity);

        /// <summary>Deletes a specific payperiod by its primary key</summary>
        /// <param name="id">The primary key of the payperiod</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The payperiodService responsible for managing payperiod related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting payperiod information.
    /// </remarks>
    public class PayPeriodService : IPayPeriodService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the PayPeriod class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public PayPeriodService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific payperiod by its primary key</summary>
        /// <param name="id">The primary key of the payperiod</param>
        /// <returns>The payperiod data</returns>
        public PayPeriod GetById(Guid id)
        {
            var entityData = _dbContext.PayPeriod.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of payperiods based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of payperiods</returns>/// <exception cref="Exception"></exception>
        public List<PayPeriod> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetPayPeriod(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new payperiod</summary>
        /// <param name="model">The payperiod data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(PayPeriod model)
        {
            model.Id = CreatePayPeriod(model);
            return model.Id;
        }

        /// <summary>Updates a specific payperiod by its primary key</summary>
        /// <param name="id">The primary key of the payperiod</param>
        /// <param name="updatedEntity">The payperiod data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, PayPeriod updatedEntity)
        {
            UpdatePayPeriod(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific payperiod by its primary key</summary>
        /// <param name="id">The primary key of the payperiod</param>
        /// <param name="updatedEntity">The payperiod data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<PayPeriod> updatedEntity)
        {
            PatchPayPeriod(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific payperiod by its primary key</summary>
        /// <param name="id">The primary key of the payperiod</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeletePayPeriod(id);
            return true;
        }
        #region
        private List<PayPeriod> GetPayPeriod(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.PayPeriod.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<PayPeriod>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(PayPeriod), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<PayPeriod, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreatePayPeriod(PayPeriod model)
        {
            _dbContext.PayPeriod.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdatePayPeriod(Guid id, PayPeriod updatedEntity)
        {
            _dbContext.PayPeriod.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeletePayPeriod(Guid id)
        {
            var entityData = _dbContext.PayPeriod.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.PayPeriod.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchPayPeriod(Guid id, JsonPatchDocument<PayPeriod> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.PayPeriod.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.PayPeriod.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}