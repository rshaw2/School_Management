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
    /// The approvalService responsible for managing approval related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting approval information.
    /// </remarks>
    public interface IApprovalService
    {
        /// <summary>Retrieves a specific approval by its primary key</summary>
        /// <param name="id">The primary key of the approval</param>
        /// <returns>The approval data</returns>
        Approval GetById(Guid id);

        /// <summary>Retrieves a list of approvals based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of approvals</returns>
        List<Approval> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new approval</summary>
        /// <param name="model">The approval data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Approval model);

        /// <summary>Updates a specific approval by its primary key</summary>
        /// <param name="id">The primary key of the approval</param>
        /// <param name="updatedEntity">The approval data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Approval updatedEntity);

        /// <summary>Updates a specific approval by its primary key</summary>
        /// <param name="id">The primary key of the approval</param>
        /// <param name="updatedEntity">The approval data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Approval> updatedEntity);

        /// <summary>Deletes a specific approval by its primary key</summary>
        /// <param name="id">The primary key of the approval</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The approvalService responsible for managing approval related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting approval information.
    /// </remarks>
    public class ApprovalService : IApprovalService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Approval class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public ApprovalService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific approval by its primary key</summary>
        /// <param name="id">The primary key of the approval</param>
        /// <returns>The approval data</returns>
        public Approval GetById(Guid id)
        {
            var entityData = _dbContext.Approval.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of approvals based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of approvals</returns>/// <exception cref="Exception"></exception>
        public List<Approval> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetApproval(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new approval</summary>
        /// <param name="model">The approval data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Approval model)
        {
            model.Id = CreateApproval(model);
            return model.Id;
        }

        /// <summary>Updates a specific approval by its primary key</summary>
        /// <param name="id">The primary key of the approval</param>
        /// <param name="updatedEntity">The approval data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Approval updatedEntity)
        {
            UpdateApproval(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific approval by its primary key</summary>
        /// <param name="id">The primary key of the approval</param>
        /// <param name="updatedEntity">The approval data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Approval> updatedEntity)
        {
            PatchApproval(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific approval by its primary key</summary>
        /// <param name="id">The primary key of the approval</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteApproval(id);
            return true;
        }
        #region
        private List<Approval> GetApproval(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Approval.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Approval>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Approval), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Approval, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateApproval(Approval model)
        {
            _dbContext.Approval.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateApproval(Guid id, Approval updatedEntity)
        {
            _dbContext.Approval.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteApproval(Guid id)
        {
            var entityData = _dbContext.Approval.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Approval.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchApproval(Guid id, JsonPatchDocument<Approval> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Approval.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Approval.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}