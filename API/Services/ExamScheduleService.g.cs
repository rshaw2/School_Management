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
    /// The examscheduleService responsible for managing examschedule related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting examschedule information.
    /// </remarks>
    public interface IExamScheduleService
    {
        /// <summary>Retrieves a specific examschedule by its primary key</summary>
        /// <param name="id">The primary key of the examschedule</param>
        /// <returns>The examschedule data</returns>
        ExamSchedule GetById(Guid id);

        /// <summary>Retrieves a list of examschedules based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of examschedules</returns>
        List<ExamSchedule> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new examschedule</summary>
        /// <param name="model">The examschedule data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(ExamSchedule model);

        /// <summary>Updates a specific examschedule by its primary key</summary>
        /// <param name="id">The primary key of the examschedule</param>
        /// <param name="updatedEntity">The examschedule data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, ExamSchedule updatedEntity);

        /// <summary>Updates a specific examschedule by its primary key</summary>
        /// <param name="id">The primary key of the examschedule</param>
        /// <param name="updatedEntity">The examschedule data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<ExamSchedule> updatedEntity);

        /// <summary>Deletes a specific examschedule by its primary key</summary>
        /// <param name="id">The primary key of the examschedule</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The examscheduleService responsible for managing examschedule related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting examschedule information.
    /// </remarks>
    public class ExamScheduleService : IExamScheduleService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the ExamSchedule class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public ExamScheduleService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific examschedule by its primary key</summary>
        /// <param name="id">The primary key of the examschedule</param>
        /// <returns>The examschedule data</returns>
        public ExamSchedule GetById(Guid id)
        {
            var entityData = _dbContext.ExamSchedule.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of examschedules based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of examschedules</returns>/// <exception cref="Exception"></exception>
        public List<ExamSchedule> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetExamSchedule(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new examschedule</summary>
        /// <param name="model">The examschedule data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(ExamSchedule model)
        {
            model.Id = CreateExamSchedule(model);
            return model.Id;
        }

        /// <summary>Updates a specific examschedule by its primary key</summary>
        /// <param name="id">The primary key of the examschedule</param>
        /// <param name="updatedEntity">The examschedule data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, ExamSchedule updatedEntity)
        {
            UpdateExamSchedule(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific examschedule by its primary key</summary>
        /// <param name="id">The primary key of the examschedule</param>
        /// <param name="updatedEntity">The examschedule data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<ExamSchedule> updatedEntity)
        {
            PatchExamSchedule(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific examschedule by its primary key</summary>
        /// <param name="id">The primary key of the examschedule</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteExamSchedule(id);
            return true;
        }
        #region
        private List<ExamSchedule> GetExamSchedule(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.ExamSchedule.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<ExamSchedule>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(ExamSchedule), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<ExamSchedule, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateExamSchedule(ExamSchedule model)
        {
            _dbContext.ExamSchedule.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateExamSchedule(Guid id, ExamSchedule updatedEntity)
        {
            _dbContext.ExamSchedule.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteExamSchedule(Guid id)
        {
            var entityData = _dbContext.ExamSchedule.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.ExamSchedule.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchExamSchedule(Guid id, JsonPatchDocument<ExamSchedule> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.ExamSchedule.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.ExamSchedule.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}