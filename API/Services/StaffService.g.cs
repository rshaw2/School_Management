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
    /// The staffService responsible for managing staff related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting staff information.
    /// </remarks>
    public interface IStaffService
    {
        /// <summary>Retrieves a specific staff by its primary key</summary>
        /// <param name="id">The primary key of the staff</param>
        /// <returns>The staff data</returns>
        Staff GetById(Guid id);

        /// <summary>Retrieves a list of staffs based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of staffs</returns>
        List<Staff> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new staff</summary>
        /// <param name="model">The staff data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Staff model);

        /// <summary>Updates a specific staff by its primary key</summary>
        /// <param name="id">The primary key of the staff</param>
        /// <param name="updatedEntity">The staff data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Staff updatedEntity);

        /// <summary>Updates a specific staff by its primary key</summary>
        /// <param name="id">The primary key of the staff</param>
        /// <param name="updatedEntity">The staff data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Staff> updatedEntity);

        /// <summary>Deletes a specific staff by its primary key</summary>
        /// <param name="id">The primary key of the staff</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The staffService responsible for managing staff related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting staff information.
    /// </remarks>
    public class StaffService : IStaffService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Staff class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public StaffService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific staff by its primary key</summary>
        /// <param name="id">The primary key of the staff</param>
        /// <returns>The staff data</returns>
        public Staff GetById(Guid id)
        {
            var entityData = _dbContext.Staff.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of staffs based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of staffs</returns>/// <exception cref="Exception"></exception>
        public List<Staff> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetStaff(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new staff</summary>
        /// <param name="model">The staff data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Staff model)
        {
            model.Id = CreateStaff(model);
            return model.Id;
        }

        /// <summary>Updates a specific staff by its primary key</summary>
        /// <param name="id">The primary key of the staff</param>
        /// <param name="updatedEntity">The staff data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Staff updatedEntity)
        {
            UpdateStaff(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific staff by its primary key</summary>
        /// <param name="id">The primary key of the staff</param>
        /// <param name="updatedEntity">The staff data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Staff> updatedEntity)
        {
            PatchStaff(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific staff by its primary key</summary>
        /// <param name="id">The primary key of the staff</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteStaff(id);
            return true;
        }
        #region
        private List<Staff> GetStaff(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Staff.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Staff>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Staff), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Staff, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateStaff(Staff model)
        {
            _dbContext.Staff.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateStaff(Guid id, Staff updatedEntity)
        {
            _dbContext.Staff.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteStaff(Guid id)
        {
            var entityData = _dbContext.Staff.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Staff.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchStaff(Guid id, JsonPatchDocument<Staff> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Staff.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Staff.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}