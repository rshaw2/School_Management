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
    /// The leaveService responsible for managing leave related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting leave information.
    /// </remarks>
    public interface ILeaveService
    {
        /// <summary>Retrieves a specific leave by its primary key</summary>
        /// <param name="id">The primary key of the leave</param>
        /// <returns>The leave data</returns>
        Leave GetById(Guid id);

        /// <summary>Retrieves a list of leaves based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of leaves</returns>
        List<Leave> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new leave</summary>
        /// <param name="model">The leave data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Leave model);

        /// <summary>Updates a specific leave by its primary key</summary>
        /// <param name="id">The primary key of the leave</param>
        /// <param name="updatedEntity">The leave data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Leave updatedEntity);

        /// <summary>Updates a specific leave by its primary key</summary>
        /// <param name="id">The primary key of the leave</param>
        /// <param name="updatedEntity">The leave data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Leave> updatedEntity);

        /// <summary>Deletes a specific leave by its primary key</summary>
        /// <param name="id">The primary key of the leave</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The leaveService responsible for managing leave related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting leave information.
    /// </remarks>
    public class LeaveService : ILeaveService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Leave class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public LeaveService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific leave by its primary key</summary>
        /// <param name="id">The primary key of the leave</param>
        /// <returns>The leave data</returns>
        public Leave GetById(Guid id)
        {
            var entityData = _dbContext.Leave.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of leaves based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of leaves</returns>/// <exception cref="Exception"></exception>
        public List<Leave> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetLeave(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new leave</summary>
        /// <param name="model">The leave data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Leave model)
        {
            model.Id = CreateLeave(model);
            return model.Id;
        }

        /// <summary>Updates a specific leave by its primary key</summary>
        /// <param name="id">The primary key of the leave</param>
        /// <param name="updatedEntity">The leave data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Leave updatedEntity)
        {
            UpdateLeave(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific leave by its primary key</summary>
        /// <param name="id">The primary key of the leave</param>
        /// <param name="updatedEntity">The leave data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Leave> updatedEntity)
        {
            PatchLeave(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific leave by its primary key</summary>
        /// <param name="id">The primary key of the leave</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteLeave(id);
            return true;
        }
        #region
        private List<Leave> GetLeave(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Leave.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Leave>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Leave), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Leave, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateLeave(Leave model)
        {
            _dbContext.Leave.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateLeave(Guid id, Leave updatedEntity)
        {
            _dbContext.Leave.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteLeave(Guid id)
        {
            var entityData = _dbContext.Leave.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Leave.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchLeave(Guid id, JsonPatchDocument<Leave> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Leave.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Leave.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}