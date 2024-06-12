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
    /// The employeeService responsible for managing employee related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting employee information.
    /// </remarks>
    public interface IEmployeeService
    {
        /// <summary>Retrieves a specific employee by its primary key</summary>
        /// <param name="id">The primary key of the employee</param>
        /// <returns>The employee data</returns>
        Employee GetById(Guid id);

        /// <summary>Retrieves a list of employees based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of employees</returns>
        List<Employee> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new employee</summary>
        /// <param name="model">The employee data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Employee model);

        /// <summary>Updates a specific employee by its primary key</summary>
        /// <param name="id">The primary key of the employee</param>
        /// <param name="updatedEntity">The employee data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Employee updatedEntity);

        /// <summary>Updates a specific employee by its primary key</summary>
        /// <param name="id">The primary key of the employee</param>
        /// <param name="updatedEntity">The employee data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Employee> updatedEntity);

        /// <summary>Deletes a specific employee by its primary key</summary>
        /// <param name="id">The primary key of the employee</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The employeeService responsible for managing employee related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting employee information.
    /// </remarks>
    public class EmployeeService : IEmployeeService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Employee class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public EmployeeService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific employee by its primary key</summary>
        /// <param name="id">The primary key of the employee</param>
        /// <returns>The employee data</returns>
        public Employee GetById(Guid id)
        {
            var entityData = _dbContext.Employee.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of employees based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of employees</returns>/// <exception cref="Exception"></exception>
        public List<Employee> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetEmployee(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new employee</summary>
        /// <param name="model">The employee data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Employee model)
        {
            model.Id = CreateEmployee(model);
            return model.Id;
        }

        /// <summary>Updates a specific employee by its primary key</summary>
        /// <param name="id">The primary key of the employee</param>
        /// <param name="updatedEntity">The employee data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Employee updatedEntity)
        {
            UpdateEmployee(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific employee by its primary key</summary>
        /// <param name="id">The primary key of the employee</param>
        /// <param name="updatedEntity">The employee data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Employee> updatedEntity)
        {
            PatchEmployee(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific employee by its primary key</summary>
        /// <param name="id">The primary key of the employee</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteEmployee(id);
            return true;
        }
        #region
        private List<Employee> GetEmployee(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Employee.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Employee>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Employee), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Employee, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateEmployee(Employee model)
        {
            _dbContext.Employee.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateEmployee(Guid id, Employee updatedEntity)
        {
            _dbContext.Employee.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteEmployee(Guid id)
        {
            var entityData = _dbContext.Employee.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Employee.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchEmployee(Guid id, JsonPatchDocument<Employee> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Employee.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Employee.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}