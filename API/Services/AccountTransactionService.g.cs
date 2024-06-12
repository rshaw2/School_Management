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
    /// The accounttransactionService responsible for managing accounttransaction related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting accounttransaction information.
    /// </remarks>
    public interface IAccountTransactionService
    {
        /// <summary>Retrieves a specific accounttransaction by its primary key</summary>
        /// <param name="id">The primary key of the accounttransaction</param>
        /// <returns>The accounttransaction data</returns>
        AccountTransaction GetById(Guid id);

        /// <summary>Retrieves a list of accounttransactions based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of accounttransactions</returns>
        List<AccountTransaction> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new accounttransaction</summary>
        /// <param name="model">The accounttransaction data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(AccountTransaction model);

        /// <summary>Updates a specific accounttransaction by its primary key</summary>
        /// <param name="id">The primary key of the accounttransaction</param>
        /// <param name="updatedEntity">The accounttransaction data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, AccountTransaction updatedEntity);

        /// <summary>Updates a specific accounttransaction by its primary key</summary>
        /// <param name="id">The primary key of the accounttransaction</param>
        /// <param name="updatedEntity">The accounttransaction data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<AccountTransaction> updatedEntity);

        /// <summary>Deletes a specific accounttransaction by its primary key</summary>
        /// <param name="id">The primary key of the accounttransaction</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The accounttransactionService responsible for managing accounttransaction related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting accounttransaction information.
    /// </remarks>
    public class AccountTransactionService : IAccountTransactionService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the AccountTransaction class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public AccountTransactionService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific accounttransaction by its primary key</summary>
        /// <param name="id">The primary key of the accounttransaction</param>
        /// <returns>The accounttransaction data</returns>
        public AccountTransaction GetById(Guid id)
        {
            var entityData = _dbContext.AccountTransaction.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of accounttransactions based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of accounttransactions</returns>/// <exception cref="Exception"></exception>
        public List<AccountTransaction> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetAccountTransaction(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new accounttransaction</summary>
        /// <param name="model">The accounttransaction data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(AccountTransaction model)
        {
            model.Id = CreateAccountTransaction(model);
            return model.Id;
        }

        /// <summary>Updates a specific accounttransaction by its primary key</summary>
        /// <param name="id">The primary key of the accounttransaction</param>
        /// <param name="updatedEntity">The accounttransaction data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, AccountTransaction updatedEntity)
        {
            UpdateAccountTransaction(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific accounttransaction by its primary key</summary>
        /// <param name="id">The primary key of the accounttransaction</param>
        /// <param name="updatedEntity">The accounttransaction data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<AccountTransaction> updatedEntity)
        {
            PatchAccountTransaction(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific accounttransaction by its primary key</summary>
        /// <param name="id">The primary key of the accounttransaction</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteAccountTransaction(id);
            return true;
        }
        #region
        private List<AccountTransaction> GetAccountTransaction(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.AccountTransaction.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<AccountTransaction>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(AccountTransaction), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<AccountTransaction, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateAccountTransaction(AccountTransaction model)
        {
            _dbContext.AccountTransaction.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateAccountTransaction(Guid id, AccountTransaction updatedEntity)
        {
            _dbContext.AccountTransaction.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteAccountTransaction(Guid id)
        {
            var entityData = _dbContext.AccountTransaction.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.AccountTransaction.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchAccountTransaction(Guid id, JsonPatchDocument<AccountTransaction> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.AccountTransaction.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.AccountTransaction.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}