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
    /// The assignmentService responsible for managing assignment related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting assignment information.
    /// </remarks>
    public interface IAssignmentService
    {
        /// <summary>Retrieves a specific assignment by its primary key</summary>
        /// <param name="id">The primary key of the assignment</param>
        /// <returns>The assignment data</returns>
        Assignment GetById(Guid id);

        /// <summary>Retrieves a list of assignments based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of assignments</returns>
        List<Assignment> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new assignment</summary>
        /// <param name="model">The assignment data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Assignment model);

        /// <summary>Updates a specific assignment by its primary key</summary>
        /// <param name="id">The primary key of the assignment</param>
        /// <param name="updatedEntity">The assignment data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Assignment updatedEntity);

        /// <summary>Updates a specific assignment by its primary key</summary>
        /// <param name="id">The primary key of the assignment</param>
        /// <param name="updatedEntity">The assignment data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Assignment> updatedEntity);

        /// <summary>Deletes a specific assignment by its primary key</summary>
        /// <param name="id">The primary key of the assignment</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The assignmentService responsible for managing assignment related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting assignment information.
    /// </remarks>
    public class AssignmentService : IAssignmentService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Assignment class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public AssignmentService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific assignment by its primary key</summary>
        /// <param name="id">The primary key of the assignment</param>
        /// <returns>The assignment data</returns>
        public Assignment GetById(Guid id)
        {
            var entityData = _dbContext.Assignment.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of assignments based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of assignments</returns>/// <exception cref="Exception"></exception>
        public List<Assignment> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetAssignment(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new assignment</summary>
        /// <param name="model">The assignment data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Assignment model)
        {
            model.Id = CreateAssignment(model);
            return model.Id;
        }

        /// <summary>Updates a specific assignment by its primary key</summary>
        /// <param name="id">The primary key of the assignment</param>
        /// <param name="updatedEntity">The assignment data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Assignment updatedEntity)
        {
            UpdateAssignment(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific assignment by its primary key</summary>
        /// <param name="id">The primary key of the assignment</param>
        /// <param name="updatedEntity">The assignment data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Assignment> updatedEntity)
        {
            PatchAssignment(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific assignment by its primary key</summary>
        /// <param name="id">The primary key of the assignment</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteAssignment(id);
            return true;
        }
        #region
        private List<Assignment> GetAssignment(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Assignment.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Assignment>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Assignment), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Assignment, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateAssignment(Assignment model)
        {
            _dbContext.Assignment.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateAssignment(Guid id, Assignment updatedEntity)
        {
            _dbContext.Assignment.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteAssignment(Guid id)
        {
            var entityData = _dbContext.Assignment.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Assignment.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchAssignment(Guid id, JsonPatchDocument<Assignment> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Assignment.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Assignment.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}