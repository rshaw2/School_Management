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
    /// The resultdetailsService responsible for managing resultdetails related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting resultdetails information.
    /// </remarks>
    public interface IResultDetailsService
    {
        /// <summary>Retrieves a specific resultdetails by its primary key</summary>
        /// <param name="id">The primary key of the resultdetails</param>
        /// <returns>The resultdetails data</returns>
        ResultDetails GetById(Guid id);

        /// <summary>Retrieves a list of resultdetailss based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of resultdetailss</returns>
        List<ResultDetails> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new resultdetails</summary>
        /// <param name="model">The resultdetails data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(ResultDetails model);

        /// <summary>Updates a specific resultdetails by its primary key</summary>
        /// <param name="id">The primary key of the resultdetails</param>
        /// <param name="updatedEntity">The resultdetails data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, ResultDetails updatedEntity);

        /// <summary>Updates a specific resultdetails by its primary key</summary>
        /// <param name="id">The primary key of the resultdetails</param>
        /// <param name="updatedEntity">The resultdetails data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<ResultDetails> updatedEntity);

        /// <summary>Deletes a specific resultdetails by its primary key</summary>
        /// <param name="id">The primary key of the resultdetails</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The resultdetailsService responsible for managing resultdetails related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting resultdetails information.
    /// </remarks>
    public class ResultDetailsService : IResultDetailsService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the ResultDetails class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public ResultDetailsService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific resultdetails by its primary key</summary>
        /// <param name="id">The primary key of the resultdetails</param>
        /// <returns>The resultdetails data</returns>
        public ResultDetails GetById(Guid id)
        {
            var entityData = _dbContext.ResultDetails.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of resultdetailss based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of resultdetailss</returns>/// <exception cref="Exception"></exception>
        public List<ResultDetails> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetResultDetails(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new resultdetails</summary>
        /// <param name="model">The resultdetails data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(ResultDetails model)
        {
            model.Id = CreateResultDetails(model);
            return model.Id;
        }

        /// <summary>Updates a specific resultdetails by its primary key</summary>
        /// <param name="id">The primary key of the resultdetails</param>
        /// <param name="updatedEntity">The resultdetails data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, ResultDetails updatedEntity)
        {
            UpdateResultDetails(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific resultdetails by its primary key</summary>
        /// <param name="id">The primary key of the resultdetails</param>
        /// <param name="updatedEntity">The resultdetails data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<ResultDetails> updatedEntity)
        {
            PatchResultDetails(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific resultdetails by its primary key</summary>
        /// <param name="id">The primary key of the resultdetails</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteResultDetails(id);
            return true;
        }
        #region
        private List<ResultDetails> GetResultDetails(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.ResultDetails.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<ResultDetails>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(ResultDetails), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<ResultDetails, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateResultDetails(ResultDetails model)
        {
            _dbContext.ResultDetails.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateResultDetails(Guid id, ResultDetails updatedEntity)
        {
            _dbContext.ResultDetails.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteResultDetails(Guid id)
        {
            var entityData = _dbContext.ResultDetails.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.ResultDetails.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchResultDetails(Guid id, JsonPatchDocument<ResultDetails> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.ResultDetails.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.ResultDetails.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}