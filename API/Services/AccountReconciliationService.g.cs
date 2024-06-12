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
    /// The accountreconciliationService responsible for managing accountreconciliation related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting accountreconciliation information.
    /// </remarks>
    public interface IAccountReconciliationService
    {
        /// <summary>Retrieves a specific accountreconciliation by its primary key</summary>
        /// <param name="id">The primary key of the accountreconciliation</param>
        /// <returns>The accountreconciliation data</returns>
        AccountReconciliation GetById(Guid id);

        /// <summary>Retrieves a list of accountreconciliations based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of accountreconciliations</returns>
        List<AccountReconciliation> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new accountreconciliation</summary>
        /// <param name="model">The accountreconciliation data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(AccountReconciliation model);

        /// <summary>Updates a specific accountreconciliation by its primary key</summary>
        /// <param name="id">The primary key of the accountreconciliation</param>
        /// <param name="updatedEntity">The accountreconciliation data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, AccountReconciliation updatedEntity);

        /// <summary>Updates a specific accountreconciliation by its primary key</summary>
        /// <param name="id">The primary key of the accountreconciliation</param>
        /// <param name="updatedEntity">The accountreconciliation data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<AccountReconciliation> updatedEntity);

        /// <summary>Deletes a specific accountreconciliation by its primary key</summary>
        /// <param name="id">The primary key of the accountreconciliation</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The accountreconciliationService responsible for managing accountreconciliation related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting accountreconciliation information.
    /// </remarks>
    public class AccountReconciliationService : IAccountReconciliationService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the AccountReconciliation class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public AccountReconciliationService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific accountreconciliation by its primary key</summary>
        /// <param name="id">The primary key of the accountreconciliation</param>
        /// <returns>The accountreconciliation data</returns>
        public AccountReconciliation GetById(Guid id)
        {
            var entityData = _dbContext.AccountReconciliation.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of accountreconciliations based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of accountreconciliations</returns>/// <exception cref="Exception"></exception>
        public List<AccountReconciliation> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetAccountReconciliation(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new accountreconciliation</summary>
        /// <param name="model">The accountreconciliation data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(AccountReconciliation model)
        {
            model.Id = CreateAccountReconciliation(model);
            return model.Id;
        }

        /// <summary>Updates a specific accountreconciliation by its primary key</summary>
        /// <param name="id">The primary key of the accountreconciliation</param>
        /// <param name="updatedEntity">The accountreconciliation data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, AccountReconciliation updatedEntity)
        {
            UpdateAccountReconciliation(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific accountreconciliation by its primary key</summary>
        /// <param name="id">The primary key of the accountreconciliation</param>
        /// <param name="updatedEntity">The accountreconciliation data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<AccountReconciliation> updatedEntity)
        {
            PatchAccountReconciliation(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific accountreconciliation by its primary key</summary>
        /// <param name="id">The primary key of the accountreconciliation</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteAccountReconciliation(id);
            return true;
        }
        #region
        private List<AccountReconciliation> GetAccountReconciliation(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.AccountReconciliation.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<AccountReconciliation>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(AccountReconciliation), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<AccountReconciliation, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateAccountReconciliation(AccountReconciliation model)
        {
            _dbContext.AccountReconciliation.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateAccountReconciliation(Guid id, AccountReconciliation updatedEntity)
        {
            _dbContext.AccountReconciliation.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteAccountReconciliation(Guid id)
        {
            var entityData = _dbContext.AccountReconciliation.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.AccountReconciliation.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchAccountReconciliation(Guid id, JsonPatchDocument<AccountReconciliation> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.AccountReconciliation.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.AccountReconciliation.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}