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
    /// The collaboratorService responsible for managing collaborator related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting collaborator information.
    /// </remarks>
    public interface ICollaboratorService
    {
        /// <summary>Retrieves a specific collaborator by its primary key</summary>
        /// <param name="id">The primary key of the collaborator</param>
        /// <returns>The collaborator data</returns>
        Collaborator GetById(Guid id);

        /// <summary>Retrieves a list of collaborators based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of collaborators</returns>
        List<Collaborator> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new collaborator</summary>
        /// <param name="model">The collaborator data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Collaborator model);

        /// <summary>Updates a specific collaborator by its primary key</summary>
        /// <param name="id">The primary key of the collaborator</param>
        /// <param name="updatedEntity">The collaborator data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Collaborator updatedEntity);

        /// <summary>Updates a specific collaborator by its primary key</summary>
        /// <param name="id">The primary key of the collaborator</param>
        /// <param name="updatedEntity">The collaborator data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Collaborator> updatedEntity);

        /// <summary>Deletes a specific collaborator by its primary key</summary>
        /// <param name="id">The primary key of the collaborator</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The collaboratorService responsible for managing collaborator related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting collaborator information.
    /// </remarks>
    public class CollaboratorService : ICollaboratorService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Collaborator class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public CollaboratorService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific collaborator by its primary key</summary>
        /// <param name="id">The primary key of the collaborator</param>
        /// <returns>The collaborator data</returns>
        public Collaborator GetById(Guid id)
        {
            var entityData = _dbContext.Collaborator.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of collaborators based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of collaborators</returns>/// <exception cref="Exception"></exception>
        public List<Collaborator> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetCollaborator(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new collaborator</summary>
        /// <param name="model">The collaborator data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Collaborator model)
        {
            model.Id = CreateCollaborator(model);
            return model.Id;
        }

        /// <summary>Updates a specific collaborator by its primary key</summary>
        /// <param name="id">The primary key of the collaborator</param>
        /// <param name="updatedEntity">The collaborator data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Collaborator updatedEntity)
        {
            UpdateCollaborator(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific collaborator by its primary key</summary>
        /// <param name="id">The primary key of the collaborator</param>
        /// <param name="updatedEntity">The collaborator data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Collaborator> updatedEntity)
        {
            PatchCollaborator(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific collaborator by its primary key</summary>
        /// <param name="id">The primary key of the collaborator</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteCollaborator(id);
            return true;
        }
        #region
        private List<Collaborator> GetCollaborator(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Collaborator.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Collaborator>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Collaborator), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Collaborator, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateCollaborator(Collaborator model)
        {
            _dbContext.Collaborator.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateCollaborator(Guid id, Collaborator updatedEntity)
        {
            _dbContext.Collaborator.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteCollaborator(Guid id)
        {
            var entityData = _dbContext.Collaborator.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Collaborator.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchCollaborator(Guid id, JsonPatchDocument<Collaborator> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Collaborator.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Collaborator.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}