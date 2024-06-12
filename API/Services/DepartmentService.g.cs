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
    /// The departmentService responsible for managing department related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting department information.
    /// </remarks>
    public interface IDepartmentService
    {
        /// <summary>Retrieves a specific department by its primary key</summary>
        /// <param name="id">The primary key of the department</param>
        /// <returns>The department data</returns>
        Department GetById(Guid id);

        /// <summary>Retrieves a list of departments based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of departments</returns>
        List<Department> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new department</summary>
        /// <param name="model">The department data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Department model);

        /// <summary>Updates a specific department by its primary key</summary>
        /// <param name="id">The primary key of the department</param>
        /// <param name="updatedEntity">The department data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Department updatedEntity);

        /// <summary>Updates a specific department by its primary key</summary>
        /// <param name="id">The primary key of the department</param>
        /// <param name="updatedEntity">The department data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Department> updatedEntity);

        /// <summary>Deletes a specific department by its primary key</summary>
        /// <param name="id">The primary key of the department</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The departmentService responsible for managing department related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting department information.
    /// </remarks>
    public class DepartmentService : IDepartmentService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Department class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public DepartmentService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific department by its primary key</summary>
        /// <param name="id">The primary key of the department</param>
        /// <returns>The department data</returns>
        public Department GetById(Guid id)
        {
            var entityData = _dbContext.Department.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of departments based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of departments</returns>/// <exception cref="Exception"></exception>
        public List<Department> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetDepartment(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new department</summary>
        /// <param name="model">The department data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Department model)
        {
            model.Id = CreateDepartment(model);
            return model.Id;
        }

        /// <summary>Updates a specific department by its primary key</summary>
        /// <param name="id">The primary key of the department</param>
        /// <param name="updatedEntity">The department data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Department updatedEntity)
        {
            UpdateDepartment(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific department by its primary key</summary>
        /// <param name="id">The primary key of the department</param>
        /// <param name="updatedEntity">The department data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Department> updatedEntity)
        {
            PatchDepartment(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific department by its primary key</summary>
        /// <param name="id">The primary key of the department</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteDepartment(id);
            return true;
        }
        #region
        private List<Department> GetDepartment(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Department.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Department>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Department), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Department, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateDepartment(Department model)
        {
            _dbContext.Department.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateDepartment(Guid id, Department updatedEntity)
        {
            _dbContext.Department.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteDepartment(Guid id)
        {
            var entityData = _dbContext.Department.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Department.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchDepartment(Guid id, JsonPatchDocument<Department> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Department.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Department.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}