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
    /// The examsubjectService responsible for managing examsubject related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting examsubject information.
    /// </remarks>
    public interface IExamSubjectService
    {
        /// <summary>Retrieves a specific examsubject by its primary key</summary>
        /// <param name="id">The primary key of the examsubject</param>
        /// <returns>The examsubject data</returns>
        ExamSubject GetById(Guid id);

        /// <summary>Retrieves a list of examsubjects based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of examsubjects</returns>
        List<ExamSubject> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new examsubject</summary>
        /// <param name="model">The examsubject data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(ExamSubject model);

        /// <summary>Updates a specific examsubject by its primary key</summary>
        /// <param name="id">The primary key of the examsubject</param>
        /// <param name="updatedEntity">The examsubject data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, ExamSubject updatedEntity);

        /// <summary>Updates a specific examsubject by its primary key</summary>
        /// <param name="id">The primary key of the examsubject</param>
        /// <param name="updatedEntity">The examsubject data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<ExamSubject> updatedEntity);

        /// <summary>Deletes a specific examsubject by its primary key</summary>
        /// <param name="id">The primary key of the examsubject</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The examsubjectService responsible for managing examsubject related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting examsubject information.
    /// </remarks>
    public class ExamSubjectService : IExamSubjectService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the ExamSubject class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public ExamSubjectService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific examsubject by its primary key</summary>
        /// <param name="id">The primary key of the examsubject</param>
        /// <returns>The examsubject data</returns>
        public ExamSubject GetById(Guid id)
        {
            var entityData = _dbContext.ExamSubject.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of examsubjects based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of examsubjects</returns>/// <exception cref="Exception"></exception>
        public List<ExamSubject> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetExamSubject(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new examsubject</summary>
        /// <param name="model">The examsubject data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(ExamSubject model)
        {
            model.Id = CreateExamSubject(model);
            return model.Id;
        }

        /// <summary>Updates a specific examsubject by its primary key</summary>
        /// <param name="id">The primary key of the examsubject</param>
        /// <param name="updatedEntity">The examsubject data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, ExamSubject updatedEntity)
        {
            UpdateExamSubject(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific examsubject by its primary key</summary>
        /// <param name="id">The primary key of the examsubject</param>
        /// <param name="updatedEntity">The examsubject data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<ExamSubject> updatedEntity)
        {
            PatchExamSubject(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific examsubject by its primary key</summary>
        /// <param name="id">The primary key of the examsubject</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteExamSubject(id);
            return true;
        }
        #region
        private List<ExamSubject> GetExamSubject(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.ExamSubject.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<ExamSubject>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(ExamSubject), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<ExamSubject, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateExamSubject(ExamSubject model)
        {
            _dbContext.ExamSubject.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateExamSubject(Guid id, ExamSubject updatedEntity)
        {
            _dbContext.ExamSubject.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteExamSubject(Guid id)
        {
            var entityData = _dbContext.ExamSubject.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.ExamSubject.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchExamSubject(Guid id, JsonPatchDocument<ExamSubject> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.ExamSubject.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.ExamSubject.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}