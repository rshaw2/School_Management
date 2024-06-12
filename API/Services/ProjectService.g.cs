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
    /// The projectService responsible for managing project related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting project information.
    /// </remarks>
    public interface IProjectService
    {
        /// <summary>Retrieves a specific project by its primary key</summary>
        /// <param name="id">The primary key of the project</param>
        /// <returns>The project data</returns>
        Project GetById(Guid id);

        /// <summary>Retrieves a list of projects based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of projects</returns>
        List<Project> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new project</summary>
        /// <param name="model">The project data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Project model);

        /// <summary>Updates a specific project by its primary key</summary>
        /// <param name="id">The primary key of the project</param>
        /// <param name="updatedEntity">The project data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Project updatedEntity);

        /// <summary>Updates a specific project by its primary key</summary>
        /// <param name="id">The primary key of the project</param>
        /// <param name="updatedEntity">The project data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Project> updatedEntity);

        /// <summary>Deletes a specific project by its primary key</summary>
        /// <param name="id">The primary key of the project</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The projectService responsible for managing project related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting project information.
    /// </remarks>
    public class ProjectService : IProjectService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Project class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public ProjectService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific project by its primary key</summary>
        /// <param name="id">The primary key of the project</param>
        /// <returns>The project data</returns>
        public Project GetById(Guid id)
        {
            var entityData = _dbContext.Project.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of projects based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of projects</returns>/// <exception cref="Exception"></exception>
        public List<Project> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetProject(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new project</summary>
        /// <param name="model">The project data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Project model)
        {
            model.Id = CreateProject(model);
            return model.Id;
        }

        /// <summary>Updates a specific project by its primary key</summary>
        /// <param name="id">The primary key of the project</param>
        /// <param name="updatedEntity">The project data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Project updatedEntity)
        {
            UpdateProject(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific project by its primary key</summary>
        /// <param name="id">The primary key of the project</param>
        /// <param name="updatedEntity">The project data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Project> updatedEntity)
        {
            PatchProject(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific project by its primary key</summary>
        /// <param name="id">The primary key of the project</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteProject(id);
            return true;
        }
        #region
        private List<Project> GetProject(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Project.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Project>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Project), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Project, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateProject(Project model)
        {
            _dbContext.Project.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateProject(Guid id, Project updatedEntity)
        {
            _dbContext.Project.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteProject(Guid id)
        {
            var entityData = _dbContext.Project.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Project.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchProject(Guid id, JsonPatchDocument<Project> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Project.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Project.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}