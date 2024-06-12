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
    /// The contactService responsible for managing contact related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting contact information.
    /// </remarks>
    public interface IContactService
    {
        /// <summary>Retrieves a specific contact by its primary key</summary>
        /// <param name="id">The primary key of the contact</param>
        /// <returns>The contact data</returns>
        Contact GetById(Guid id);

        /// <summary>Retrieves a list of contacts based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of contacts</returns>
        List<Contact> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new contact</summary>
        /// <param name="model">The contact data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Contact model);

        /// <summary>Updates a specific contact by its primary key</summary>
        /// <param name="id">The primary key of the contact</param>
        /// <param name="updatedEntity">The contact data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Contact updatedEntity);

        /// <summary>Updates a specific contact by its primary key</summary>
        /// <param name="id">The primary key of the contact</param>
        /// <param name="updatedEntity">The contact data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Contact> updatedEntity);

        /// <summary>Deletes a specific contact by its primary key</summary>
        /// <param name="id">The primary key of the contact</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The contactService responsible for managing contact related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting contact information.
    /// </remarks>
    public class ContactService : IContactService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Contact class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public ContactService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific contact by its primary key</summary>
        /// <param name="id">The primary key of the contact</param>
        /// <returns>The contact data</returns>
        public Contact GetById(Guid id)
        {
            var entityData = _dbContext.Contact.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of contacts based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of contacts</returns>/// <exception cref="Exception"></exception>
        public List<Contact> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetContact(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new contact</summary>
        /// <param name="model">The contact data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Contact model)
        {
            model.Id = CreateContact(model);
            return model.Id;
        }

        /// <summary>Updates a specific contact by its primary key</summary>
        /// <param name="id">The primary key of the contact</param>
        /// <param name="updatedEntity">The contact data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Contact updatedEntity)
        {
            UpdateContact(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific contact by its primary key</summary>
        /// <param name="id">The primary key of the contact</param>
        /// <param name="updatedEntity">The contact data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Contact> updatedEntity)
        {
            PatchContact(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific contact by its primary key</summary>
        /// <param name="id">The primary key of the contact</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteContact(id);
            return true;
        }
        #region
        private List<Contact> GetContact(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Contact.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Contact>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Contact), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Contact, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateContact(Contact model)
        {
            _dbContext.Contact.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateContact(Guid id, Contact updatedEntity)
        {
            _dbContext.Contact.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteContact(Guid id)
        {
            var entityData = _dbContext.Contact.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Contact.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchContact(Guid id, JsonPatchDocument<Contact> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Contact.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Contact.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}