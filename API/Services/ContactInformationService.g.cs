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
    /// The contactinformationService responsible for managing contactinformation related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting contactinformation information.
    /// </remarks>
    public interface IContactInformationService
    {
        /// <summary>Retrieves a specific contactinformation by its primary key</summary>
        /// <param name="id">The primary key of the contactinformation</param>
        /// <returns>The contactinformation data</returns>
        ContactInformation GetById(Guid id);

        /// <summary>Retrieves a list of contactinformations based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of contactinformations</returns>
        List<ContactInformation> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new contactinformation</summary>
        /// <param name="model">The contactinformation data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(ContactInformation model);

        /// <summary>Updates a specific contactinformation by its primary key</summary>
        /// <param name="id">The primary key of the contactinformation</param>
        /// <param name="updatedEntity">The contactinformation data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, ContactInformation updatedEntity);

        /// <summary>Updates a specific contactinformation by its primary key</summary>
        /// <param name="id">The primary key of the contactinformation</param>
        /// <param name="updatedEntity">The contactinformation data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<ContactInformation> updatedEntity);

        /// <summary>Deletes a specific contactinformation by its primary key</summary>
        /// <param name="id">The primary key of the contactinformation</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The contactinformationService responsible for managing contactinformation related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting contactinformation information.
    /// </remarks>
    public class ContactInformationService : IContactInformationService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the ContactInformation class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public ContactInformationService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific contactinformation by its primary key</summary>
        /// <param name="id">The primary key of the contactinformation</param>
        /// <returns>The contactinformation data</returns>
        public ContactInformation GetById(Guid id)
        {
            var entityData = _dbContext.ContactInformation.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of contactinformations based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of contactinformations</returns>/// <exception cref="Exception"></exception>
        public List<ContactInformation> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetContactInformation(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new contactinformation</summary>
        /// <param name="model">The contactinformation data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(ContactInformation model)
        {
            model.Id = CreateContactInformation(model);
            return model.Id;
        }

        /// <summary>Updates a specific contactinformation by its primary key</summary>
        /// <param name="id">The primary key of the contactinformation</param>
        /// <param name="updatedEntity">The contactinformation data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, ContactInformation updatedEntity)
        {
            UpdateContactInformation(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific contactinformation by its primary key</summary>
        /// <param name="id">The primary key of the contactinformation</param>
        /// <param name="updatedEntity">The contactinformation data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<ContactInformation> updatedEntity)
        {
            PatchContactInformation(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific contactinformation by its primary key</summary>
        /// <param name="id">The primary key of the contactinformation</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteContactInformation(id);
            return true;
        }
        #region
        private List<ContactInformation> GetContactInformation(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.ContactInformation.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<ContactInformation>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(ContactInformation), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<ContactInformation, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateContactInformation(ContactInformation model)
        {
            _dbContext.ContactInformation.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateContactInformation(Guid id, ContactInformation updatedEntity)
        {
            _dbContext.ContactInformation.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteContactInformation(Guid id)
        {
            var entityData = _dbContext.ContactInformation.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.ContactInformation.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchContactInformation(Guid id, JsonPatchDocument<ContactInformation> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.ContactInformation.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.ContactInformation.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}