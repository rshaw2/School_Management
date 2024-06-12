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
    /// The attachmentService responsible for managing attachment related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting attachment information.
    /// </remarks>
    public interface IAttachmentService
    {
        /// <summary>Retrieves a specific attachment by its primary key</summary>
        /// <param name="id">The primary key of the attachment</param>
        /// <returns>The attachment data</returns>
        Attachment GetById(Guid id);

        /// <summary>Retrieves a list of attachments based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of attachments</returns>
        List<Attachment> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new attachment</summary>
        /// <param name="model">The attachment data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Attachment model);

        /// <summary>Updates a specific attachment by its primary key</summary>
        /// <param name="id">The primary key of the attachment</param>
        /// <param name="updatedEntity">The attachment data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Attachment updatedEntity);

        /// <summary>Updates a specific attachment by its primary key</summary>
        /// <param name="id">The primary key of the attachment</param>
        /// <param name="updatedEntity">The attachment data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Attachment> updatedEntity);

        /// <summary>Deletes a specific attachment by its primary key</summary>
        /// <param name="id">The primary key of the attachment</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The attachmentService responsible for managing attachment related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting attachment information.
    /// </remarks>
    public class AttachmentService : IAttachmentService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Attachment class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public AttachmentService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific attachment by its primary key</summary>
        /// <param name="id">The primary key of the attachment</param>
        /// <returns>The attachment data</returns>
        public Attachment GetById(Guid id)
        {
            var entityData = _dbContext.Attachment.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of attachments based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of attachments</returns>/// <exception cref="Exception"></exception>
        public List<Attachment> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetAttachment(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new attachment</summary>
        /// <param name="model">The attachment data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Attachment model)
        {
            model.Id = CreateAttachment(model);
            return model.Id;
        }

        /// <summary>Updates a specific attachment by its primary key</summary>
        /// <param name="id">The primary key of the attachment</param>
        /// <param name="updatedEntity">The attachment data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Attachment updatedEntity)
        {
            UpdateAttachment(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific attachment by its primary key</summary>
        /// <param name="id">The primary key of the attachment</param>
        /// <param name="updatedEntity">The attachment data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Attachment> updatedEntity)
        {
            PatchAttachment(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific attachment by its primary key</summary>
        /// <param name="id">The primary key of the attachment</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteAttachment(id);
            return true;
        }
        #region
        private List<Attachment> GetAttachment(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Attachment.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Attachment>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Attachment), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Attachment, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateAttachment(Attachment model)
        {
            _dbContext.Attachment.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateAttachment(Guid id, Attachment updatedEntity)
        {
            _dbContext.Attachment.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteAttachment(Guid id)
        {
            var entityData = _dbContext.Attachment.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Attachment.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchAttachment(Guid id, JsonPatchDocument<Attachment> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Attachment.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Attachment.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}