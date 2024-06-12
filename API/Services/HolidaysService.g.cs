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
    /// The holidaysService responsible for managing holidays related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting holidays information.
    /// </remarks>
    public interface IHolidaysService
    {
        /// <summary>Retrieves a specific holidays by its primary key</summary>
        /// <param name="id">The primary key of the holidays</param>
        /// <returns>The holidays data</returns>
        Holidays GetById(Guid id);

        /// <summary>Retrieves a list of holidayss based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of holidayss</returns>
        List<Holidays> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new holidays</summary>
        /// <param name="model">The holidays data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Holidays model);

        /// <summary>Updates a specific holidays by its primary key</summary>
        /// <param name="id">The primary key of the holidays</param>
        /// <param name="updatedEntity">The holidays data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Holidays updatedEntity);

        /// <summary>Updates a specific holidays by its primary key</summary>
        /// <param name="id">The primary key of the holidays</param>
        /// <param name="updatedEntity">The holidays data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Holidays> updatedEntity);

        /// <summary>Deletes a specific holidays by its primary key</summary>
        /// <param name="id">The primary key of the holidays</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The holidaysService responsible for managing holidays related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting holidays information.
    /// </remarks>
    public class HolidaysService : IHolidaysService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Holidays class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public HolidaysService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific holidays by its primary key</summary>
        /// <param name="id">The primary key of the holidays</param>
        /// <returns>The holidays data</returns>
        public Holidays GetById(Guid id)
        {
            var entityData = _dbContext.Holidays.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of holidayss based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of holidayss</returns>/// <exception cref="Exception"></exception>
        public List<Holidays> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetHolidays(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new holidays</summary>
        /// <param name="model">The holidays data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Holidays model)
        {
            model.Id = CreateHolidays(model);
            return model.Id;
        }

        /// <summary>Updates a specific holidays by its primary key</summary>
        /// <param name="id">The primary key of the holidays</param>
        /// <param name="updatedEntity">The holidays data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Holidays updatedEntity)
        {
            UpdateHolidays(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific holidays by its primary key</summary>
        /// <param name="id">The primary key of the holidays</param>
        /// <param name="updatedEntity">The holidays data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Holidays> updatedEntity)
        {
            PatchHolidays(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific holidays by its primary key</summary>
        /// <param name="id">The primary key of the holidays</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteHolidays(id);
            return true;
        }
        #region
        private List<Holidays> GetHolidays(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Holidays.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Holidays>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Holidays), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Holidays, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateHolidays(Holidays model)
        {
            _dbContext.Holidays.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateHolidays(Guid id, Holidays updatedEntity)
        {
            _dbContext.Holidays.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteHolidays(Guid id)
        {
            var entityData = _dbContext.Holidays.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Holidays.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchHolidays(Guid id, JsonPatchDocument<Holidays> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Holidays.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Holidays.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}