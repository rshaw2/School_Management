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
    /// The searchcriteriaService responsible for managing searchcriteria related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting searchcriteria information.
    /// </remarks>
    public interface ISearchCriteriaService
    {
        /// <summary>Retrieves a specific searchcriteria by its primary key</summary>
        /// <param name="id">The primary key of the searchcriteria</param>
        /// <returns>The searchcriteria data</returns>
        SearchCriteria GetById(Guid id);

        /// <summary>Retrieves a list of searchcriterias based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of searchcriterias</returns>
        List<SearchCriteria> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new searchcriteria</summary>
        /// <param name="model">The searchcriteria data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(SearchCriteria model);

        /// <summary>Updates a specific searchcriteria by its primary key</summary>
        /// <param name="id">The primary key of the searchcriteria</param>
        /// <param name="updatedEntity">The searchcriteria data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, SearchCriteria updatedEntity);

        /// <summary>Updates a specific searchcriteria by its primary key</summary>
        /// <param name="id">The primary key of the searchcriteria</param>
        /// <param name="updatedEntity">The searchcriteria data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<SearchCriteria> updatedEntity);

        /// <summary>Deletes a specific searchcriteria by its primary key</summary>
        /// <param name="id">The primary key of the searchcriteria</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The searchcriteriaService responsible for managing searchcriteria related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting searchcriteria information.
    /// </remarks>
    public class SearchCriteriaService : ISearchCriteriaService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the SearchCriteria class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public SearchCriteriaService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific searchcriteria by its primary key</summary>
        /// <param name="id">The primary key of the searchcriteria</param>
        /// <returns>The searchcriteria data</returns>
        public SearchCriteria GetById(Guid id)
        {
            var entityData = _dbContext.SearchCriteria.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of searchcriterias based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of searchcriterias</returns>/// <exception cref="Exception"></exception>
        public List<SearchCriteria> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetSearchCriteria(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new searchcriteria</summary>
        /// <param name="model">The searchcriteria data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(SearchCriteria model)
        {
            model.Id = CreateSearchCriteria(model);
            return model.Id;
        }

        /// <summary>Updates a specific searchcriteria by its primary key</summary>
        /// <param name="id">The primary key of the searchcriteria</param>
        /// <param name="updatedEntity">The searchcriteria data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, SearchCriteria updatedEntity)
        {
            UpdateSearchCriteria(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific searchcriteria by its primary key</summary>
        /// <param name="id">The primary key of the searchcriteria</param>
        /// <param name="updatedEntity">The searchcriteria data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<SearchCriteria> updatedEntity)
        {
            PatchSearchCriteria(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific searchcriteria by its primary key</summary>
        /// <param name="id">The primary key of the searchcriteria</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteSearchCriteria(id);
            return true;
        }
        #region
        private List<SearchCriteria> GetSearchCriteria(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.SearchCriteria.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<SearchCriteria>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(SearchCriteria), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<SearchCriteria, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateSearchCriteria(SearchCriteria model)
        {
            _dbContext.SearchCriteria.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateSearchCriteria(Guid id, SearchCriteria updatedEntity)
        {
            _dbContext.SearchCriteria.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteSearchCriteria(Guid id)
        {
            var entityData = _dbContext.SearchCriteria.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.SearchCriteria.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchSearchCriteria(Guid id, JsonPatchDocument<SearchCriteria> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.SearchCriteria.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.SearchCriteria.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}