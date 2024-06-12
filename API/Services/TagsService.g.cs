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
    /// The tagsService responsible for managing tags related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting tags information.
    /// </remarks>
    public interface ITagsService
    {
        /// <summary>Retrieves a specific tags by its primary key</summary>
        /// <param name="id">The primary key of the tags</param>
        /// <returns>The tags data</returns>
        Tags GetById(Guid id);

        /// <summary>Retrieves a list of tagss based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of tagss</returns>
        List<Tags> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new tags</summary>
        /// <param name="model">The tags data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Tags model);

        /// <summary>Updates a specific tags by its primary key</summary>
        /// <param name="id">The primary key of the tags</param>
        /// <param name="updatedEntity">The tags data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Tags updatedEntity);

        /// <summary>Updates a specific tags by its primary key</summary>
        /// <param name="id">The primary key of the tags</param>
        /// <param name="updatedEntity">The tags data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Tags> updatedEntity);

        /// <summary>Deletes a specific tags by its primary key</summary>
        /// <param name="id">The primary key of the tags</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The tagsService responsible for managing tags related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting tags information.
    /// </remarks>
    public class TagsService : ITagsService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Tags class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public TagsService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific tags by its primary key</summary>
        /// <param name="id">The primary key of the tags</param>
        /// <returns>The tags data</returns>
        public Tags GetById(Guid id)
        {
            var entityData = _dbContext.Tags.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of tagss based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of tagss</returns>/// <exception cref="Exception"></exception>
        public List<Tags> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetTags(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new tags</summary>
        /// <param name="model">The tags data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Tags model)
        {
            model.Id = CreateTags(model);
            return model.Id;
        }

        /// <summary>Updates a specific tags by its primary key</summary>
        /// <param name="id">The primary key of the tags</param>
        /// <param name="updatedEntity">The tags data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Tags updatedEntity)
        {
            UpdateTags(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific tags by its primary key</summary>
        /// <param name="id">The primary key of the tags</param>
        /// <param name="updatedEntity">The tags data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Tags> updatedEntity)
        {
            PatchTags(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific tags by its primary key</summary>
        /// <param name="id">The primary key of the tags</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteTags(id);
            return true;
        }
        #region
        private List<Tags> GetTags(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Tags.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Tags>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Tags), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Tags, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateTags(Tags model)
        {
            _dbContext.Tags.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateTags(Guid id, Tags updatedEntity)
        {
            _dbContext.Tags.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteTags(Guid id)
        {
            var entityData = _dbContext.Tags.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Tags.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchTags(Guid id, JsonPatchDocument<Tags> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Tags.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Tags.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}