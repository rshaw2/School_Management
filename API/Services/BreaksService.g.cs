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
    /// The breaksService responsible for managing breaks related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting breaks information.
    /// </remarks>
    public interface IBreaksService
    {
        /// <summary>Retrieves a specific breaks by its primary key</summary>
        /// <param name="id">The primary key of the breaks</param>
        /// <returns>The breaks data</returns>
        Breaks GetById(Guid id);

        /// <summary>Retrieves a list of breakss based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of breakss</returns>
        List<Breaks> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new breaks</summary>
        /// <param name="model">The breaks data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Breaks model);

        /// <summary>Updates a specific breaks by its primary key</summary>
        /// <param name="id">The primary key of the breaks</param>
        /// <param name="updatedEntity">The breaks data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Breaks updatedEntity);

        /// <summary>Updates a specific breaks by its primary key</summary>
        /// <param name="id">The primary key of the breaks</param>
        /// <param name="updatedEntity">The breaks data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Breaks> updatedEntity);

        /// <summary>Deletes a specific breaks by its primary key</summary>
        /// <param name="id">The primary key of the breaks</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The breaksService responsible for managing breaks related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting breaks information.
    /// </remarks>
    public class BreaksService : IBreaksService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Breaks class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public BreaksService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific breaks by its primary key</summary>
        /// <param name="id">The primary key of the breaks</param>
        /// <returns>The breaks data</returns>
        public Breaks GetById(Guid id)
        {
            var entityData = _dbContext.Breaks.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of breakss based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of breakss</returns>/// <exception cref="Exception"></exception>
        public List<Breaks> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetBreaks(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new breaks</summary>
        /// <param name="model">The breaks data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Breaks model)
        {
            model.Id = CreateBreaks(model);
            return model.Id;
        }

        /// <summary>Updates a specific breaks by its primary key</summary>
        /// <param name="id">The primary key of the breaks</param>
        /// <param name="updatedEntity">The breaks data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Breaks updatedEntity)
        {
            UpdateBreaks(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific breaks by its primary key</summary>
        /// <param name="id">The primary key of the breaks</param>
        /// <param name="updatedEntity">The breaks data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Breaks> updatedEntity)
        {
            PatchBreaks(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific breaks by its primary key</summary>
        /// <param name="id">The primary key of the breaks</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteBreaks(id);
            return true;
        }
        #region
        private List<Breaks> GetBreaks(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Breaks.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Breaks>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Breaks), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Breaks, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateBreaks(Breaks model)
        {
            _dbContext.Breaks.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateBreaks(Guid id, Breaks updatedEntity)
        {
            _dbContext.Breaks.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteBreaks(Guid id)
        {
            var entityData = _dbContext.Breaks.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Breaks.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchBreaks(Guid id, JsonPatchDocument<Breaks> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Breaks.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Breaks.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}