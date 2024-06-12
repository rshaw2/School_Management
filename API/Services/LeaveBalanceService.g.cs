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
    /// The leavebalanceService responsible for managing leavebalance related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting leavebalance information.
    /// </remarks>
    public interface ILeaveBalanceService
    {
        /// <summary>Retrieves a specific leavebalance by its primary key</summary>
        /// <param name="id">The primary key of the leavebalance</param>
        /// <returns>The leavebalance data</returns>
        LeaveBalance GetById(Guid id);

        /// <summary>Retrieves a list of leavebalances based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of leavebalances</returns>
        List<LeaveBalance> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new leavebalance</summary>
        /// <param name="model">The leavebalance data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(LeaveBalance model);

        /// <summary>Updates a specific leavebalance by its primary key</summary>
        /// <param name="id">The primary key of the leavebalance</param>
        /// <param name="updatedEntity">The leavebalance data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, LeaveBalance updatedEntity);

        /// <summary>Updates a specific leavebalance by its primary key</summary>
        /// <param name="id">The primary key of the leavebalance</param>
        /// <param name="updatedEntity">The leavebalance data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<LeaveBalance> updatedEntity);

        /// <summary>Deletes a specific leavebalance by its primary key</summary>
        /// <param name="id">The primary key of the leavebalance</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The leavebalanceService responsible for managing leavebalance related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting leavebalance information.
    /// </remarks>
    public class LeaveBalanceService : ILeaveBalanceService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the LeaveBalance class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public LeaveBalanceService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific leavebalance by its primary key</summary>
        /// <param name="id">The primary key of the leavebalance</param>
        /// <returns>The leavebalance data</returns>
        public LeaveBalance GetById(Guid id)
        {
            var entityData = _dbContext.LeaveBalance.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of leavebalances based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of leavebalances</returns>/// <exception cref="Exception"></exception>
        public List<LeaveBalance> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetLeaveBalance(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new leavebalance</summary>
        /// <param name="model">The leavebalance data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(LeaveBalance model)
        {
            model.Id = CreateLeaveBalance(model);
            return model.Id;
        }

        /// <summary>Updates a specific leavebalance by its primary key</summary>
        /// <param name="id">The primary key of the leavebalance</param>
        /// <param name="updatedEntity">The leavebalance data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, LeaveBalance updatedEntity)
        {
            UpdateLeaveBalance(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific leavebalance by its primary key</summary>
        /// <param name="id">The primary key of the leavebalance</param>
        /// <param name="updatedEntity">The leavebalance data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<LeaveBalance> updatedEntity)
        {
            PatchLeaveBalance(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific leavebalance by its primary key</summary>
        /// <param name="id">The primary key of the leavebalance</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteLeaveBalance(id);
            return true;
        }
        #region
        private List<LeaveBalance> GetLeaveBalance(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.LeaveBalance.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<LeaveBalance>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(LeaveBalance), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<LeaveBalance, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateLeaveBalance(LeaveBalance model)
        {
            _dbContext.LeaveBalance.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateLeaveBalance(Guid id, LeaveBalance updatedEntity)
        {
            _dbContext.LeaveBalance.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteLeaveBalance(Guid id)
        {
            var entityData = _dbContext.LeaveBalance.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.LeaveBalance.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchLeaveBalance(Guid id, JsonPatchDocument<LeaveBalance> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.LeaveBalance.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.LeaveBalance.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}