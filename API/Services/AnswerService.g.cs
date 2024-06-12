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
    /// The answerService responsible for managing answer related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting answer information.
    /// </remarks>
    public interface IAnswerService
    {
        /// <summary>Retrieves a specific answer by its primary key</summary>
        /// <param name="id">The primary key of the answer</param>
        /// <returns>The answer data</returns>
        Answer GetById(Guid id);

        /// <summary>Retrieves a list of answers based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of answers</returns>
        List<Answer> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new answer</summary>
        /// <param name="model">The answer data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Answer model);

        /// <summary>Updates a specific answer by its primary key</summary>
        /// <param name="id">The primary key of the answer</param>
        /// <param name="updatedEntity">The answer data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Answer updatedEntity);

        /// <summary>Updates a specific answer by its primary key</summary>
        /// <param name="id">The primary key of the answer</param>
        /// <param name="updatedEntity">The answer data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Answer> updatedEntity);

        /// <summary>Deletes a specific answer by its primary key</summary>
        /// <param name="id">The primary key of the answer</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The answerService responsible for managing answer related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting answer information.
    /// </remarks>
    public class AnswerService : IAnswerService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Answer class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public AnswerService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific answer by its primary key</summary>
        /// <param name="id">The primary key of the answer</param>
        /// <returns>The answer data</returns>
        public Answer GetById(Guid id)
        {
            var entityData = _dbContext.Answer.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of answers based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of answers</returns>/// <exception cref="Exception"></exception>
        public List<Answer> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetAnswer(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new answer</summary>
        /// <param name="model">The answer data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Answer model)
        {
            model.Id = CreateAnswer(model);
            return model.Id;
        }

        /// <summary>Updates a specific answer by its primary key</summary>
        /// <param name="id">The primary key of the answer</param>
        /// <param name="updatedEntity">The answer data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Answer updatedEntity)
        {
            UpdateAnswer(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific answer by its primary key</summary>
        /// <param name="id">The primary key of the answer</param>
        /// <param name="updatedEntity">The answer data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Answer> updatedEntity)
        {
            PatchAnswer(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific answer by its primary key</summary>
        /// <param name="id">The primary key of the answer</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteAnswer(id);
            return true;
        }
        #region
        private List<Answer> GetAnswer(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Answer.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Answer>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Answer), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Answer, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateAnswer(Answer model)
        {
            _dbContext.Answer.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateAnswer(Guid id, Answer updatedEntity)
        {
            _dbContext.Answer.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteAnswer(Guid id)
        {
            var entityData = _dbContext.Answer.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Answer.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchAnswer(Guid id, JsonPatchDocument<Answer> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Answer.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Answer.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}