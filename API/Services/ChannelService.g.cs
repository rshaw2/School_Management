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
    /// The channelService responsible for managing channel related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting channel information.
    /// </remarks>
    public interface IChannelService
    {
        /// <summary>Retrieves a specific channel by its primary key</summary>
        /// <param name="id">The primary key of the channel</param>
        /// <returns>The channel data</returns>
        Channel GetById(Guid id);

        /// <summary>Retrieves a list of channels based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of channels</returns>
        List<Channel> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new channel</summary>
        /// <param name="model">The channel data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Channel model);

        /// <summary>Updates a specific channel by its primary key</summary>
        /// <param name="id">The primary key of the channel</param>
        /// <param name="updatedEntity">The channel data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Channel updatedEntity);

        /// <summary>Updates a specific channel by its primary key</summary>
        /// <param name="id">The primary key of the channel</param>
        /// <param name="updatedEntity">The channel data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Channel> updatedEntity);

        /// <summary>Deletes a specific channel by its primary key</summary>
        /// <param name="id">The primary key of the channel</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The channelService responsible for managing channel related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting channel information.
    /// </remarks>
    public class ChannelService : IChannelService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Channel class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public ChannelService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific channel by its primary key</summary>
        /// <param name="id">The primary key of the channel</param>
        /// <returns>The channel data</returns>
        public Channel GetById(Guid id)
        {
            var entityData = _dbContext.Channel.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of channels based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of channels</returns>/// <exception cref="Exception"></exception>
        public List<Channel> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetChannel(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new channel</summary>
        /// <param name="model">The channel data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Channel model)
        {
            model.Id = CreateChannel(model);
            return model.Id;
        }

        /// <summary>Updates a specific channel by its primary key</summary>
        /// <param name="id">The primary key of the channel</param>
        /// <param name="updatedEntity">The channel data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Channel updatedEntity)
        {
            UpdateChannel(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific channel by its primary key</summary>
        /// <param name="id">The primary key of the channel</param>
        /// <param name="updatedEntity">The channel data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Channel> updatedEntity)
        {
            PatchChannel(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific channel by its primary key</summary>
        /// <param name="id">The primary key of the channel</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteChannel(id);
            return true;
        }
        #region
        private List<Channel> GetChannel(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Channel.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Channel>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Channel), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Channel, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateChannel(Channel model)
        {
            _dbContext.Channel.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateChannel(Guid id, Channel updatedEntity)
        {
            _dbContext.Channel.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteChannel(Guid id)
        {
            var entityData = _dbContext.Channel.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Channel.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchChannel(Guid id, JsonPatchDocument<Channel> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Channel.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Channel.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}