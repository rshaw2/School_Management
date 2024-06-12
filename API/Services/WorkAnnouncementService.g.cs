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
    /// The workannouncementService responsible for managing workannouncement related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting workannouncement information.
    /// </remarks>
    public interface IWorkAnnouncementService
    {
        /// <summary>Retrieves a specific workannouncement by its primary key</summary>
        /// <param name="id">The primary key of the workannouncement</param>
        /// <returns>The workannouncement data</returns>
        WorkAnnouncement GetById(Guid id);

        /// <summary>Retrieves a list of workannouncements based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of workannouncements</returns>
        List<WorkAnnouncement> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new workannouncement</summary>
        /// <param name="model">The workannouncement data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(WorkAnnouncement model);

        /// <summary>Updates a specific workannouncement by its primary key</summary>
        /// <param name="id">The primary key of the workannouncement</param>
        /// <param name="updatedEntity">The workannouncement data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, WorkAnnouncement updatedEntity);

        /// <summary>Updates a specific workannouncement by its primary key</summary>
        /// <param name="id">The primary key of the workannouncement</param>
        /// <param name="updatedEntity">The workannouncement data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<WorkAnnouncement> updatedEntity);

        /// <summary>Deletes a specific workannouncement by its primary key</summary>
        /// <param name="id">The primary key of the workannouncement</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The workannouncementService responsible for managing workannouncement related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting workannouncement information.
    /// </remarks>
    public class WorkAnnouncementService : IWorkAnnouncementService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the WorkAnnouncement class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public WorkAnnouncementService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific workannouncement by its primary key</summary>
        /// <param name="id">The primary key of the workannouncement</param>
        /// <returns>The workannouncement data</returns>
        public WorkAnnouncement GetById(Guid id)
        {
            var entityData = _dbContext.WorkAnnouncement.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of workannouncements based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of workannouncements</returns>/// <exception cref="Exception"></exception>
        public List<WorkAnnouncement> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetWorkAnnouncement(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new workannouncement</summary>
        /// <param name="model">The workannouncement data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(WorkAnnouncement model)
        {
            model.Id = CreateWorkAnnouncement(model);
            return model.Id;
        }

        /// <summary>Updates a specific workannouncement by its primary key</summary>
        /// <param name="id">The primary key of the workannouncement</param>
        /// <param name="updatedEntity">The workannouncement data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, WorkAnnouncement updatedEntity)
        {
            UpdateWorkAnnouncement(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific workannouncement by its primary key</summary>
        /// <param name="id">The primary key of the workannouncement</param>
        /// <param name="updatedEntity">The workannouncement data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<WorkAnnouncement> updatedEntity)
        {
            PatchWorkAnnouncement(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific workannouncement by its primary key</summary>
        /// <param name="id">The primary key of the workannouncement</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteWorkAnnouncement(id);
            return true;
        }
        #region
        private List<WorkAnnouncement> GetWorkAnnouncement(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.WorkAnnouncement.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<WorkAnnouncement>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(WorkAnnouncement), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<WorkAnnouncement, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateWorkAnnouncement(WorkAnnouncement model)
        {
            _dbContext.WorkAnnouncement.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateWorkAnnouncement(Guid id, WorkAnnouncement updatedEntity)
        {
            _dbContext.WorkAnnouncement.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteWorkAnnouncement(Guid id)
        {
            var entityData = _dbContext.WorkAnnouncement.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.WorkAnnouncement.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchWorkAnnouncement(Guid id, JsonPatchDocument<WorkAnnouncement> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.WorkAnnouncement.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.WorkAnnouncement.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}