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
    /// The examresultnotificationService responsible for managing examresultnotification related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting examresultnotification information.
    /// </remarks>
    public interface IExamResultNotificationService
    {
        /// <summary>Retrieves a specific examresultnotification by its primary key</summary>
        /// <param name="id">The primary key of the examresultnotification</param>
        /// <returns>The examresultnotification data</returns>
        ExamResultNotification GetById(Guid id);

        /// <summary>Retrieves a list of examresultnotifications based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of examresultnotifications</returns>
        List<ExamResultNotification> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new examresultnotification</summary>
        /// <param name="model">The examresultnotification data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(ExamResultNotification model);

        /// <summary>Updates a specific examresultnotification by its primary key</summary>
        /// <param name="id">The primary key of the examresultnotification</param>
        /// <param name="updatedEntity">The examresultnotification data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, ExamResultNotification updatedEntity);

        /// <summary>Updates a specific examresultnotification by its primary key</summary>
        /// <param name="id">The primary key of the examresultnotification</param>
        /// <param name="updatedEntity">The examresultnotification data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<ExamResultNotification> updatedEntity);

        /// <summary>Deletes a specific examresultnotification by its primary key</summary>
        /// <param name="id">The primary key of the examresultnotification</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The examresultnotificationService responsible for managing examresultnotification related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting examresultnotification information.
    /// </remarks>
    public class ExamResultNotificationService : IExamResultNotificationService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the ExamResultNotification class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public ExamResultNotificationService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific examresultnotification by its primary key</summary>
        /// <param name="id">The primary key of the examresultnotification</param>
        /// <returns>The examresultnotification data</returns>
        public ExamResultNotification GetById(Guid id)
        {
            var entityData = _dbContext.ExamResultNotification.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of examresultnotifications based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of examresultnotifications</returns>/// <exception cref="Exception"></exception>
        public List<ExamResultNotification> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetExamResultNotification(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new examresultnotification</summary>
        /// <param name="model">The examresultnotification data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(ExamResultNotification model)
        {
            model.Id = CreateExamResultNotification(model);
            return model.Id;
        }

        /// <summary>Updates a specific examresultnotification by its primary key</summary>
        /// <param name="id">The primary key of the examresultnotification</param>
        /// <param name="updatedEntity">The examresultnotification data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, ExamResultNotification updatedEntity)
        {
            UpdateExamResultNotification(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific examresultnotification by its primary key</summary>
        /// <param name="id">The primary key of the examresultnotification</param>
        /// <param name="updatedEntity">The examresultnotification data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<ExamResultNotification> updatedEntity)
        {
            PatchExamResultNotification(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific examresultnotification by its primary key</summary>
        /// <param name="id">The primary key of the examresultnotification</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteExamResultNotification(id);
            return true;
        }
        #region
        private List<ExamResultNotification> GetExamResultNotification(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.ExamResultNotification.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<ExamResultNotification>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(ExamResultNotification), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<ExamResultNotification, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateExamResultNotification(ExamResultNotification model)
        {
            _dbContext.ExamResultNotification.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateExamResultNotification(Guid id, ExamResultNotification updatedEntity)
        {
            _dbContext.ExamResultNotification.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteExamResultNotification(Guid id)
        {
            var entityData = _dbContext.ExamResultNotification.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.ExamResultNotification.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchExamResultNotification(Guid id, JsonPatchDocument<ExamResultNotification> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.ExamResultNotification.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.ExamResultNotification.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}