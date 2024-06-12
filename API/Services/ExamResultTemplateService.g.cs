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
    /// The examresulttemplateService responsible for managing examresulttemplate related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting examresulttemplate information.
    /// </remarks>
    public interface IExamResultTemplateService
    {
        /// <summary>Retrieves a specific examresulttemplate by its primary key</summary>
        /// <param name="id">The primary key of the examresulttemplate</param>
        /// <returns>The examresulttemplate data</returns>
        ExamResultTemplate GetById(Guid id);

        /// <summary>Retrieves a list of examresulttemplates based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of examresulttemplates</returns>
        List<ExamResultTemplate> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new examresulttemplate</summary>
        /// <param name="model">The examresulttemplate data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(ExamResultTemplate model);

        /// <summary>Updates a specific examresulttemplate by its primary key</summary>
        /// <param name="id">The primary key of the examresulttemplate</param>
        /// <param name="updatedEntity">The examresulttemplate data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, ExamResultTemplate updatedEntity);

        /// <summary>Updates a specific examresulttemplate by its primary key</summary>
        /// <param name="id">The primary key of the examresulttemplate</param>
        /// <param name="updatedEntity">The examresulttemplate data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<ExamResultTemplate> updatedEntity);

        /// <summary>Deletes a specific examresulttemplate by its primary key</summary>
        /// <param name="id">The primary key of the examresulttemplate</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The examresulttemplateService responsible for managing examresulttemplate related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting examresulttemplate information.
    /// </remarks>
    public class ExamResultTemplateService : IExamResultTemplateService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the ExamResultTemplate class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public ExamResultTemplateService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific examresulttemplate by its primary key</summary>
        /// <param name="id">The primary key of the examresulttemplate</param>
        /// <returns>The examresulttemplate data</returns>
        public ExamResultTemplate GetById(Guid id)
        {
            var entityData = _dbContext.ExamResultTemplate.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of examresulttemplates based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of examresulttemplates</returns>/// <exception cref="Exception"></exception>
        public List<ExamResultTemplate> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetExamResultTemplate(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new examresulttemplate</summary>
        /// <param name="model">The examresulttemplate data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(ExamResultTemplate model)
        {
            model.Id = CreateExamResultTemplate(model);
            return model.Id;
        }

        /// <summary>Updates a specific examresulttemplate by its primary key</summary>
        /// <param name="id">The primary key of the examresulttemplate</param>
        /// <param name="updatedEntity">The examresulttemplate data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, ExamResultTemplate updatedEntity)
        {
            UpdateExamResultTemplate(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific examresulttemplate by its primary key</summary>
        /// <param name="id">The primary key of the examresulttemplate</param>
        /// <param name="updatedEntity">The examresulttemplate data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<ExamResultTemplate> updatedEntity)
        {
            PatchExamResultTemplate(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific examresulttemplate by its primary key</summary>
        /// <param name="id">The primary key of the examresulttemplate</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteExamResultTemplate(id);
            return true;
        }
        #region
        private List<ExamResultTemplate> GetExamResultTemplate(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.ExamResultTemplate.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<ExamResultTemplate>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(ExamResultTemplate), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<ExamResultTemplate, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateExamResultTemplate(ExamResultTemplate model)
        {
            _dbContext.ExamResultTemplate.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateExamResultTemplate(Guid id, ExamResultTemplate updatedEntity)
        {
            _dbContext.ExamResultTemplate.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteExamResultTemplate(Guid id)
        {
            var entityData = _dbContext.ExamResultTemplate.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.ExamResultTemplate.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchExamResultTemplate(Guid id, JsonPatchDocument<ExamResultTemplate> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.ExamResultTemplate.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.ExamResultTemplate.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}