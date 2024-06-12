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
    /// The workflowService responsible for managing workflow related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting workflow information.
    /// </remarks>
    public interface IWorkflowService
    {
        /// <summary>Retrieves a specific workflow by its primary key</summary>
        /// <param name="id">The primary key of the workflow</param>
        /// <returns>The workflow data</returns>
        Workflow GetById(Guid id);

        /// <summary>Retrieves a list of workflows based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of workflows</returns>
        List<Workflow> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new workflow</summary>
        /// <param name="model">The workflow data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Workflow model);

        /// <summary>Updates a specific workflow by its primary key</summary>
        /// <param name="id">The primary key of the workflow</param>
        /// <param name="updatedEntity">The workflow data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Workflow updatedEntity);

        /// <summary>Updates a specific workflow by its primary key</summary>
        /// <param name="id">The primary key of the workflow</param>
        /// <param name="updatedEntity">The workflow data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Workflow> updatedEntity);

        /// <summary>Deletes a specific workflow by its primary key</summary>
        /// <param name="id">The primary key of the workflow</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The workflowService responsible for managing workflow related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting workflow information.
    /// </remarks>
    public class WorkflowService : IWorkflowService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Workflow class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public WorkflowService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific workflow by its primary key</summary>
        /// <param name="id">The primary key of the workflow</param>
        /// <returns>The workflow data</returns>
        public Workflow GetById(Guid id)
        {
            var entityData = _dbContext.Workflow.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of workflows based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of workflows</returns>/// <exception cref="Exception"></exception>
        public List<Workflow> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetWorkflow(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new workflow</summary>
        /// <param name="model">The workflow data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Workflow model)
        {
            model.Id = CreateWorkflow(model);
            return model.Id;
        }

        /// <summary>Updates a specific workflow by its primary key</summary>
        /// <param name="id">The primary key of the workflow</param>
        /// <param name="updatedEntity">The workflow data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Workflow updatedEntity)
        {
            UpdateWorkflow(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific workflow by its primary key</summary>
        /// <param name="id">The primary key of the workflow</param>
        /// <param name="updatedEntity">The workflow data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Workflow> updatedEntity)
        {
            PatchWorkflow(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific workflow by its primary key</summary>
        /// <param name="id">The primary key of the workflow</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteWorkflow(id);
            return true;
        }
        #region
        private List<Workflow> GetWorkflow(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Workflow.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Workflow>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Workflow), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Workflow, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateWorkflow(Workflow model)
        {
            _dbContext.Workflow.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateWorkflow(Guid id, Workflow updatedEntity)
        {
            _dbContext.Workflow.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteWorkflow(Guid id)
        {
            var entityData = _dbContext.Workflow.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Workflow.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchWorkflow(Guid id, JsonPatchDocument<Workflow> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Workflow.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Workflow.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}