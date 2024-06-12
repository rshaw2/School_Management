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
    /// The feescheduleService responsible for managing feeschedule related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting feeschedule information.
    /// </remarks>
    public interface IFeeScheduleService
    {
        /// <summary>Retrieves a specific feeschedule by its primary key</summary>
        /// <param name="id">The primary key of the feeschedule</param>
        /// <returns>The feeschedule data</returns>
        FeeSchedule GetById(Guid id);

        /// <summary>Retrieves a list of feeschedules based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of feeschedules</returns>
        List<FeeSchedule> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new feeschedule</summary>
        /// <param name="model">The feeschedule data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(FeeSchedule model);

        /// <summary>Updates a specific feeschedule by its primary key</summary>
        /// <param name="id">The primary key of the feeschedule</param>
        /// <param name="updatedEntity">The feeschedule data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, FeeSchedule updatedEntity);

        /// <summary>Updates a specific feeschedule by its primary key</summary>
        /// <param name="id">The primary key of the feeschedule</param>
        /// <param name="updatedEntity">The feeschedule data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<FeeSchedule> updatedEntity);

        /// <summary>Deletes a specific feeschedule by its primary key</summary>
        /// <param name="id">The primary key of the feeschedule</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The feescheduleService responsible for managing feeschedule related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting feeschedule information.
    /// </remarks>
    public class FeeScheduleService : IFeeScheduleService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the FeeSchedule class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public FeeScheduleService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific feeschedule by its primary key</summary>
        /// <param name="id">The primary key of the feeschedule</param>
        /// <returns>The feeschedule data</returns>
        public FeeSchedule GetById(Guid id)
        {
            var entityData = _dbContext.FeeSchedule.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of feeschedules based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of feeschedules</returns>/// <exception cref="Exception"></exception>
        public List<FeeSchedule> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetFeeSchedule(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new feeschedule</summary>
        /// <param name="model">The feeschedule data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(FeeSchedule model)
        {
            model.Id = CreateFeeSchedule(model);
            return model.Id;
        }

        /// <summary>Updates a specific feeschedule by its primary key</summary>
        /// <param name="id">The primary key of the feeschedule</param>
        /// <param name="updatedEntity">The feeschedule data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, FeeSchedule updatedEntity)
        {
            UpdateFeeSchedule(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific feeschedule by its primary key</summary>
        /// <param name="id">The primary key of the feeschedule</param>
        /// <param name="updatedEntity">The feeschedule data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<FeeSchedule> updatedEntity)
        {
            PatchFeeSchedule(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific feeschedule by its primary key</summary>
        /// <param name="id">The primary key of the feeschedule</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteFeeSchedule(id);
            return true;
        }
        #region
        private List<FeeSchedule> GetFeeSchedule(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.FeeSchedule.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<FeeSchedule>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(FeeSchedule), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<FeeSchedule, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateFeeSchedule(FeeSchedule model)
        {
            _dbContext.FeeSchedule.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateFeeSchedule(Guid id, FeeSchedule updatedEntity)
        {
            _dbContext.FeeSchedule.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteFeeSchedule(Guid id)
        {
            var entityData = _dbContext.FeeSchedule.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.FeeSchedule.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchFeeSchedule(Guid id, JsonPatchDocument<FeeSchedule> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.FeeSchedule.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.FeeSchedule.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}