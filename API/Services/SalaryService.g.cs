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
    /// The salaryService responsible for managing salary related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting salary information.
    /// </remarks>
    public interface ISalaryService
    {
        /// <summary>Retrieves a specific salary by its primary key</summary>
        /// <param name="id">The primary key of the salary</param>
        /// <returns>The salary data</returns>
        Salary GetById(Guid id);

        /// <summary>Retrieves a list of salarys based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of salarys</returns>
        List<Salary> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new salary</summary>
        /// <param name="model">The salary data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Salary model);

        /// <summary>Updates a specific salary by its primary key</summary>
        /// <param name="id">The primary key of the salary</param>
        /// <param name="updatedEntity">The salary data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Salary updatedEntity);

        /// <summary>Updates a specific salary by its primary key</summary>
        /// <param name="id">The primary key of the salary</param>
        /// <param name="updatedEntity">The salary data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Salary> updatedEntity);

        /// <summary>Deletes a specific salary by its primary key</summary>
        /// <param name="id">The primary key of the salary</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The salaryService responsible for managing salary related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting salary information.
    /// </remarks>
    public class SalaryService : ISalaryService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Salary class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public SalaryService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific salary by its primary key</summary>
        /// <param name="id">The primary key of the salary</param>
        /// <returns>The salary data</returns>
        public Salary GetById(Guid id)
        {
            var entityData = _dbContext.Salary.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of salarys based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of salarys</returns>/// <exception cref="Exception"></exception>
        public List<Salary> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetSalary(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new salary</summary>
        /// <param name="model">The salary data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Salary model)
        {
            model.Id = CreateSalary(model);
            return model.Id;
        }

        /// <summary>Updates a specific salary by its primary key</summary>
        /// <param name="id">The primary key of the salary</param>
        /// <param name="updatedEntity">The salary data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Salary updatedEntity)
        {
            UpdateSalary(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific salary by its primary key</summary>
        /// <param name="id">The primary key of the salary</param>
        /// <param name="updatedEntity">The salary data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Salary> updatedEntity)
        {
            PatchSalary(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific salary by its primary key</summary>
        /// <param name="id">The primary key of the salary</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteSalary(id);
            return true;
        }
        #region
        private List<Salary> GetSalary(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Salary.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Salary>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Salary), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Salary, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateSalary(Salary model)
        {
            _dbContext.Salary.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateSalary(Guid id, Salary updatedEntity)
        {
            _dbContext.Salary.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteSalary(Guid id)
        {
            var entityData = _dbContext.Salary.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Salary.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchSalary(Guid id, JsonPatchDocument<Salary> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Salary.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Salary.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}