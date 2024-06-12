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
    /// The evaluationcriteriaService responsible for managing evaluationcriteria related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting evaluationcriteria information.
    /// </remarks>
    public interface IEvaluationCriteriaService
    {
        /// <summary>Retrieves a specific evaluationcriteria by its primary key</summary>
        /// <param name="id">The primary key of the evaluationcriteria</param>
        /// <returns>The evaluationcriteria data</returns>
        EvaluationCriteria GetById(Guid id);

        /// <summary>Retrieves a list of evaluationcriterias based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of evaluationcriterias</returns>
        List<EvaluationCriteria> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new evaluationcriteria</summary>
        /// <param name="model">The evaluationcriteria data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(EvaluationCriteria model);

        /// <summary>Updates a specific evaluationcriteria by its primary key</summary>
        /// <param name="id">The primary key of the evaluationcriteria</param>
        /// <param name="updatedEntity">The evaluationcriteria data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, EvaluationCriteria updatedEntity);

        /// <summary>Updates a specific evaluationcriteria by its primary key</summary>
        /// <param name="id">The primary key of the evaluationcriteria</param>
        /// <param name="updatedEntity">The evaluationcriteria data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<EvaluationCriteria> updatedEntity);

        /// <summary>Deletes a specific evaluationcriteria by its primary key</summary>
        /// <param name="id">The primary key of the evaluationcriteria</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The evaluationcriteriaService responsible for managing evaluationcriteria related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting evaluationcriteria information.
    /// </remarks>
    public class EvaluationCriteriaService : IEvaluationCriteriaService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the EvaluationCriteria class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public EvaluationCriteriaService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific evaluationcriteria by its primary key</summary>
        /// <param name="id">The primary key of the evaluationcriteria</param>
        /// <returns>The evaluationcriteria data</returns>
        public EvaluationCriteria GetById(Guid id)
        {
            var entityData = _dbContext.EvaluationCriteria.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of evaluationcriterias based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of evaluationcriterias</returns>/// <exception cref="Exception"></exception>
        public List<EvaluationCriteria> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetEvaluationCriteria(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new evaluationcriteria</summary>
        /// <param name="model">The evaluationcriteria data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(EvaluationCriteria model)
        {
            model.Id = CreateEvaluationCriteria(model);
            return model.Id;
        }

        /// <summary>Updates a specific evaluationcriteria by its primary key</summary>
        /// <param name="id">The primary key of the evaluationcriteria</param>
        /// <param name="updatedEntity">The evaluationcriteria data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, EvaluationCriteria updatedEntity)
        {
            UpdateEvaluationCriteria(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific evaluationcriteria by its primary key</summary>
        /// <param name="id">The primary key of the evaluationcriteria</param>
        /// <param name="updatedEntity">The evaluationcriteria data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<EvaluationCriteria> updatedEntity)
        {
            PatchEvaluationCriteria(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific evaluationcriteria by its primary key</summary>
        /// <param name="id">The primary key of the evaluationcriteria</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteEvaluationCriteria(id);
            return true;
        }
        #region
        private List<EvaluationCriteria> GetEvaluationCriteria(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.EvaluationCriteria.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<EvaluationCriteria>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(EvaluationCriteria), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<EvaluationCriteria, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateEvaluationCriteria(EvaluationCriteria model)
        {
            _dbContext.EvaluationCriteria.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateEvaluationCriteria(Guid id, EvaluationCriteria updatedEntity)
        {
            _dbContext.EvaluationCriteria.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteEvaluationCriteria(Guid id)
        {
            var entityData = _dbContext.EvaluationCriteria.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.EvaluationCriteria.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchEvaluationCriteria(Guid id, JsonPatchDocument<EvaluationCriteria> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.EvaluationCriteria.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.EvaluationCriteria.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}