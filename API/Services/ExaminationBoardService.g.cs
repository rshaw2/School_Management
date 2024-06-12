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
    /// The examinationboardService responsible for managing examinationboard related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting examinationboard information.
    /// </remarks>
    public interface IExaminationBoardService
    {
        /// <summary>Retrieves a specific examinationboard by its primary key</summary>
        /// <param name="id">The primary key of the examinationboard</param>
        /// <returns>The examinationboard data</returns>
        ExaminationBoard GetById(Guid id);

        /// <summary>Retrieves a list of examinationboards based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of examinationboards</returns>
        List<ExaminationBoard> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new examinationboard</summary>
        /// <param name="model">The examinationboard data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(ExaminationBoard model);

        /// <summary>Updates a specific examinationboard by its primary key</summary>
        /// <param name="id">The primary key of the examinationboard</param>
        /// <param name="updatedEntity">The examinationboard data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, ExaminationBoard updatedEntity);

        /// <summary>Updates a specific examinationboard by its primary key</summary>
        /// <param name="id">The primary key of the examinationboard</param>
        /// <param name="updatedEntity">The examinationboard data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<ExaminationBoard> updatedEntity);

        /// <summary>Deletes a specific examinationboard by its primary key</summary>
        /// <param name="id">The primary key of the examinationboard</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The examinationboardService responsible for managing examinationboard related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting examinationboard information.
    /// </remarks>
    public class ExaminationBoardService : IExaminationBoardService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the ExaminationBoard class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public ExaminationBoardService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific examinationboard by its primary key</summary>
        /// <param name="id">The primary key of the examinationboard</param>
        /// <returns>The examinationboard data</returns>
        public ExaminationBoard GetById(Guid id)
        {
            var entityData = _dbContext.ExaminationBoard.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of examinationboards based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of examinationboards</returns>/// <exception cref="Exception"></exception>
        public List<ExaminationBoard> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetExaminationBoard(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new examinationboard</summary>
        /// <param name="model">The examinationboard data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(ExaminationBoard model)
        {
            model.Id = CreateExaminationBoard(model);
            return model.Id;
        }

        /// <summary>Updates a specific examinationboard by its primary key</summary>
        /// <param name="id">The primary key of the examinationboard</param>
        /// <param name="updatedEntity">The examinationboard data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, ExaminationBoard updatedEntity)
        {
            UpdateExaminationBoard(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific examinationboard by its primary key</summary>
        /// <param name="id">The primary key of the examinationboard</param>
        /// <param name="updatedEntity">The examinationboard data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<ExaminationBoard> updatedEntity)
        {
            PatchExaminationBoard(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific examinationboard by its primary key</summary>
        /// <param name="id">The primary key of the examinationboard</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteExaminationBoard(id);
            return true;
        }
        #region
        private List<ExaminationBoard> GetExaminationBoard(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.ExaminationBoard.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<ExaminationBoard>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(ExaminationBoard), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<ExaminationBoard, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateExaminationBoard(ExaminationBoard model)
        {
            _dbContext.ExaminationBoard.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateExaminationBoard(Guid id, ExaminationBoard updatedEntity)
        {
            _dbContext.ExaminationBoard.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteExaminationBoard(Guid id)
        {
            var entityData = _dbContext.ExaminationBoard.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.ExaminationBoard.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchExaminationBoard(Guid id, JsonPatchDocument<ExaminationBoard> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.ExaminationBoard.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.ExaminationBoard.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}