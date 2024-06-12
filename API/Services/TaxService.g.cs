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
    /// The taxService responsible for managing tax related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting tax information.
    /// </remarks>
    public interface ITaxService
    {
        /// <summary>Retrieves a specific tax by its primary key</summary>
        /// <param name="id">The primary key of the tax</param>
        /// <returns>The tax data</returns>
        Tax GetById(Guid id);

        /// <summary>Retrieves a list of taxs based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of taxs</returns>
        List<Tax> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new tax</summary>
        /// <param name="model">The tax data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Tax model);

        /// <summary>Updates a specific tax by its primary key</summary>
        /// <param name="id">The primary key of the tax</param>
        /// <param name="updatedEntity">The tax data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Tax updatedEntity);

        /// <summary>Updates a specific tax by its primary key</summary>
        /// <param name="id">The primary key of the tax</param>
        /// <param name="updatedEntity">The tax data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Tax> updatedEntity);

        /// <summary>Deletes a specific tax by its primary key</summary>
        /// <param name="id">The primary key of the tax</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The taxService responsible for managing tax related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting tax information.
    /// </remarks>
    public class TaxService : ITaxService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Tax class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public TaxService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific tax by its primary key</summary>
        /// <param name="id">The primary key of the tax</param>
        /// <returns>The tax data</returns>
        public Tax GetById(Guid id)
        {
            var entityData = _dbContext.Tax.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of taxs based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of taxs</returns>/// <exception cref="Exception"></exception>
        public List<Tax> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetTax(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new tax</summary>
        /// <param name="model">The tax data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Tax model)
        {
            model.Id = CreateTax(model);
            return model.Id;
        }

        /// <summary>Updates a specific tax by its primary key</summary>
        /// <param name="id">The primary key of the tax</param>
        /// <param name="updatedEntity">The tax data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Tax updatedEntity)
        {
            UpdateTax(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific tax by its primary key</summary>
        /// <param name="id">The primary key of the tax</param>
        /// <param name="updatedEntity">The tax data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Tax> updatedEntity)
        {
            PatchTax(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific tax by its primary key</summary>
        /// <param name="id">The primary key of the tax</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteTax(id);
            return true;
        }
        #region
        private List<Tax> GetTax(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Tax.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Tax>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Tax), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Tax, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateTax(Tax model)
        {
            _dbContext.Tax.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateTax(Guid id, Tax updatedEntity)
        {
            _dbContext.Tax.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteTax(Guid id)
        {
            var entityData = _dbContext.Tax.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Tax.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchTax(Guid id, JsonPatchDocument<Tax> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Tax.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Tax.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}