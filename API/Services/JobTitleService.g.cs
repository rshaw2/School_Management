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
    /// The jobtitleService responsible for managing jobtitle related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting jobtitle information.
    /// </remarks>
    public interface IJobTitleService
    {
        /// <summary>Retrieves a specific jobtitle by its primary key</summary>
        /// <param name="id">The primary key of the jobtitle</param>
        /// <returns>The jobtitle data</returns>
        JobTitle GetById(Guid id);

        /// <summary>Retrieves a list of jobtitles based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of jobtitles</returns>
        List<JobTitle> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new jobtitle</summary>
        /// <param name="model">The jobtitle data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(JobTitle model);

        /// <summary>Updates a specific jobtitle by its primary key</summary>
        /// <param name="id">The primary key of the jobtitle</param>
        /// <param name="updatedEntity">The jobtitle data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, JobTitle updatedEntity);

        /// <summary>Updates a specific jobtitle by its primary key</summary>
        /// <param name="id">The primary key of the jobtitle</param>
        /// <param name="updatedEntity">The jobtitle data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<JobTitle> updatedEntity);

        /// <summary>Deletes a specific jobtitle by its primary key</summary>
        /// <param name="id">The primary key of the jobtitle</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The jobtitleService responsible for managing jobtitle related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting jobtitle information.
    /// </remarks>
    public class JobTitleService : IJobTitleService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the JobTitle class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public JobTitleService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific jobtitle by its primary key</summary>
        /// <param name="id">The primary key of the jobtitle</param>
        /// <returns>The jobtitle data</returns>
        public JobTitle GetById(Guid id)
        {
            var entityData = _dbContext.JobTitle.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of jobtitles based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of jobtitles</returns>/// <exception cref="Exception"></exception>
        public List<JobTitle> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetJobTitle(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new jobtitle</summary>
        /// <param name="model">The jobtitle data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(JobTitle model)
        {
            model.Id = CreateJobTitle(model);
            return model.Id;
        }

        /// <summary>Updates a specific jobtitle by its primary key</summary>
        /// <param name="id">The primary key of the jobtitle</param>
        /// <param name="updatedEntity">The jobtitle data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, JobTitle updatedEntity)
        {
            UpdateJobTitle(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific jobtitle by its primary key</summary>
        /// <param name="id">The primary key of the jobtitle</param>
        /// <param name="updatedEntity">The jobtitle data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<JobTitle> updatedEntity)
        {
            PatchJobTitle(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific jobtitle by its primary key</summary>
        /// <param name="id">The primary key of the jobtitle</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteJobTitle(id);
            return true;
        }
        #region
        private List<JobTitle> GetJobTitle(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.JobTitle.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<JobTitle>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(JobTitle), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<JobTitle, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateJobTitle(JobTitle model)
        {
            _dbContext.JobTitle.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateJobTitle(Guid id, JobTitle updatedEntity)
        {
            _dbContext.JobTitle.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteJobTitle(Guid id)
        {
            var entityData = _dbContext.JobTitle.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.JobTitle.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchJobTitle(Guid id, JsonPatchDocument<JobTitle> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.JobTitle.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.JobTitle.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}