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
    /// The workspaceService responsible for managing workspace related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting workspace information.
    /// </remarks>
    public interface IWorkspaceService
    {
        /// <summary>Retrieves a specific workspace by its primary key</summary>
        /// <param name="id">The primary key of the workspace</param>
        /// <returns>The workspace data</returns>
        Workspace GetById(Guid id);

        /// <summary>Retrieves a list of workspaces based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of workspaces</returns>
        List<Workspace> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new workspace</summary>
        /// <param name="model">The workspace data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Workspace model);

        /// <summary>Updates a specific workspace by its primary key</summary>
        /// <param name="id">The primary key of the workspace</param>
        /// <param name="updatedEntity">The workspace data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Workspace updatedEntity);

        /// <summary>Updates a specific workspace by its primary key</summary>
        /// <param name="id">The primary key of the workspace</param>
        /// <param name="updatedEntity">The workspace data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Workspace> updatedEntity);

        /// <summary>Deletes a specific workspace by its primary key</summary>
        /// <param name="id">The primary key of the workspace</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The workspaceService responsible for managing workspace related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting workspace information.
    /// </remarks>
    public class WorkspaceService : IWorkspaceService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Workspace class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public WorkspaceService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific workspace by its primary key</summary>
        /// <param name="id">The primary key of the workspace</param>
        /// <returns>The workspace data</returns>
        public Workspace GetById(Guid id)
        {
            var entityData = _dbContext.Workspace.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of workspaces based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of workspaces</returns>/// <exception cref="Exception"></exception>
        public List<Workspace> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetWorkspace(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new workspace</summary>
        /// <param name="model">The workspace data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Workspace model)
        {
            model.Id = CreateWorkspace(model);
            return model.Id;
        }

        /// <summary>Updates a specific workspace by its primary key</summary>
        /// <param name="id">The primary key of the workspace</param>
        /// <param name="updatedEntity">The workspace data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Workspace updatedEntity)
        {
            UpdateWorkspace(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific workspace by its primary key</summary>
        /// <param name="id">The primary key of the workspace</param>
        /// <param name="updatedEntity">The workspace data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Workspace> updatedEntity)
        {
            PatchWorkspace(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific workspace by its primary key</summary>
        /// <param name="id">The primary key of the workspace</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteWorkspace(id);
            return true;
        }
        #region
        private List<Workspace> GetWorkspace(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Workspace.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Workspace>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Workspace), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Workspace, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateWorkspace(Workspace model)
        {
            _dbContext.Workspace.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateWorkspace(Guid id, Workspace updatedEntity)
        {
            _dbContext.Workspace.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteWorkspace(Guid id)
        {
            var entityData = _dbContext.Workspace.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Workspace.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchWorkspace(Guid id, JsonPatchDocument<Workspace> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Workspace.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Workspace.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}