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
    /// The examService responsible for managing exam related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting exam information.
    /// </remarks>
    public interface IExamService
    {
        /// <summary>Retrieves a specific exam by its primary key</summary>
        /// <param name="id">The primary key of the exam</param>
        /// <returns>The exam data</returns>
        Exam GetById(Guid id);

        /// <summary>Retrieves a list of exams based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of exams</returns>
        List<Exam> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new exam</summary>
        /// <param name="model">The exam data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Exam model);

        /// <summary>Updates a specific exam by its primary key</summary>
        /// <param name="id">The primary key of the exam</param>
        /// <param name="updatedEntity">The exam data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Exam updatedEntity);

        /// <summary>Updates a specific exam by its primary key</summary>
        /// <param name="id">The primary key of the exam</param>
        /// <param name="updatedEntity">The exam data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Exam> updatedEntity);

        /// <summary>Deletes a specific exam by its primary key</summary>
        /// <param name="id">The primary key of the exam</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The examService responsible for managing exam related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting exam information.
    /// </remarks>
    public class ExamService : IExamService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Exam class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public ExamService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific exam by its primary key</summary>
        /// <param name="id">The primary key of the exam</param>
        /// <returns>The exam data</returns>
        public Exam GetById(Guid id)
        {
            var entityData = _dbContext.Exam.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of exams based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of exams</returns>/// <exception cref="Exception"></exception>
        public List<Exam> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetExam(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new exam</summary>
        /// <param name="model">The exam data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Exam model)
        {
            model.Id = CreateExam(model);
            return model.Id;
        }

        /// <summary>Updates a specific exam by its primary key</summary>
        /// <param name="id">The primary key of the exam</param>
        /// <param name="updatedEntity">The exam data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Exam updatedEntity)
        {
            UpdateExam(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific exam by its primary key</summary>
        /// <param name="id">The primary key of the exam</param>
        /// <param name="updatedEntity">The exam data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Exam> updatedEntity)
        {
            PatchExam(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific exam by its primary key</summary>
        /// <param name="id">The primary key of the exam</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteExam(id);
            return true;
        }
        #region
        private List<Exam> GetExam(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Exam.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Exam>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Exam), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Exam, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateExam(Exam model)
        {
            _dbContext.Exam.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateExam(Guid id, Exam updatedEntity)
        {
            _dbContext.Exam.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteExam(Guid id)
        {
            var entityData = _dbContext.Exam.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Exam.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchExam(Guid id, JsonPatchDocument<Exam> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Exam.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Exam.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}