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
    /// The overtimeService responsible for managing overtime related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting overtime information.
    /// </remarks>
    public interface IOvertimeService
    {
        /// <summary>Retrieves a specific overtime by its primary key</summary>
        /// <param name="id">The primary key of the overtime</param>
        /// <returns>The overtime data</returns>
        Overtime GetById(Guid id);

        /// <summary>Retrieves a list of overtimes based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of overtimes</returns>
        List<Overtime> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new overtime</summary>
        /// <param name="model">The overtime data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Overtime model);

        /// <summary>Updates a specific overtime by its primary key</summary>
        /// <param name="id">The primary key of the overtime</param>
        /// <param name="updatedEntity">The overtime data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Overtime updatedEntity);

        /// <summary>Updates a specific overtime by its primary key</summary>
        /// <param name="id">The primary key of the overtime</param>
        /// <param name="updatedEntity">The overtime data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Overtime> updatedEntity);

        /// <summary>Deletes a specific overtime by its primary key</summary>
        /// <param name="id">The primary key of the overtime</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The overtimeService responsible for managing overtime related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting overtime information.
    /// </remarks>
    public class OvertimeService : IOvertimeService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Overtime class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public OvertimeService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific overtime by its primary key</summary>
        /// <param name="id">The primary key of the overtime</param>
        /// <returns>The overtime data</returns>
        public Overtime GetById(Guid id)
        {
            var entityData = _dbContext.Overtime.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of overtimes based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of overtimes</returns>/// <exception cref="Exception"></exception>
        public List<Overtime> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetOvertime(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new overtime</summary>
        /// <param name="model">The overtime data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Overtime model)
        {
            model.Id = CreateOvertime(model);
            return model.Id;
        }

        /// <summary>Updates a specific overtime by its primary key</summary>
        /// <param name="id">The primary key of the overtime</param>
        /// <param name="updatedEntity">The overtime data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Overtime updatedEntity)
        {
            UpdateOvertime(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific overtime by its primary key</summary>
        /// <param name="id">The primary key of the overtime</param>
        /// <param name="updatedEntity">The overtime data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Overtime> updatedEntity)
        {
            PatchOvertime(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific overtime by its primary key</summary>
        /// <param name="id">The primary key of the overtime</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteOvertime(id);
            return true;
        }
        #region
        private List<Overtime> GetOvertime(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Overtime.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Overtime>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Overtime), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Overtime, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateOvertime(Overtime model)
        {
            _dbContext.Overtime.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateOvertime(Guid id, Overtime updatedEntity)
        {
            _dbContext.Overtime.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteOvertime(Guid id)
        {
            var entityData = _dbContext.Overtime.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Overtime.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchOvertime(Guid id, JsonPatchDocument<Overtime> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Overtime.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Overtime.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}