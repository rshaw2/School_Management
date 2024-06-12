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
    /// The financialaccountService responsible for managing financialaccount related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting financialaccount information.
    /// </remarks>
    public interface IFinancialAccountService
    {
        /// <summary>Retrieves a specific financialaccount by its primary key</summary>
        /// <param name="id">The primary key of the financialaccount</param>
        /// <returns>The financialaccount data</returns>
        FinancialAccount GetById(Guid id);

        /// <summary>Retrieves a list of financialaccounts based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of financialaccounts</returns>
        List<FinancialAccount> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new financialaccount</summary>
        /// <param name="model">The financialaccount data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(FinancialAccount model);

        /// <summary>Updates a specific financialaccount by its primary key</summary>
        /// <param name="id">The primary key of the financialaccount</param>
        /// <param name="updatedEntity">The financialaccount data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, FinancialAccount updatedEntity);

        /// <summary>Updates a specific financialaccount by its primary key</summary>
        /// <param name="id">The primary key of the financialaccount</param>
        /// <param name="updatedEntity">The financialaccount data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<FinancialAccount> updatedEntity);

        /// <summary>Deletes a specific financialaccount by its primary key</summary>
        /// <param name="id">The primary key of the financialaccount</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The financialaccountService responsible for managing financialaccount related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting financialaccount information.
    /// </remarks>
    public class FinancialAccountService : IFinancialAccountService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the FinancialAccount class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public FinancialAccountService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific financialaccount by its primary key</summary>
        /// <param name="id">The primary key of the financialaccount</param>
        /// <returns>The financialaccount data</returns>
        public FinancialAccount GetById(Guid id)
        {
            var entityData = _dbContext.FinancialAccount.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of financialaccounts based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of financialaccounts</returns>/// <exception cref="Exception"></exception>
        public List<FinancialAccount> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetFinancialAccount(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new financialaccount</summary>
        /// <param name="model">The financialaccount data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(FinancialAccount model)
        {
            model.Id = CreateFinancialAccount(model);
            return model.Id;
        }

        /// <summary>Updates a specific financialaccount by its primary key</summary>
        /// <param name="id">The primary key of the financialaccount</param>
        /// <param name="updatedEntity">The financialaccount data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, FinancialAccount updatedEntity)
        {
            UpdateFinancialAccount(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific financialaccount by its primary key</summary>
        /// <param name="id">The primary key of the financialaccount</param>
        /// <param name="updatedEntity">The financialaccount data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<FinancialAccount> updatedEntity)
        {
            PatchFinancialAccount(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific financialaccount by its primary key</summary>
        /// <param name="id">The primary key of the financialaccount</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteFinancialAccount(id);
            return true;
        }
        #region
        private List<FinancialAccount> GetFinancialAccount(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.FinancialAccount.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<FinancialAccount>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(FinancialAccount), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<FinancialAccount, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateFinancialAccount(FinancialAccount model)
        {
            _dbContext.FinancialAccount.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateFinancialAccount(Guid id, FinancialAccount updatedEntity)
        {
            _dbContext.FinancialAccount.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteFinancialAccount(Guid id)
        {
            var entityData = _dbContext.FinancialAccount.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.FinancialAccount.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchFinancialAccount(Guid id, JsonPatchDocument<FinancialAccount> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.FinancialAccount.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.FinancialAccount.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}