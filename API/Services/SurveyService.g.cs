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
    /// The surveyService responsible for managing survey related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting survey information.
    /// </remarks>
    public interface ISurveyService
    {
        /// <summary>Retrieves a specific survey by its primary key</summary>
        /// <param name="id">The primary key of the survey</param>
        /// <returns>The survey data</returns>
        Survey GetById(Guid id);

        /// <summary>Retrieves a list of surveys based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of surveys</returns>
        List<Survey> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new survey</summary>
        /// <param name="model">The survey data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Survey model);

        /// <summary>Updates a specific survey by its primary key</summary>
        /// <param name="id">The primary key of the survey</param>
        /// <param name="updatedEntity">The survey data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Survey updatedEntity);

        /// <summary>Updates a specific survey by its primary key</summary>
        /// <param name="id">The primary key of the survey</param>
        /// <param name="updatedEntity">The survey data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Survey> updatedEntity);

        /// <summary>Deletes a specific survey by its primary key</summary>
        /// <param name="id">The primary key of the survey</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The surveyService responsible for managing survey related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting survey information.
    /// </remarks>
    public class SurveyService : ISurveyService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Survey class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public SurveyService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific survey by its primary key</summary>
        /// <param name="id">The primary key of the survey</param>
        /// <returns>The survey data</returns>
        public Survey GetById(Guid id)
        {
            var entityData = _dbContext.Survey.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of surveys based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of surveys</returns>/// <exception cref="Exception"></exception>
        public List<Survey> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetSurvey(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new survey</summary>
        /// <param name="model">The survey data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Survey model)
        {
            model.Id = CreateSurvey(model);
            return model.Id;
        }

        /// <summary>Updates a specific survey by its primary key</summary>
        /// <param name="id">The primary key of the survey</param>
        /// <param name="updatedEntity">The survey data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Survey updatedEntity)
        {
            UpdateSurvey(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific survey by its primary key</summary>
        /// <param name="id">The primary key of the survey</param>
        /// <param name="updatedEntity">The survey data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Survey> updatedEntity)
        {
            PatchSurvey(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific survey by its primary key</summary>
        /// <param name="id">The primary key of the survey</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteSurvey(id);
            return true;
        }
        #region
        private List<Survey> GetSurvey(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Survey.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Survey>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Survey), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Survey, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateSurvey(Survey model)
        {
            _dbContext.Survey.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateSurvey(Guid id, Survey updatedEntity)
        {
            _dbContext.Survey.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteSurvey(Guid id)
        {
            var entityData = _dbContext.Survey.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Survey.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchSurvey(Guid id, JsonPatchDocument<Survey> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Survey.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Survey.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}