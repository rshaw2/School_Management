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
    /// The installmentService responsible for managing installment related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting installment information.
    /// </remarks>
    public interface IInstallmentService
    {
        /// <summary>Retrieves a specific installment by its primary key</summary>
        /// <param name="id">The primary key of the installment</param>
        /// <returns>The installment data</returns>
        Installment GetById(Guid id);

        /// <summary>Retrieves a list of installments based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of installments</returns>
        List<Installment> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new installment</summary>
        /// <param name="model">The installment data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Installment model);

        /// <summary>Updates a specific installment by its primary key</summary>
        /// <param name="id">The primary key of the installment</param>
        /// <param name="updatedEntity">The installment data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Installment updatedEntity);

        /// <summary>Updates a specific installment by its primary key</summary>
        /// <param name="id">The primary key of the installment</param>
        /// <param name="updatedEntity">The installment data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Installment> updatedEntity);

        /// <summary>Deletes a specific installment by its primary key</summary>
        /// <param name="id">The primary key of the installment</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The installmentService responsible for managing installment related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting installment information.
    /// </remarks>
    public class InstallmentService : IInstallmentService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Installment class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public InstallmentService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific installment by its primary key</summary>
        /// <param name="id">The primary key of the installment</param>
        /// <returns>The installment data</returns>
        public Installment GetById(Guid id)
        {
            var entityData = _dbContext.Installment.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of installments based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of installments</returns>/// <exception cref="Exception"></exception>
        public List<Installment> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetInstallment(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new installment</summary>
        /// <param name="model">The installment data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Installment model)
        {
            model.Id = CreateInstallment(model);
            return model.Id;
        }

        /// <summary>Updates a specific installment by its primary key</summary>
        /// <param name="id">The primary key of the installment</param>
        /// <param name="updatedEntity">The installment data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Installment updatedEntity)
        {
            UpdateInstallment(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific installment by its primary key</summary>
        /// <param name="id">The primary key of the installment</param>
        /// <param name="updatedEntity">The installment data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Installment> updatedEntity)
        {
            PatchInstallment(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific installment by its primary key</summary>
        /// <param name="id">The primary key of the installment</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteInstallment(id);
            return true;
        }
        #region
        private List<Installment> GetInstallment(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Installment.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Installment>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Installment), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Installment, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateInstallment(Installment model)
        {
            _dbContext.Installment.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateInstallment(Guid id, Installment updatedEntity)
        {
            _dbContext.Installment.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteInstallment(Guid id)
        {
            var entityData = _dbContext.Installment.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Installment.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchInstallment(Guid id, JsonPatchDocument<Installment> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Installment.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Installment.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}