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
    /// The workflowstepService responsible for managing workflowstep related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting workflowstep information.
    /// </remarks>
    public interface IWorkflowStepService
    {
        /// <summary>Retrieves a specific workflowstep by its primary key</summary>
        /// <param name="id">The primary key of the workflowstep</param>
        /// <returns>The workflowstep data</returns>
        WorkflowStep GetById(Guid id);

        /// <summary>Retrieves a list of workflowsteps based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of workflowsteps</returns>
        List<WorkflowStep> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new workflowstep</summary>
        /// <param name="model">The workflowstep data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(WorkflowStep model);

        /// <summary>Updates a specific workflowstep by its primary key</summary>
        /// <param name="id">The primary key of the workflowstep</param>
        /// <param name="updatedEntity">The workflowstep data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, WorkflowStep updatedEntity);

        /// <summary>Updates a specific workflowstep by its primary key</summary>
        /// <param name="id">The primary key of the workflowstep</param>
        /// <param name="updatedEntity">The workflowstep data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<WorkflowStep> updatedEntity);

        /// <summary>Deletes a specific workflowstep by its primary key</summary>
        /// <param name="id">The primary key of the workflowstep</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The workflowstepService responsible for managing workflowstep related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting workflowstep information.
    /// </remarks>
    public class WorkflowStepService : IWorkflowStepService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the WorkflowStep class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public WorkflowStepService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific workflowstep by its primary key</summary>
        /// <param name="id">The primary key of the workflowstep</param>
        /// <returns>The workflowstep data</returns>
        public WorkflowStep GetById(Guid id)
        {
            var entityData = _dbContext.WorkflowStep.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of workflowsteps based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of workflowsteps</returns>/// <exception cref="Exception"></exception>
        public List<WorkflowStep> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetWorkflowStep(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new workflowstep</summary>
        /// <param name="model">The workflowstep data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(WorkflowStep model)
        {
            model.Id = CreateWorkflowStep(model);
            return model.Id;
        }

        /// <summary>Updates a specific workflowstep by its primary key</summary>
        /// <param name="id">The primary key of the workflowstep</param>
        /// <param name="updatedEntity">The workflowstep data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, WorkflowStep updatedEntity)
        {
            UpdateWorkflowStep(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific workflowstep by its primary key</summary>
        /// <param name="id">The primary key of the workflowstep</param>
        /// <param name="updatedEntity">The workflowstep data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<WorkflowStep> updatedEntity)
        {
            PatchWorkflowStep(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific workflowstep by its primary key</summary>
        /// <param name="id">The primary key of the workflowstep</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteWorkflowStep(id);
            return true;
        }
        #region
        private List<WorkflowStep> GetWorkflowStep(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.WorkflowStep.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<WorkflowStep>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(WorkflowStep), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<WorkflowStep, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateWorkflowStep(WorkflowStep model)
        {
            _dbContext.WorkflowStep.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateWorkflowStep(Guid id, WorkflowStep updatedEntity)
        {
            _dbContext.WorkflowStep.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteWorkflowStep(Guid id)
        {
            var entityData = _dbContext.WorkflowStep.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.WorkflowStep.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchWorkflowStep(Guid id, JsonPatchDocument<WorkflowStep> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.WorkflowStep.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.WorkflowStep.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}