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
    /// The campusService responsible for managing campus related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting campus information.
    /// </remarks>
    public interface ICampusService
    {
        /// <summary>Retrieves a specific campus by its primary key</summary>
        /// <param name="id">The primary key of the campus</param>
        /// <returns>The campus data</returns>
        Campus GetById(Guid id);

        /// <summary>Retrieves a list of campuss based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of campuss</returns>
        List<Campus> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new campus</summary>
        /// <param name="model">The campus data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Campus model);

        /// <summary>Updates a specific campus by its primary key</summary>
        /// <param name="id">The primary key of the campus</param>
        /// <param name="updatedEntity">The campus data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Campus updatedEntity);

        /// <summary>Updates a specific campus by its primary key</summary>
        /// <param name="id">The primary key of the campus</param>
        /// <param name="updatedEntity">The campus data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Campus> updatedEntity);

        /// <summary>Deletes a specific campus by its primary key</summary>
        /// <param name="id">The primary key of the campus</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The campusService responsible for managing campus related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting campus information.
    /// </remarks>
    public class CampusService : ICampusService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Campus class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public CampusService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific campus by its primary key</summary>
        /// <param name="id">The primary key of the campus</param>
        /// <returns>The campus data</returns>
        public Campus GetById(Guid id)
        {
            var entityData = _dbContext.Campus.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of campuss based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of campuss</returns>/// <exception cref="Exception"></exception>
        public List<Campus> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetCampus(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new campus</summary>
        /// <param name="model">The campus data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Campus model)
        {
            model.Id = CreateCampus(model);
            return model.Id;
        }

        /// <summary>Updates a specific campus by its primary key</summary>
        /// <param name="id">The primary key of the campus</param>
        /// <param name="updatedEntity">The campus data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Campus updatedEntity)
        {
            UpdateCampus(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific campus by its primary key</summary>
        /// <param name="id">The primary key of the campus</param>
        /// <param name="updatedEntity">The campus data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Campus> updatedEntity)
        {
            PatchCampus(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific campus by its primary key</summary>
        /// <param name="id">The primary key of the campus</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteCampus(id);
            return true;
        }
        #region
        private List<Campus> GetCampus(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Campus.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Campus>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Campus), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Campus, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateCampus(Campus model)
        {
            _dbContext.Campus.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateCampus(Guid id, Campus updatedEntity)
        {
            _dbContext.Campus.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteCampus(Guid id)
        {
            var entityData = _dbContext.Campus.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Campus.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchCampus(Guid id, JsonPatchDocument<Campus> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Campus.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Campus.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}