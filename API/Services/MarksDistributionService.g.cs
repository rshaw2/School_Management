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
    /// The marksdistributionService responsible for managing marksdistribution related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting marksdistribution information.
    /// </remarks>
    public interface IMarksDistributionService
    {
        /// <summary>Retrieves a specific marksdistribution by its primary key</summary>
        /// <param name="id">The primary key of the marksdistribution</param>
        /// <returns>The marksdistribution data</returns>
        MarksDistribution GetById(Guid id);

        /// <summary>Retrieves a list of marksdistributions based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of marksdistributions</returns>
        List<MarksDistribution> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new marksdistribution</summary>
        /// <param name="model">The marksdistribution data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(MarksDistribution model);

        /// <summary>Updates a specific marksdistribution by its primary key</summary>
        /// <param name="id">The primary key of the marksdistribution</param>
        /// <param name="updatedEntity">The marksdistribution data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, MarksDistribution updatedEntity);

        /// <summary>Updates a specific marksdistribution by its primary key</summary>
        /// <param name="id">The primary key of the marksdistribution</param>
        /// <param name="updatedEntity">The marksdistribution data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<MarksDistribution> updatedEntity);

        /// <summary>Deletes a specific marksdistribution by its primary key</summary>
        /// <param name="id">The primary key of the marksdistribution</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The marksdistributionService responsible for managing marksdistribution related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting marksdistribution information.
    /// </remarks>
    public class MarksDistributionService : IMarksDistributionService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the MarksDistribution class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public MarksDistributionService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific marksdistribution by its primary key</summary>
        /// <param name="id">The primary key of the marksdistribution</param>
        /// <returns>The marksdistribution data</returns>
        public MarksDistribution GetById(Guid id)
        {
            var entityData = _dbContext.MarksDistribution.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of marksdistributions based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of marksdistributions</returns>/// <exception cref="Exception"></exception>
        public List<MarksDistribution> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetMarksDistribution(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new marksdistribution</summary>
        /// <param name="model">The marksdistribution data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(MarksDistribution model)
        {
            model.Id = CreateMarksDistribution(model);
            return model.Id;
        }

        /// <summary>Updates a specific marksdistribution by its primary key</summary>
        /// <param name="id">The primary key of the marksdistribution</param>
        /// <param name="updatedEntity">The marksdistribution data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, MarksDistribution updatedEntity)
        {
            UpdateMarksDistribution(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific marksdistribution by its primary key</summary>
        /// <param name="id">The primary key of the marksdistribution</param>
        /// <param name="updatedEntity">The marksdistribution data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<MarksDistribution> updatedEntity)
        {
            PatchMarksDistribution(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific marksdistribution by its primary key</summary>
        /// <param name="id">The primary key of the marksdistribution</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteMarksDistribution(id);
            return true;
        }
        #region
        private List<MarksDistribution> GetMarksDistribution(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.MarksDistribution.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<MarksDistribution>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(MarksDistribution), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<MarksDistribution, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateMarksDistribution(MarksDistribution model)
        {
            _dbContext.MarksDistribution.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateMarksDistribution(Guid id, MarksDistribution updatedEntity)
        {
            _dbContext.MarksDistribution.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteMarksDistribution(Guid id)
        {
            var entityData = _dbContext.MarksDistribution.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.MarksDistribution.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchMarksDistribution(Guid id, JsonPatchDocument<MarksDistribution> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.MarksDistribution.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.MarksDistribution.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}