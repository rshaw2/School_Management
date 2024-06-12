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
    /// The pollService responsible for managing poll related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting poll information.
    /// </remarks>
    public interface IPollService
    {
        /// <summary>Retrieves a specific poll by its primary key</summary>
        /// <param name="id">The primary key of the poll</param>
        /// <returns>The poll data</returns>
        Poll GetById(Guid id);

        /// <summary>Retrieves a list of polls based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of polls</returns>
        List<Poll> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new poll</summary>
        /// <param name="model">The poll data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Poll model);

        /// <summary>Updates a specific poll by its primary key</summary>
        /// <param name="id">The primary key of the poll</param>
        /// <param name="updatedEntity">The poll data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Poll updatedEntity);

        /// <summary>Updates a specific poll by its primary key</summary>
        /// <param name="id">The primary key of the poll</param>
        /// <param name="updatedEntity">The poll data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Poll> updatedEntity);

        /// <summary>Deletes a specific poll by its primary key</summary>
        /// <param name="id">The primary key of the poll</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The pollService responsible for managing poll related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting poll information.
    /// </remarks>
    public class PollService : IPollService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Poll class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public PollService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific poll by its primary key</summary>
        /// <param name="id">The primary key of the poll</param>
        /// <returns>The poll data</returns>
        public Poll GetById(Guid id)
        {
            var entityData = _dbContext.Poll.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of polls based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of polls</returns>/// <exception cref="Exception"></exception>
        public List<Poll> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetPoll(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new poll</summary>
        /// <param name="model">The poll data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Poll model)
        {
            model.Id = CreatePoll(model);
            return model.Id;
        }

        /// <summary>Updates a specific poll by its primary key</summary>
        /// <param name="id">The primary key of the poll</param>
        /// <param name="updatedEntity">The poll data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Poll updatedEntity)
        {
            UpdatePoll(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific poll by its primary key</summary>
        /// <param name="id">The primary key of the poll</param>
        /// <param name="updatedEntity">The poll data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Poll> updatedEntity)
        {
            PatchPoll(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific poll by its primary key</summary>
        /// <param name="id">The primary key of the poll</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeletePoll(id);
            return true;
        }
        #region
        private List<Poll> GetPoll(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Poll.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Poll>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Poll), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Poll, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreatePoll(Poll model)
        {
            _dbContext.Poll.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdatePoll(Guid id, Poll updatedEntity)
        {
            _dbContext.Poll.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeletePoll(Guid id)
        {
            var entityData = _dbContext.Poll.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Poll.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchPoll(Guid id, JsonPatchDocument<Poll> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Poll.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Poll.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}