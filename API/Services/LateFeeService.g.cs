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
    /// The latefeeService responsible for managing latefee related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting latefee information.
    /// </remarks>
    public interface ILateFeeService
    {
        /// <summary>Retrieves a specific latefee by its primary key</summary>
        /// <param name="id">The primary key of the latefee</param>
        /// <returns>The latefee data</returns>
        LateFee GetById(Guid id);

        /// <summary>Retrieves a list of latefees based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of latefees</returns>
        List<LateFee> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new latefee</summary>
        /// <param name="model">The latefee data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(LateFee model);

        /// <summary>Updates a specific latefee by its primary key</summary>
        /// <param name="id">The primary key of the latefee</param>
        /// <param name="updatedEntity">The latefee data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, LateFee updatedEntity);

        /// <summary>Updates a specific latefee by its primary key</summary>
        /// <param name="id">The primary key of the latefee</param>
        /// <param name="updatedEntity">The latefee data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<LateFee> updatedEntity);

        /// <summary>Deletes a specific latefee by its primary key</summary>
        /// <param name="id">The primary key of the latefee</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The latefeeService responsible for managing latefee related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting latefee information.
    /// </remarks>
    public class LateFeeService : ILateFeeService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the LateFee class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public LateFeeService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific latefee by its primary key</summary>
        /// <param name="id">The primary key of the latefee</param>
        /// <returns>The latefee data</returns>
        public LateFee GetById(Guid id)
        {
            var entityData = _dbContext.LateFee.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of latefees based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of latefees</returns>/// <exception cref="Exception"></exception>
        public List<LateFee> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetLateFee(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new latefee</summary>
        /// <param name="model">The latefee data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(LateFee model)
        {
            model.Id = CreateLateFee(model);
            return model.Id;
        }

        /// <summary>Updates a specific latefee by its primary key</summary>
        /// <param name="id">The primary key of the latefee</param>
        /// <param name="updatedEntity">The latefee data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, LateFee updatedEntity)
        {
            UpdateLateFee(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific latefee by its primary key</summary>
        /// <param name="id">The primary key of the latefee</param>
        /// <param name="updatedEntity">The latefee data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<LateFee> updatedEntity)
        {
            PatchLateFee(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific latefee by its primary key</summary>
        /// <param name="id">The primary key of the latefee</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteLateFee(id);
            return true;
        }
        #region
        private List<LateFee> GetLateFee(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.LateFee.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<LateFee>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(LateFee), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<LateFee, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateLateFee(LateFee model)
        {
            _dbContext.LateFee.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateLateFee(Guid id, LateFee updatedEntity)
        {
            _dbContext.LateFee.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteLateFee(Guid id)
        {
            var entityData = _dbContext.LateFee.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.LateFee.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchLateFee(Guid id, JsonPatchDocument<LateFee> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.LateFee.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.LateFee.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}