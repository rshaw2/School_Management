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
    /// The gradingscaleService responsible for managing gradingscale related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting gradingscale information.
    /// </remarks>
    public interface IGradingScaleService
    {
        /// <summary>Retrieves a specific gradingscale by its primary key</summary>
        /// <param name="id">The primary key of the gradingscale</param>
        /// <returns>The gradingscale data</returns>
        GradingScale GetById(Guid id);

        /// <summary>Retrieves a list of gradingscales based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of gradingscales</returns>
        List<GradingScale> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new gradingscale</summary>
        /// <param name="model">The gradingscale data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(GradingScale model);

        /// <summary>Updates a specific gradingscale by its primary key</summary>
        /// <param name="id">The primary key of the gradingscale</param>
        /// <param name="updatedEntity">The gradingscale data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, GradingScale updatedEntity);

        /// <summary>Updates a specific gradingscale by its primary key</summary>
        /// <param name="id">The primary key of the gradingscale</param>
        /// <param name="updatedEntity">The gradingscale data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<GradingScale> updatedEntity);

        /// <summary>Deletes a specific gradingscale by its primary key</summary>
        /// <param name="id">The primary key of the gradingscale</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The gradingscaleService responsible for managing gradingscale related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting gradingscale information.
    /// </remarks>
    public class GradingScaleService : IGradingScaleService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the GradingScale class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public GradingScaleService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific gradingscale by its primary key</summary>
        /// <param name="id">The primary key of the gradingscale</param>
        /// <returns>The gradingscale data</returns>
        public GradingScale GetById(Guid id)
        {
            var entityData = _dbContext.GradingScale.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of gradingscales based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of gradingscales</returns>/// <exception cref="Exception"></exception>
        public List<GradingScale> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetGradingScale(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new gradingscale</summary>
        /// <param name="model">The gradingscale data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(GradingScale model)
        {
            model.Id = CreateGradingScale(model);
            return model.Id;
        }

        /// <summary>Updates a specific gradingscale by its primary key</summary>
        /// <param name="id">The primary key of the gradingscale</param>
        /// <param name="updatedEntity">The gradingscale data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, GradingScale updatedEntity)
        {
            UpdateGradingScale(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific gradingscale by its primary key</summary>
        /// <param name="id">The primary key of the gradingscale</param>
        /// <param name="updatedEntity">The gradingscale data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<GradingScale> updatedEntity)
        {
            PatchGradingScale(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific gradingscale by its primary key</summary>
        /// <param name="id">The primary key of the gradingscale</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteGradingScale(id);
            return true;
        }
        #region
        private List<GradingScale> GetGradingScale(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.GradingScale.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<GradingScale>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(GradingScale), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<GradingScale, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateGradingScale(GradingScale model)
        {
            _dbContext.GradingScale.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateGradingScale(Guid id, GradingScale updatedEntity)
        {
            _dbContext.GradingScale.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteGradingScale(Guid id)
        {
            var entityData = _dbContext.GradingScale.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.GradingScale.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchGradingScale(Guid id, JsonPatchDocument<GradingScale> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.GradingScale.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.GradingScale.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}