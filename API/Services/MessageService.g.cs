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
    /// The messageService responsible for managing message related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting message information.
    /// </remarks>
    public interface IMessageService
    {
        /// <summary>Retrieves a specific message by its primary key</summary>
        /// <param name="id">The primary key of the message</param>
        /// <returns>The message data</returns>
        Message GetById(Guid id);

        /// <summary>Retrieves a list of messages based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of messages</returns>
        List<Message> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new message</summary>
        /// <param name="model">The message data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Message model);

        /// <summary>Updates a specific message by its primary key</summary>
        /// <param name="id">The primary key of the message</param>
        /// <param name="updatedEntity">The message data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Message updatedEntity);

        /// <summary>Updates a specific message by its primary key</summary>
        /// <param name="id">The primary key of the message</param>
        /// <param name="updatedEntity">The message data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Message> updatedEntity);

        /// <summary>Deletes a specific message by its primary key</summary>
        /// <param name="id">The primary key of the message</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The messageService responsible for managing message related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting message information.
    /// </remarks>
    public class MessageService : IMessageService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Message class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public MessageService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific message by its primary key</summary>
        /// <param name="id">The primary key of the message</param>
        /// <returns>The message data</returns>
        public Message GetById(Guid id)
        {
            var entityData = _dbContext.Message.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of messages based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of messages</returns>/// <exception cref="Exception"></exception>
        public List<Message> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetMessage(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new message</summary>
        /// <param name="model">The message data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Message model)
        {
            model.Id = CreateMessage(model);
            return model.Id;
        }

        /// <summary>Updates a specific message by its primary key</summary>
        /// <param name="id">The primary key of the message</param>
        /// <param name="updatedEntity">The message data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Message updatedEntity)
        {
            UpdateMessage(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific message by its primary key</summary>
        /// <param name="id">The primary key of the message</param>
        /// <param name="updatedEntity">The message data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Message> updatedEntity)
        {
            PatchMessage(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific message by its primary key</summary>
        /// <param name="id">The primary key of the message</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteMessage(id);
            return true;
        }
        #region
        private List<Message> GetMessage(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Message.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Message>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Message), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Message, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateMessage(Message model)
        {
            _dbContext.Message.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateMessage(Guid id, Message updatedEntity)
        {
            _dbContext.Message.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteMessage(Guid id)
        {
            var entityData = _dbContext.Message.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Message.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchMessage(Guid id, JsonPatchDocument<Message> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Message.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Message.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}