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
    /// The commentService responsible for managing comment related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting comment information.
    /// </remarks>
    public interface ICommentService
    {
        /// <summary>Retrieves a specific comment by its primary key</summary>
        /// <param name="id">The primary key of the comment</param>
        /// <returns>The comment data</returns>
        Comment GetById(Guid id);

        /// <summary>Retrieves a list of comments based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of comments</returns>
        List<Comment> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new comment</summary>
        /// <param name="model">The comment data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Comment model);

        /// <summary>Updates a specific comment by its primary key</summary>
        /// <param name="id">The primary key of the comment</param>
        /// <param name="updatedEntity">The comment data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Comment updatedEntity);

        /// <summary>Updates a specific comment by its primary key</summary>
        /// <param name="id">The primary key of the comment</param>
        /// <param name="updatedEntity">The comment data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Comment> updatedEntity);

        /// <summary>Deletes a specific comment by its primary key</summary>
        /// <param name="id">The primary key of the comment</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The commentService responsible for managing comment related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting comment information.
    /// </remarks>
    public class CommentService : ICommentService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Comment class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public CommentService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific comment by its primary key</summary>
        /// <param name="id">The primary key of the comment</param>
        /// <returns>The comment data</returns>
        public Comment GetById(Guid id)
        {
            var entityData = _dbContext.Comment.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of comments based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of comments</returns>/// <exception cref="Exception"></exception>
        public List<Comment> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetComment(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new comment</summary>
        /// <param name="model">The comment data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Comment model)
        {
            model.Id = CreateComment(model);
            return model.Id;
        }

        /// <summary>Updates a specific comment by its primary key</summary>
        /// <param name="id">The primary key of the comment</param>
        /// <param name="updatedEntity">The comment data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Comment updatedEntity)
        {
            UpdateComment(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific comment by its primary key</summary>
        /// <param name="id">The primary key of the comment</param>
        /// <param name="updatedEntity">The comment data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Comment> updatedEntity)
        {
            PatchComment(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific comment by its primary key</summary>
        /// <param name="id">The primary key of the comment</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteComment(id);
            return true;
        }
        #region
        private List<Comment> GetComment(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Comment.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Comment>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Comment), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Comment, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateComment(Comment model)
        {
            _dbContext.Comment.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateComment(Guid id, Comment updatedEntity)
        {
            _dbContext.Comment.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteComment(Guid id)
        {
            var entityData = _dbContext.Comment.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Comment.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchComment(Guid id, JsonPatchDocument<Comment> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Comment.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Comment.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}