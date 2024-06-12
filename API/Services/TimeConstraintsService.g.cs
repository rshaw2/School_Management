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
    /// The timeconstraintsService responsible for managing timeconstraints related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting timeconstraints information.
    /// </remarks>
    public interface ITimeConstraintsService
    {
        /// <summary>Retrieves a specific timeconstraints by its primary key</summary>
        /// <param name="id">The primary key of the timeconstraints</param>
        /// <returns>The timeconstraints data</returns>
        TimeConstraints GetById(Guid id);

        /// <summary>Retrieves a list of timeconstraintss based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of timeconstraintss</returns>
        List<TimeConstraints> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new timeconstraints</summary>
        /// <param name="model">The timeconstraints data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(TimeConstraints model);

        /// <summary>Updates a specific timeconstraints by its primary key</summary>
        /// <param name="id">The primary key of the timeconstraints</param>
        /// <param name="updatedEntity">The timeconstraints data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, TimeConstraints updatedEntity);

        /// <summary>Updates a specific timeconstraints by its primary key</summary>
        /// <param name="id">The primary key of the timeconstraints</param>
        /// <param name="updatedEntity">The timeconstraints data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<TimeConstraints> updatedEntity);

        /// <summary>Deletes a specific timeconstraints by its primary key</summary>
        /// <param name="id">The primary key of the timeconstraints</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The timeconstraintsService responsible for managing timeconstraints related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting timeconstraints information.
    /// </remarks>
    public class TimeConstraintsService : ITimeConstraintsService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the TimeConstraints class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public TimeConstraintsService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific timeconstraints by its primary key</summary>
        /// <param name="id">The primary key of the timeconstraints</param>
        /// <returns>The timeconstraints data</returns>
        public TimeConstraints GetById(Guid id)
        {
            var entityData = _dbContext.TimeConstraints.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of timeconstraintss based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of timeconstraintss</returns>/// <exception cref="Exception"></exception>
        public List<TimeConstraints> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetTimeConstraints(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new timeconstraints</summary>
        /// <param name="model">The timeconstraints data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(TimeConstraints model)
        {
            model.Id = CreateTimeConstraints(model);
            return model.Id;
        }

        /// <summary>Updates a specific timeconstraints by its primary key</summary>
        /// <param name="id">The primary key of the timeconstraints</param>
        /// <param name="updatedEntity">The timeconstraints data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, TimeConstraints updatedEntity)
        {
            UpdateTimeConstraints(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific timeconstraints by its primary key</summary>
        /// <param name="id">The primary key of the timeconstraints</param>
        /// <param name="updatedEntity">The timeconstraints data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<TimeConstraints> updatedEntity)
        {
            PatchTimeConstraints(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific timeconstraints by its primary key</summary>
        /// <param name="id">The primary key of the timeconstraints</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteTimeConstraints(id);
            return true;
        }
        #region
        private List<TimeConstraints> GetTimeConstraints(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.TimeConstraints.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<TimeConstraints>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(TimeConstraints), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<TimeConstraints, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateTimeConstraints(TimeConstraints model)
        {
            _dbContext.TimeConstraints.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateTimeConstraints(Guid id, TimeConstraints updatedEntity)
        {
            _dbContext.TimeConstraints.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteTimeConstraints(Guid id)
        {
            var entityData = _dbContext.TimeConstraints.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.TimeConstraints.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchTimeConstraints(Guid id, JsonPatchDocument<TimeConstraints> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.TimeConstraints.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.TimeConstraints.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}