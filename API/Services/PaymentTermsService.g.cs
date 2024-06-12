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
    /// The paymenttermsService responsible for managing paymentterms related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting paymentterms information.
    /// </remarks>
    public interface IPaymentTermsService
    {
        /// <summary>Retrieves a specific paymentterms by its primary key</summary>
        /// <param name="id">The primary key of the paymentterms</param>
        /// <returns>The paymentterms data</returns>
        PaymentTerms GetById(Guid id);

        /// <summary>Retrieves a list of paymenttermss based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of paymenttermss</returns>
        List<PaymentTerms> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new paymentterms</summary>
        /// <param name="model">The paymentterms data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(PaymentTerms model);

        /// <summary>Updates a specific paymentterms by its primary key</summary>
        /// <param name="id">The primary key of the paymentterms</param>
        /// <param name="updatedEntity">The paymentterms data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, PaymentTerms updatedEntity);

        /// <summary>Updates a specific paymentterms by its primary key</summary>
        /// <param name="id">The primary key of the paymentterms</param>
        /// <param name="updatedEntity">The paymentterms data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<PaymentTerms> updatedEntity);

        /// <summary>Deletes a specific paymentterms by its primary key</summary>
        /// <param name="id">The primary key of the paymentterms</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The paymenttermsService responsible for managing paymentterms related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting paymentterms information.
    /// </remarks>
    public class PaymentTermsService : IPaymentTermsService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the PaymentTerms class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public PaymentTermsService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific paymentterms by its primary key</summary>
        /// <param name="id">The primary key of the paymentterms</param>
        /// <returns>The paymentterms data</returns>
        public PaymentTerms GetById(Guid id)
        {
            var entityData = _dbContext.PaymentTerms.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of paymenttermss based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of paymenttermss</returns>/// <exception cref="Exception"></exception>
        public List<PaymentTerms> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetPaymentTerms(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new paymentterms</summary>
        /// <param name="model">The paymentterms data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(PaymentTerms model)
        {
            model.Id = CreatePaymentTerms(model);
            return model.Id;
        }

        /// <summary>Updates a specific paymentterms by its primary key</summary>
        /// <param name="id">The primary key of the paymentterms</param>
        /// <param name="updatedEntity">The paymentterms data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, PaymentTerms updatedEntity)
        {
            UpdatePaymentTerms(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific paymentterms by its primary key</summary>
        /// <param name="id">The primary key of the paymentterms</param>
        /// <param name="updatedEntity">The paymentterms data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<PaymentTerms> updatedEntity)
        {
            PatchPaymentTerms(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific paymentterms by its primary key</summary>
        /// <param name="id">The primary key of the paymentterms</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeletePaymentTerms(id);
            return true;
        }
        #region
        private List<PaymentTerms> GetPaymentTerms(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.PaymentTerms.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<PaymentTerms>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(PaymentTerms), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<PaymentTerms, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreatePaymentTerms(PaymentTerms model)
        {
            _dbContext.PaymentTerms.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdatePaymentTerms(Guid id, PaymentTerms updatedEntity)
        {
            _dbContext.PaymentTerms.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeletePaymentTerms(Guid id)
        {
            var entityData = _dbContext.PaymentTerms.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.PaymentTerms.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchPaymentTerms(Guid id, JsonPatchDocument<PaymentTerms> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.PaymentTerms.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.PaymentTerms.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}