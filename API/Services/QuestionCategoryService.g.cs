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
    /// The questioncategoryService responsible for managing questioncategory related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting questioncategory information.
    /// </remarks>
    public interface IQuestionCategoryService
    {
        /// <summary>Retrieves a specific questioncategory by its primary key</summary>
        /// <param name="id">The primary key of the questioncategory</param>
        /// <returns>The questioncategory data</returns>
        QuestionCategory GetById(Guid id);

        /// <summary>Retrieves a list of questioncategorys based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of questioncategorys</returns>
        List<QuestionCategory> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new questioncategory</summary>
        /// <param name="model">The questioncategory data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(QuestionCategory model);

        /// <summary>Updates a specific questioncategory by its primary key</summary>
        /// <param name="id">The primary key of the questioncategory</param>
        /// <param name="updatedEntity">The questioncategory data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, QuestionCategory updatedEntity);

        /// <summary>Updates a specific questioncategory by its primary key</summary>
        /// <param name="id">The primary key of the questioncategory</param>
        /// <param name="updatedEntity">The questioncategory data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<QuestionCategory> updatedEntity);

        /// <summary>Deletes a specific questioncategory by its primary key</summary>
        /// <param name="id">The primary key of the questioncategory</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The questioncategoryService responsible for managing questioncategory related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting questioncategory information.
    /// </remarks>
    public class QuestionCategoryService : IQuestionCategoryService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the QuestionCategory class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public QuestionCategoryService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific questioncategory by its primary key</summary>
        /// <param name="id">The primary key of the questioncategory</param>
        /// <returns>The questioncategory data</returns>
        public QuestionCategory GetById(Guid id)
        {
            var entityData = _dbContext.QuestionCategory.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of questioncategorys based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of questioncategorys</returns>/// <exception cref="Exception"></exception>
        public List<QuestionCategory> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetQuestionCategory(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new questioncategory</summary>
        /// <param name="model">The questioncategory data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(QuestionCategory model)
        {
            model.Id = CreateQuestionCategory(model);
            return model.Id;
        }

        /// <summary>Updates a specific questioncategory by its primary key</summary>
        /// <param name="id">The primary key of the questioncategory</param>
        /// <param name="updatedEntity">The questioncategory data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, QuestionCategory updatedEntity)
        {
            UpdateQuestionCategory(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific questioncategory by its primary key</summary>
        /// <param name="id">The primary key of the questioncategory</param>
        /// <param name="updatedEntity">The questioncategory data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<QuestionCategory> updatedEntity)
        {
            PatchQuestionCategory(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific questioncategory by its primary key</summary>
        /// <param name="id">The primary key of the questioncategory</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteQuestionCategory(id);
            return true;
        }
        #region
        private List<QuestionCategory> GetQuestionCategory(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.QuestionCategory.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<QuestionCategory>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(QuestionCategory), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<QuestionCategory, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateQuestionCategory(QuestionCategory model)
        {
            _dbContext.QuestionCategory.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateQuestionCategory(Guid id, QuestionCategory updatedEntity)
        {
            _dbContext.QuestionCategory.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteQuestionCategory(Guid id)
        {
            var entityData = _dbContext.QuestionCategory.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.QuestionCategory.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchQuestionCategory(Guid id, JsonPatchDocument<QuestionCategory> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.QuestionCategory.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.QuestionCategory.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}