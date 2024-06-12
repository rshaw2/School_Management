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
    /// The emailService responsible for managing email related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting email information.
    /// </remarks>
    public interface IEmailService
    {
        /// <summary>Retrieves a specific email by its primary key</summary>
        /// <param name="id">The primary key of the email</param>
        /// <returns>The email data</returns>
        Email GetById(Guid id);

        /// <summary>Retrieves a list of emails based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of emails</returns>
        List<Email> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new email</summary>
        /// <param name="model">The email data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Email model);

        /// <summary>Updates a specific email by its primary key</summary>
        /// <param name="id">The primary key of the email</param>
        /// <param name="updatedEntity">The email data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Email updatedEntity);

        /// <summary>Updates a specific email by its primary key</summary>
        /// <param name="id">The primary key of the email</param>
        /// <param name="updatedEntity">The email data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Email> updatedEntity);

        /// <summary>Deletes a specific email by its primary key</summary>
        /// <param name="id">The primary key of the email</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The emailService responsible for managing email related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting email information.
    /// </remarks>
    public class EmailService : IEmailService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Email class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public EmailService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific email by its primary key</summary>
        /// <param name="id">The primary key of the email</param>
        /// <returns>The email data</returns>
        public Email GetById(Guid id)
        {
            var entityData = _dbContext.Email.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of emails based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of emails</returns>/// <exception cref="Exception"></exception>
        public List<Email> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetEmail(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new email</summary>
        /// <param name="model">The email data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Email model)
        {
            model.Id = CreateEmail(model);
            return model.Id;
        }

        /// <summary>Updates a specific email by its primary key</summary>
        /// <param name="id">The primary key of the email</param>
        /// <param name="updatedEntity">The email data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Email updatedEntity)
        {
            UpdateEmail(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific email by its primary key</summary>
        /// <param name="id">The primary key of the email</param>
        /// <param name="updatedEntity">The email data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Email> updatedEntity)
        {
            PatchEmail(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific email by its primary key</summary>
        /// <param name="id">The primary key of the email</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteEmail(id);
            return true;
        }
        #region
        private List<Email> GetEmail(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Email.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Email>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Email), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Email, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateEmail(Email model)
        {
            _dbContext.Email.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateEmail(Guid id, Email updatedEntity)
        {
            _dbContext.Email.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteEmail(Guid id)
        {
            var entityData = _dbContext.Email.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Email.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchEmail(Guid id, JsonPatchDocument<Email> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Email.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Email.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}