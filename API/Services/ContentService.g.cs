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
    /// The contentService responsible for managing content related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting content information.
    /// </remarks>
    public interface IContentService
    {
        /// <summary>Retrieves a specific content by its primary key</summary>
        /// <param name="id">The primary key of the content</param>
        /// <returns>The content data</returns>
        Content GetById(Guid id);

        /// <summary>Retrieves a list of contents based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of contents</returns>
        List<Content> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new content</summary>
        /// <param name="model">The content data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Content model);

        /// <summary>Updates a specific content by its primary key</summary>
        /// <param name="id">The primary key of the content</param>
        /// <param name="updatedEntity">The content data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Content updatedEntity);

        /// <summary>Updates a specific content by its primary key</summary>
        /// <param name="id">The primary key of the content</param>
        /// <param name="updatedEntity">The content data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Content> updatedEntity);

        /// <summary>Deletes a specific content by its primary key</summary>
        /// <param name="id">The primary key of the content</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The contentService responsible for managing content related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting content information.
    /// </remarks>
    public class ContentService : IContentService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Content class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public ContentService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific content by its primary key</summary>
        /// <param name="id">The primary key of the content</param>
        /// <returns>The content data</returns>
        public Content GetById(Guid id)
        {
            var entityData = _dbContext.Content.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of contents based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of contents</returns>/// <exception cref="Exception"></exception>
        public List<Content> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetContent(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new content</summary>
        /// <param name="model">The content data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Content model)
        {
            model.Id = CreateContent(model);
            return model.Id;
        }

        /// <summary>Updates a specific content by its primary key</summary>
        /// <param name="id">The primary key of the content</param>
        /// <param name="updatedEntity">The content data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Content updatedEntity)
        {
            UpdateContent(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific content by its primary key</summary>
        /// <param name="id">The primary key of the content</param>
        /// <param name="updatedEntity">The content data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Content> updatedEntity)
        {
            PatchContent(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific content by its primary key</summary>
        /// <param name="id">The primary key of the content</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteContent(id);
            return true;
        }
        #region
        private List<Content> GetContent(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Content.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Content>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Content), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Content, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateContent(Content model)
        {
            _dbContext.Content.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateContent(Guid id, Content updatedEntity)
        {
            _dbContext.Content.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteContent(Guid id)
        {
            var entityData = _dbContext.Content.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Content.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchContent(Guid id, JsonPatchDocument<Content> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Content.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Content.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}