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
    /// The benefitsService responsible for managing benefits related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting benefits information.
    /// </remarks>
    public interface IBenefitsService
    {
        /// <summary>Retrieves a specific benefits by its primary key</summary>
        /// <param name="id">The primary key of the benefits</param>
        /// <returns>The benefits data</returns>
        Benefits GetById(Guid id);

        /// <summary>Retrieves a list of benefitss based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of benefitss</returns>
        List<Benefits> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new benefits</summary>
        /// <param name="model">The benefits data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Benefits model);

        /// <summary>Updates a specific benefits by its primary key</summary>
        /// <param name="id">The primary key of the benefits</param>
        /// <param name="updatedEntity">The benefits data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Benefits updatedEntity);

        /// <summary>Updates a specific benefits by its primary key</summary>
        /// <param name="id">The primary key of the benefits</param>
        /// <param name="updatedEntity">The benefits data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Benefits> updatedEntity);

        /// <summary>Deletes a specific benefits by its primary key</summary>
        /// <param name="id">The primary key of the benefits</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The benefitsService responsible for managing benefits related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting benefits information.
    /// </remarks>
    public class BenefitsService : IBenefitsService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Benefits class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public BenefitsService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific benefits by its primary key</summary>
        /// <param name="id">The primary key of the benefits</param>
        /// <returns>The benefits data</returns>
        public Benefits GetById(Guid id)
        {
            var entityData = _dbContext.Benefits.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of benefitss based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of benefitss</returns>/// <exception cref="Exception"></exception>
        public List<Benefits> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetBenefits(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new benefits</summary>
        /// <param name="model">The benefits data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Benefits model)
        {
            model.Id = CreateBenefits(model);
            return model.Id;
        }

        /// <summary>Updates a specific benefits by its primary key</summary>
        /// <param name="id">The primary key of the benefits</param>
        /// <param name="updatedEntity">The benefits data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Benefits updatedEntity)
        {
            UpdateBenefits(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific benefits by its primary key</summary>
        /// <param name="id">The primary key of the benefits</param>
        /// <param name="updatedEntity">The benefits data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Benefits> updatedEntity)
        {
            PatchBenefits(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific benefits by its primary key</summary>
        /// <param name="id">The primary key of the benefits</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteBenefits(id);
            return true;
        }
        #region
        private List<Benefits> GetBenefits(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Benefits.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Benefits>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Benefits), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Benefits, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateBenefits(Benefits model)
        {
            _dbContext.Benefits.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateBenefits(Guid id, Benefits updatedEntity)
        {
            _dbContext.Benefits.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteBenefits(Guid id)
        {
            var entityData = _dbContext.Benefits.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Benefits.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchBenefits(Guid id, JsonPatchDocument<Benefits> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Benefits.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Benefits.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}