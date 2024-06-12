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
    /// The shiftService responsible for managing shift related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting shift information.
    /// </remarks>
    public interface IShiftService
    {
        /// <summary>Retrieves a specific shift by its primary key</summary>
        /// <param name="id">The primary key of the shift</param>
        /// <returns>The shift data</returns>
        Shift GetById(Guid id);

        /// <summary>Retrieves a list of shifts based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of shifts</returns>
        List<Shift> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new shift</summary>
        /// <param name="model">The shift data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Shift model);

        /// <summary>Updates a specific shift by its primary key</summary>
        /// <param name="id">The primary key of the shift</param>
        /// <param name="updatedEntity">The shift data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Shift updatedEntity);

        /// <summary>Updates a specific shift by its primary key</summary>
        /// <param name="id">The primary key of the shift</param>
        /// <param name="updatedEntity">The shift data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Shift> updatedEntity);

        /// <summary>Deletes a specific shift by its primary key</summary>
        /// <param name="id">The primary key of the shift</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The shiftService responsible for managing shift related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting shift information.
    /// </remarks>
    public class ShiftService : IShiftService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Shift class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public ShiftService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific shift by its primary key</summary>
        /// <param name="id">The primary key of the shift</param>
        /// <returns>The shift data</returns>
        public Shift GetById(Guid id)
        {
            var entityData = _dbContext.Shift.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of shifts based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of shifts</returns>/// <exception cref="Exception"></exception>
        public List<Shift> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetShift(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new shift</summary>
        /// <param name="model">The shift data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Shift model)
        {
            model.Id = CreateShift(model);
            return model.Id;
        }

        /// <summary>Updates a specific shift by its primary key</summary>
        /// <param name="id">The primary key of the shift</param>
        /// <param name="updatedEntity">The shift data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Shift updatedEntity)
        {
            UpdateShift(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific shift by its primary key</summary>
        /// <param name="id">The primary key of the shift</param>
        /// <param name="updatedEntity">The shift data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Shift> updatedEntity)
        {
            PatchShift(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific shift by its primary key</summary>
        /// <param name="id">The primary key of the shift</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteShift(id);
            return true;
        }
        #region
        private List<Shift> GetShift(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Shift.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Shift>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Shift), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Shift, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateShift(Shift model)
        {
            _dbContext.Shift.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateShift(Guid id, Shift updatedEntity)
        {
            _dbContext.Shift.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteShift(Guid id)
        {
            var entityData = _dbContext.Shift.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Shift.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchShift(Guid id, JsonPatchDocument<Shift> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Shift.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Shift.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}