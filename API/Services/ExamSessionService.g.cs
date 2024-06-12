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
    /// The examsessionService responsible for managing examsession related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting examsession information.
    /// </remarks>
    public interface IExamSessionService
    {
        /// <summary>Retrieves a specific examsession by its primary key</summary>
        /// <param name="id">The primary key of the examsession</param>
        /// <returns>The examsession data</returns>
        ExamSession GetById(Guid id);

        /// <summary>Retrieves a list of examsessions based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of examsessions</returns>
        List<ExamSession> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new examsession</summary>
        /// <param name="model">The examsession data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(ExamSession model);

        /// <summary>Updates a specific examsession by its primary key</summary>
        /// <param name="id">The primary key of the examsession</param>
        /// <param name="updatedEntity">The examsession data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, ExamSession updatedEntity);

        /// <summary>Updates a specific examsession by its primary key</summary>
        /// <param name="id">The primary key of the examsession</param>
        /// <param name="updatedEntity">The examsession data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<ExamSession> updatedEntity);

        /// <summary>Deletes a specific examsession by its primary key</summary>
        /// <param name="id">The primary key of the examsession</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The examsessionService responsible for managing examsession related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting examsession information.
    /// </remarks>
    public class ExamSessionService : IExamSessionService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the ExamSession class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public ExamSessionService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific examsession by its primary key</summary>
        /// <param name="id">The primary key of the examsession</param>
        /// <returns>The examsession data</returns>
        public ExamSession GetById(Guid id)
        {
            var entityData = _dbContext.ExamSession.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of examsessions based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of examsessions</returns>/// <exception cref="Exception"></exception>
        public List<ExamSession> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetExamSession(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new examsession</summary>
        /// <param name="model">The examsession data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(ExamSession model)
        {
            model.Id = CreateExamSession(model);
            return model.Id;
        }

        /// <summary>Updates a specific examsession by its primary key</summary>
        /// <param name="id">The primary key of the examsession</param>
        /// <param name="updatedEntity">The examsession data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, ExamSession updatedEntity)
        {
            UpdateExamSession(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific examsession by its primary key</summary>
        /// <param name="id">The primary key of the examsession</param>
        /// <param name="updatedEntity">The examsession data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<ExamSession> updatedEntity)
        {
            PatchExamSession(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific examsession by its primary key</summary>
        /// <param name="id">The primary key of the examsession</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteExamSession(id);
            return true;
        }
        #region
        private List<ExamSession> GetExamSession(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.ExamSession.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<ExamSession>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(ExamSession), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<ExamSession, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateExamSession(ExamSession model)
        {
            _dbContext.ExamSession.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateExamSession(Guid id, ExamSession updatedEntity)
        {
            _dbContext.ExamSession.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteExamSession(Guid id)
        {
            var entityData = _dbContext.ExamSession.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.ExamSession.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchExamSession(Guid id, JsonPatchDocument<ExamSession> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.ExamSession.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.ExamSession.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}