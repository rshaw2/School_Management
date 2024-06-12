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
    /// The chatService responsible for managing chat related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting chat information.
    /// </remarks>
    public interface IChatService
    {
        /// <summary>Retrieves a specific chat by its primary key</summary>
        /// <param name="id">The primary key of the chat</param>
        /// <returns>The chat data</returns>
        Chat GetById(Guid id);

        /// <summary>Retrieves a list of chats based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of chats</returns>
        List<Chat> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new chat</summary>
        /// <param name="model">The chat data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Chat model);

        /// <summary>Updates a specific chat by its primary key</summary>
        /// <param name="id">The primary key of the chat</param>
        /// <param name="updatedEntity">The chat data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Chat updatedEntity);

        /// <summary>Updates a specific chat by its primary key</summary>
        /// <param name="id">The primary key of the chat</param>
        /// <param name="updatedEntity">The chat data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Chat> updatedEntity);

        /// <summary>Deletes a specific chat by its primary key</summary>
        /// <param name="id">The primary key of the chat</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The chatService responsible for managing chat related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting chat information.
    /// </remarks>
    public class ChatService : IChatService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Chat class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public ChatService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific chat by its primary key</summary>
        /// <param name="id">The primary key of the chat</param>
        /// <returns>The chat data</returns>
        public Chat GetById(Guid id)
        {
            var entityData = _dbContext.Chat.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of chats based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of chats</returns>/// <exception cref="Exception"></exception>
        public List<Chat> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetChat(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new chat</summary>
        /// <param name="model">The chat data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Chat model)
        {
            model.Id = CreateChat(model);
            return model.Id;
        }

        /// <summary>Updates a specific chat by its primary key</summary>
        /// <param name="id">The primary key of the chat</param>
        /// <param name="updatedEntity">The chat data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Chat updatedEntity)
        {
            UpdateChat(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific chat by its primary key</summary>
        /// <param name="id">The primary key of the chat</param>
        /// <param name="updatedEntity">The chat data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Chat> updatedEntity)
        {
            PatchChat(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific chat by its primary key</summary>
        /// <param name="id">The primary key of the chat</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteChat(id);
            return true;
        }
        #region
        private List<Chat> GetChat(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Chat.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Chat>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Chat), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Chat, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateChat(Chat model)
        {
            _dbContext.Chat.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateChat(Guid id, Chat updatedEntity)
        {
            _dbContext.Chat.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteChat(Guid id)
        {
            var entityData = _dbContext.Chat.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Chat.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchChat(Guid id, JsonPatchDocument<Chat> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Chat.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Chat.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}