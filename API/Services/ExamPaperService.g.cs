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
    /// The exampaperService responsible for managing exampaper related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting exampaper information.
    /// </remarks>
    public interface IExamPaperService
    {
        /// <summary>Retrieves a specific exampaper by its primary key</summary>
        /// <param name="id">The primary key of the exampaper</param>
        /// <returns>The exampaper data</returns>
        ExamPaper GetById(Guid id);

        /// <summary>Retrieves a list of exampapers based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of exampapers</returns>
        List<ExamPaper> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new exampaper</summary>
        /// <param name="model">The exampaper data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(ExamPaper model);

        /// <summary>Updates a specific exampaper by its primary key</summary>
        /// <param name="id">The primary key of the exampaper</param>
        /// <param name="updatedEntity">The exampaper data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, ExamPaper updatedEntity);

        /// <summary>Updates a specific exampaper by its primary key</summary>
        /// <param name="id">The primary key of the exampaper</param>
        /// <param name="updatedEntity">The exampaper data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<ExamPaper> updatedEntity);

        /// <summary>Deletes a specific exampaper by its primary key</summary>
        /// <param name="id">The primary key of the exampaper</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The exampaperService responsible for managing exampaper related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting exampaper information.
    /// </remarks>
    public class ExamPaperService : IExamPaperService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the ExamPaper class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public ExamPaperService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific exampaper by its primary key</summary>
        /// <param name="id">The primary key of the exampaper</param>
        /// <returns>The exampaper data</returns>
        public ExamPaper GetById(Guid id)
        {
            var entityData = _dbContext.ExamPaper.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of exampapers based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of exampapers</returns>/// <exception cref="Exception"></exception>
        public List<ExamPaper> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetExamPaper(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new exampaper</summary>
        /// <param name="model">The exampaper data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(ExamPaper model)
        {
            model.Id = CreateExamPaper(model);
            return model.Id;
        }

        /// <summary>Updates a specific exampaper by its primary key</summary>
        /// <param name="id">The primary key of the exampaper</param>
        /// <param name="updatedEntity">The exampaper data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, ExamPaper updatedEntity)
        {
            UpdateExamPaper(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific exampaper by its primary key</summary>
        /// <param name="id">The primary key of the exampaper</param>
        /// <param name="updatedEntity">The exampaper data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<ExamPaper> updatedEntity)
        {
            PatchExamPaper(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific exampaper by its primary key</summary>
        /// <param name="id">The primary key of the exampaper</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteExamPaper(id);
            return true;
        }
        #region
        private List<ExamPaper> GetExamPaper(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.ExamPaper.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<ExamPaper>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(ExamPaper), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<ExamPaper, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateExamPaper(ExamPaper model)
        {
            _dbContext.ExamPaper.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateExamPaper(Guid id, ExamPaper updatedEntity)
        {
            _dbContext.ExamPaper.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteExamPaper(Guid id)
        {
            var entityData = _dbContext.ExamPaper.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.ExamPaper.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchExamPaper(Guid id, JsonPatchDocument<ExamPaper> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.ExamPaper.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.ExamPaper.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}