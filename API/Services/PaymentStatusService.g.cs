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
    /// The paymentstatusService responsible for managing paymentstatus related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting paymentstatus information.
    /// </remarks>
    public interface IPaymentStatusService
    {
        /// <summary>Retrieves a specific paymentstatus by its primary key</summary>
        /// <param name="id">The primary key of the paymentstatus</param>
        /// <returns>The paymentstatus data</returns>
        PaymentStatus GetById(Guid id);

        /// <summary>Retrieves a list of paymentstatuss based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of paymentstatuss</returns>
        List<PaymentStatus> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new paymentstatus</summary>
        /// <param name="model">The paymentstatus data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(PaymentStatus model);

        /// <summary>Updates a specific paymentstatus by its primary key</summary>
        /// <param name="id">The primary key of the paymentstatus</param>
        /// <param name="updatedEntity">The paymentstatus data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, PaymentStatus updatedEntity);

        /// <summary>Updates a specific paymentstatus by its primary key</summary>
        /// <param name="id">The primary key of the paymentstatus</param>
        /// <param name="updatedEntity">The paymentstatus data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<PaymentStatus> updatedEntity);

        /// <summary>Deletes a specific paymentstatus by its primary key</summary>
        /// <param name="id">The primary key of the paymentstatus</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The paymentstatusService responsible for managing paymentstatus related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting paymentstatus information.
    /// </remarks>
    public class PaymentStatusService : IPaymentStatusService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the PaymentStatus class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public PaymentStatusService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific paymentstatus by its primary key</summary>
        /// <param name="id">The primary key of the paymentstatus</param>
        /// <returns>The paymentstatus data</returns>
        public PaymentStatus GetById(Guid id)
        {
            var entityData = _dbContext.PaymentStatus.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of paymentstatuss based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of paymentstatuss</returns>/// <exception cref="Exception"></exception>
        public List<PaymentStatus> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetPaymentStatus(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new paymentstatus</summary>
        /// <param name="model">The paymentstatus data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(PaymentStatus model)
        {
            model.Id = CreatePaymentStatus(model);
            return model.Id;
        }

        /// <summary>Updates a specific paymentstatus by its primary key</summary>
        /// <param name="id">The primary key of the paymentstatus</param>
        /// <param name="updatedEntity">The paymentstatus data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, PaymentStatus updatedEntity)
        {
            UpdatePaymentStatus(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific paymentstatus by its primary key</summary>
        /// <param name="id">The primary key of the paymentstatus</param>
        /// <param name="updatedEntity">The paymentstatus data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<PaymentStatus> updatedEntity)
        {
            PatchPaymentStatus(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific paymentstatus by its primary key</summary>
        /// <param name="id">The primary key of the paymentstatus</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeletePaymentStatus(id);
            return true;
        }
        #region
        private List<PaymentStatus> GetPaymentStatus(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.PaymentStatus.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<PaymentStatus>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(PaymentStatus), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<PaymentStatus, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreatePaymentStatus(PaymentStatus model)
        {
            _dbContext.PaymentStatus.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdatePaymentStatus(Guid id, PaymentStatus updatedEntity)
        {
            _dbContext.PaymentStatus.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeletePaymentStatus(Guid id)
        {
            var entityData = _dbContext.PaymentStatus.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.PaymentStatus.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchPaymentStatus(Guid id, JsonPatchDocument<PaymentStatus> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.PaymentStatus.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.PaymentStatus.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}