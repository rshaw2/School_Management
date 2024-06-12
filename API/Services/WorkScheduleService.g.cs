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
    /// The workscheduleService responsible for managing workschedule related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting workschedule information.
    /// </remarks>
    public interface IWorkScheduleService
    {
        /// <summary>Retrieves a specific workschedule by its primary key</summary>
        /// <param name="id">The primary key of the workschedule</param>
        /// <returns>The workschedule data</returns>
        WorkSchedule GetById(Guid id);

        /// <summary>Retrieves a list of workschedules based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of workschedules</returns>
        List<WorkSchedule> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new workschedule</summary>
        /// <param name="model">The workschedule data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(WorkSchedule model);

        /// <summary>Updates a specific workschedule by its primary key</summary>
        /// <param name="id">The primary key of the workschedule</param>
        /// <param name="updatedEntity">The workschedule data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, WorkSchedule updatedEntity);

        /// <summary>Updates a specific workschedule by its primary key</summary>
        /// <param name="id">The primary key of the workschedule</param>
        /// <param name="updatedEntity">The workschedule data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<WorkSchedule> updatedEntity);

        /// <summary>Deletes a specific workschedule by its primary key</summary>
        /// <param name="id">The primary key of the workschedule</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The workscheduleService responsible for managing workschedule related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting workschedule information.
    /// </remarks>
    public class WorkScheduleService : IWorkScheduleService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the WorkSchedule class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public WorkScheduleService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific workschedule by its primary key</summary>
        /// <param name="id">The primary key of the workschedule</param>
        /// <returns>The workschedule data</returns>
        public WorkSchedule GetById(Guid id)
        {
            var entityData = _dbContext.WorkSchedule.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of workschedules based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of workschedules</returns>/// <exception cref="Exception"></exception>
        public List<WorkSchedule> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetWorkSchedule(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new workschedule</summary>
        /// <param name="model">The workschedule data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(WorkSchedule model)
        {
            model.Id = CreateWorkSchedule(model);
            return model.Id;
        }

        /// <summary>Updates a specific workschedule by its primary key</summary>
        /// <param name="id">The primary key of the workschedule</param>
        /// <param name="updatedEntity">The workschedule data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, WorkSchedule updatedEntity)
        {
            UpdateWorkSchedule(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific workschedule by its primary key</summary>
        /// <param name="id">The primary key of the workschedule</param>
        /// <param name="updatedEntity">The workschedule data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<WorkSchedule> updatedEntity)
        {
            PatchWorkSchedule(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific workschedule by its primary key</summary>
        /// <param name="id">The primary key of the workschedule</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteWorkSchedule(id);
            return true;
        }
        #region
        private List<WorkSchedule> GetWorkSchedule(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.WorkSchedule.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<WorkSchedule>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(WorkSchedule), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<WorkSchedule, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateWorkSchedule(WorkSchedule model)
        {
            _dbContext.WorkSchedule.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateWorkSchedule(Guid id, WorkSchedule updatedEntity)
        {
            _dbContext.WorkSchedule.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteWorkSchedule(Guid id)
        {
            var entityData = _dbContext.WorkSchedule.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.WorkSchedule.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchWorkSchedule(Guid id, JsonPatchDocument<WorkSchedule> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.WorkSchedule.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.WorkSchedule.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}