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
    /// The discussionService responsible for managing discussion related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting discussion information.
    /// </remarks>
    public interface IDiscussionService
    {
        /// <summary>Retrieves a specific discussion by its primary key</summary>
        /// <param name="id">The primary key of the discussion</param>
        /// <returns>The discussion data</returns>
        Discussion GetById(Guid id);

        /// <summary>Retrieves a list of discussions based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of discussions</returns>
        List<Discussion> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new discussion</summary>
        /// <param name="model">The discussion data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Discussion model);

        /// <summary>Updates a specific discussion by its primary key</summary>
        /// <param name="id">The primary key of the discussion</param>
        /// <param name="updatedEntity">The discussion data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Discussion updatedEntity);

        /// <summary>Updates a specific discussion by its primary key</summary>
        /// <param name="id">The primary key of the discussion</param>
        /// <param name="updatedEntity">The discussion data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Discussion> updatedEntity);

        /// <summary>Deletes a specific discussion by its primary key</summary>
        /// <param name="id">The primary key of the discussion</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The discussionService responsible for managing discussion related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting discussion information.
    /// </remarks>
    public class DiscussionService : IDiscussionService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Discussion class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public DiscussionService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific discussion by its primary key</summary>
        /// <param name="id">The primary key of the discussion</param>
        /// <returns>The discussion data</returns>
        public Discussion GetById(Guid id)
        {
            var entityData = _dbContext.Discussion.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of discussions based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of discussions</returns>/// <exception cref="Exception"></exception>
        public List<Discussion> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetDiscussion(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new discussion</summary>
        /// <param name="model">The discussion data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Discussion model)
        {
            model.Id = CreateDiscussion(model);
            return model.Id;
        }

        /// <summary>Updates a specific discussion by its primary key</summary>
        /// <param name="id">The primary key of the discussion</param>
        /// <param name="updatedEntity">The discussion data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Discussion updatedEntity)
        {
            UpdateDiscussion(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific discussion by its primary key</summary>
        /// <param name="id">The primary key of the discussion</param>
        /// <param name="updatedEntity">The discussion data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Discussion> updatedEntity)
        {
            PatchDiscussion(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific discussion by its primary key</summary>
        /// <param name="id">The primary key of the discussion</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteDiscussion(id);
            return true;
        }
        #region
        private List<Discussion> GetDiscussion(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Discussion.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Discussion>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Discussion), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Discussion, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateDiscussion(Discussion model)
        {
            _dbContext.Discussion.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateDiscussion(Guid id, Discussion updatedEntity)
        {
            _dbContext.Discussion.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteDiscussion(Guid id)
        {
            var entityData = _dbContext.Discussion.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Discussion.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchDiscussion(Guid id, JsonPatchDocument<Discussion> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Discussion.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Discussion.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}