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
    /// The employmentstatusService responsible for managing employmentstatus related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting employmentstatus information.
    /// </remarks>
    public interface IEmploymentStatusService
    {
        /// <summary>Retrieves a specific employmentstatus by its primary key</summary>
        /// <param name="id">The primary key of the employmentstatus</param>
        /// <returns>The employmentstatus data</returns>
        EmploymentStatus GetById(Guid id);

        /// <summary>Retrieves a list of employmentstatuss based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of employmentstatuss</returns>
        List<EmploymentStatus> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new employmentstatus</summary>
        /// <param name="model">The employmentstatus data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(EmploymentStatus model);

        /// <summary>Updates a specific employmentstatus by its primary key</summary>
        /// <param name="id">The primary key of the employmentstatus</param>
        /// <param name="updatedEntity">The employmentstatus data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, EmploymentStatus updatedEntity);

        /// <summary>Updates a specific employmentstatus by its primary key</summary>
        /// <param name="id">The primary key of the employmentstatus</param>
        /// <param name="updatedEntity">The employmentstatus data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<EmploymentStatus> updatedEntity);

        /// <summary>Deletes a specific employmentstatus by its primary key</summary>
        /// <param name="id">The primary key of the employmentstatus</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The employmentstatusService responsible for managing employmentstatus related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting employmentstatus information.
    /// </remarks>
    public class EmploymentStatusService : IEmploymentStatusService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the EmploymentStatus class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public EmploymentStatusService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific employmentstatus by its primary key</summary>
        /// <param name="id">The primary key of the employmentstatus</param>
        /// <returns>The employmentstatus data</returns>
        public EmploymentStatus GetById(Guid id)
        {
            var entityData = _dbContext.EmploymentStatus.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of employmentstatuss based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of employmentstatuss</returns>/// <exception cref="Exception"></exception>
        public List<EmploymentStatus> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetEmploymentStatus(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new employmentstatus</summary>
        /// <param name="model">The employmentstatus data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(EmploymentStatus model)
        {
            model.Id = CreateEmploymentStatus(model);
            return model.Id;
        }

        /// <summary>Updates a specific employmentstatus by its primary key</summary>
        /// <param name="id">The primary key of the employmentstatus</param>
        /// <param name="updatedEntity">The employmentstatus data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, EmploymentStatus updatedEntity)
        {
            UpdateEmploymentStatus(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific employmentstatus by its primary key</summary>
        /// <param name="id">The primary key of the employmentstatus</param>
        /// <param name="updatedEntity">The employmentstatus data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<EmploymentStatus> updatedEntity)
        {
            PatchEmploymentStatus(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific employmentstatus by its primary key</summary>
        /// <param name="id">The primary key of the employmentstatus</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteEmploymentStatus(id);
            return true;
        }
        #region
        private List<EmploymentStatus> GetEmploymentStatus(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.EmploymentStatus.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<EmploymentStatus>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(EmploymentStatus), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<EmploymentStatus, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateEmploymentStatus(EmploymentStatus model)
        {
            _dbContext.EmploymentStatus.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateEmploymentStatus(Guid id, EmploymentStatus updatedEntity)
        {
            _dbContext.EmploymentStatus.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteEmploymentStatus(Guid id)
        {
            var entityData = _dbContext.EmploymentStatus.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.EmploymentStatus.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchEmploymentStatus(Guid id, JsonPatchDocument<EmploymentStatus> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.EmploymentStatus.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.EmploymentStatus.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}