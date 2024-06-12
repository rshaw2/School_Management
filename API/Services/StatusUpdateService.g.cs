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
    /// The statusupdateService responsible for managing statusupdate related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting statusupdate information.
    /// </remarks>
    public interface IStatusUpdateService
    {
        /// <summary>Retrieves a specific statusupdate by its primary key</summary>
        /// <param name="id">The primary key of the statusupdate</param>
        /// <returns>The statusupdate data</returns>
        StatusUpdate GetById(Guid id);

        /// <summary>Retrieves a list of statusupdates based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of statusupdates</returns>
        List<StatusUpdate> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new statusupdate</summary>
        /// <param name="model">The statusupdate data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(StatusUpdate model);

        /// <summary>Updates a specific statusupdate by its primary key</summary>
        /// <param name="id">The primary key of the statusupdate</param>
        /// <param name="updatedEntity">The statusupdate data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, StatusUpdate updatedEntity);

        /// <summary>Updates a specific statusupdate by its primary key</summary>
        /// <param name="id">The primary key of the statusupdate</param>
        /// <param name="updatedEntity">The statusupdate data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<StatusUpdate> updatedEntity);

        /// <summary>Deletes a specific statusupdate by its primary key</summary>
        /// <param name="id">The primary key of the statusupdate</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The statusupdateService responsible for managing statusupdate related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting statusupdate information.
    /// </remarks>
    public class StatusUpdateService : IStatusUpdateService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the StatusUpdate class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public StatusUpdateService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific statusupdate by its primary key</summary>
        /// <param name="id">The primary key of the statusupdate</param>
        /// <returns>The statusupdate data</returns>
        public StatusUpdate GetById(Guid id)
        {
            var entityData = _dbContext.StatusUpdate.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of statusupdates based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of statusupdates</returns>/// <exception cref="Exception"></exception>
        public List<StatusUpdate> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetStatusUpdate(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new statusupdate</summary>
        /// <param name="model">The statusupdate data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(StatusUpdate model)
        {
            model.Id = CreateStatusUpdate(model);
            return model.Id;
        }

        /// <summary>Updates a specific statusupdate by its primary key</summary>
        /// <param name="id">The primary key of the statusupdate</param>
        /// <param name="updatedEntity">The statusupdate data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, StatusUpdate updatedEntity)
        {
            UpdateStatusUpdate(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific statusupdate by its primary key</summary>
        /// <param name="id">The primary key of the statusupdate</param>
        /// <param name="updatedEntity">The statusupdate data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<StatusUpdate> updatedEntity)
        {
            PatchStatusUpdate(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific statusupdate by its primary key</summary>
        /// <param name="id">The primary key of the statusupdate</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteStatusUpdate(id);
            return true;
        }
        #region
        private List<StatusUpdate> GetStatusUpdate(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.StatusUpdate.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<StatusUpdate>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(StatusUpdate), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<StatusUpdate, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateStatusUpdate(StatusUpdate model)
        {
            _dbContext.StatusUpdate.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateStatusUpdate(Guid id, StatusUpdate updatedEntity)
        {
            _dbContext.StatusUpdate.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteStatusUpdate(Guid id)
        {
            var entityData = _dbContext.StatusUpdate.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.StatusUpdate.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchStatusUpdate(Guid id, JsonPatchDocument<StatusUpdate> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.StatusUpdate.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.StatusUpdate.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}