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
    /// The questiontypeService responsible for managing questiontype related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting questiontype information.
    /// </remarks>
    public interface IQuestionTypeService
    {
        /// <summary>Retrieves a specific questiontype by its primary key</summary>
        /// <param name="id">The primary key of the questiontype</param>
        /// <returns>The questiontype data</returns>
        QuestionType GetById(Guid id);

        /// <summary>Retrieves a list of questiontypes based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of questiontypes</returns>
        List<QuestionType> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new questiontype</summary>
        /// <param name="model">The questiontype data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(QuestionType model);

        /// <summary>Updates a specific questiontype by its primary key</summary>
        /// <param name="id">The primary key of the questiontype</param>
        /// <param name="updatedEntity">The questiontype data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, QuestionType updatedEntity);

        /// <summary>Updates a specific questiontype by its primary key</summary>
        /// <param name="id">The primary key of the questiontype</param>
        /// <param name="updatedEntity">The questiontype data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<QuestionType> updatedEntity);

        /// <summary>Deletes a specific questiontype by its primary key</summary>
        /// <param name="id">The primary key of the questiontype</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The questiontypeService responsible for managing questiontype related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting questiontype information.
    /// </remarks>
    public class QuestionTypeService : IQuestionTypeService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the QuestionType class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public QuestionTypeService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific questiontype by its primary key</summary>
        /// <param name="id">The primary key of the questiontype</param>
        /// <returns>The questiontype data</returns>
        public QuestionType GetById(Guid id)
        {
            var entityData = _dbContext.QuestionType.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of questiontypes based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of questiontypes</returns>/// <exception cref="Exception"></exception>
        public List<QuestionType> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetQuestionType(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new questiontype</summary>
        /// <param name="model">The questiontype data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(QuestionType model)
        {
            model.Id = CreateQuestionType(model);
            return model.Id;
        }

        /// <summary>Updates a specific questiontype by its primary key</summary>
        /// <param name="id">The primary key of the questiontype</param>
        /// <param name="updatedEntity">The questiontype data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, QuestionType updatedEntity)
        {
            UpdateQuestionType(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific questiontype by its primary key</summary>
        /// <param name="id">The primary key of the questiontype</param>
        /// <param name="updatedEntity">The questiontype data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<QuestionType> updatedEntity)
        {
            PatchQuestionType(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific questiontype by its primary key</summary>
        /// <param name="id">The primary key of the questiontype</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteQuestionType(id);
            return true;
        }
        #region
        private List<QuestionType> GetQuestionType(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.QuestionType.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<QuestionType>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(QuestionType), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<QuestionType, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateQuestionType(QuestionType model)
        {
            _dbContext.QuestionType.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateQuestionType(Guid id, QuestionType updatedEntity)
        {
            _dbContext.QuestionType.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteQuestionType(Guid id)
        {
            var entityData = _dbContext.QuestionType.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.QuestionType.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchQuestionType(Guid id, JsonPatchDocument<QuestionType> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.QuestionType.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.QuestionType.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}