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
    /// The activityfeedService responsible for managing activityfeed related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting activityfeed information.
    /// </remarks>
    public interface IActivityFeedService
    {
        /// <summary>Retrieves a specific activityfeed by its primary key</summary>
        /// <param name="id">The primary key of the activityfeed</param>
        /// <returns>The activityfeed data</returns>
        ActivityFeed GetById(Guid id);

        /// <summary>Retrieves a list of activityfeeds based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of activityfeeds</returns>
        List<ActivityFeed> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new activityfeed</summary>
        /// <param name="model">The activityfeed data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(ActivityFeed model);

        /// <summary>Updates a specific activityfeed by its primary key</summary>
        /// <param name="id">The primary key of the activityfeed</param>
        /// <param name="updatedEntity">The activityfeed data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, ActivityFeed updatedEntity);

        /// <summary>Updates a specific activityfeed by its primary key</summary>
        /// <param name="id">The primary key of the activityfeed</param>
        /// <param name="updatedEntity">The activityfeed data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<ActivityFeed> updatedEntity);

        /// <summary>Deletes a specific activityfeed by its primary key</summary>
        /// <param name="id">The primary key of the activityfeed</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The activityfeedService responsible for managing activityfeed related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting activityfeed information.
    /// </remarks>
    public class ActivityFeedService : IActivityFeedService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the ActivityFeed class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public ActivityFeedService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific activityfeed by its primary key</summary>
        /// <param name="id">The primary key of the activityfeed</param>
        /// <returns>The activityfeed data</returns>
        public ActivityFeed GetById(Guid id)
        {
            var entityData = _dbContext.ActivityFeed.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of activityfeeds based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of activityfeeds</returns>/// <exception cref="Exception"></exception>
        public List<ActivityFeed> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetActivityFeed(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new activityfeed</summary>
        /// <param name="model">The activityfeed data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(ActivityFeed model)
        {
            model.Id = CreateActivityFeed(model);
            return model.Id;
        }

        /// <summary>Updates a specific activityfeed by its primary key</summary>
        /// <param name="id">The primary key of the activityfeed</param>
        /// <param name="updatedEntity">The activityfeed data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, ActivityFeed updatedEntity)
        {
            UpdateActivityFeed(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific activityfeed by its primary key</summary>
        /// <param name="id">The primary key of the activityfeed</param>
        /// <param name="updatedEntity">The activityfeed data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<ActivityFeed> updatedEntity)
        {
            PatchActivityFeed(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific activityfeed by its primary key</summary>
        /// <param name="id">The primary key of the activityfeed</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteActivityFeed(id);
            return true;
        }
        #region
        private List<ActivityFeed> GetActivityFeed(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.ActivityFeed.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<ActivityFeed>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(ActivityFeed), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<ActivityFeed, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateActivityFeed(ActivityFeed model)
        {
            _dbContext.ActivityFeed.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateActivityFeed(Guid id, ActivityFeed updatedEntity)
        {
            _dbContext.ActivityFeed.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteActivityFeed(Guid id)
        {
            var entityData = _dbContext.ActivityFeed.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.ActivityFeed.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchActivityFeed(Guid id, JsonPatchDocument<ActivityFeed> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.ActivityFeed.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.ActivityFeed.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}