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
    /// The emergencycontactService responsible for managing emergencycontact related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting emergencycontact information.
    /// </remarks>
    public interface IEmergencyContactService
    {
        /// <summary>Retrieves a specific emergencycontact by its primary key</summary>
        /// <param name="id">The primary key of the emergencycontact</param>
        /// <returns>The emergencycontact data</returns>
        EmergencyContact GetById(Guid id);

        /// <summary>Retrieves a list of emergencycontacts based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of emergencycontacts</returns>
        List<EmergencyContact> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new emergencycontact</summary>
        /// <param name="model">The emergencycontact data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(EmergencyContact model);

        /// <summary>Updates a specific emergencycontact by its primary key</summary>
        /// <param name="id">The primary key of the emergencycontact</param>
        /// <param name="updatedEntity">The emergencycontact data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, EmergencyContact updatedEntity);

        /// <summary>Updates a specific emergencycontact by its primary key</summary>
        /// <param name="id">The primary key of the emergencycontact</param>
        /// <param name="updatedEntity">The emergencycontact data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<EmergencyContact> updatedEntity);

        /// <summary>Deletes a specific emergencycontact by its primary key</summary>
        /// <param name="id">The primary key of the emergencycontact</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The emergencycontactService responsible for managing emergencycontact related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting emergencycontact information.
    /// </remarks>
    public class EmergencyContactService : IEmergencyContactService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the EmergencyContact class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public EmergencyContactService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific emergencycontact by its primary key</summary>
        /// <param name="id">The primary key of the emergencycontact</param>
        /// <returns>The emergencycontact data</returns>
        public EmergencyContact GetById(Guid id)
        {
            var entityData = _dbContext.EmergencyContact.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of emergencycontacts based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of emergencycontacts</returns>/// <exception cref="Exception"></exception>
        public List<EmergencyContact> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetEmergencyContact(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new emergencycontact</summary>
        /// <param name="model">The emergencycontact data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(EmergencyContact model)
        {
            model.Id = CreateEmergencyContact(model);
            return model.Id;
        }

        /// <summary>Updates a specific emergencycontact by its primary key</summary>
        /// <param name="id">The primary key of the emergencycontact</param>
        /// <param name="updatedEntity">The emergencycontact data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, EmergencyContact updatedEntity)
        {
            UpdateEmergencyContact(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific emergencycontact by its primary key</summary>
        /// <param name="id">The primary key of the emergencycontact</param>
        /// <param name="updatedEntity">The emergencycontact data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<EmergencyContact> updatedEntity)
        {
            PatchEmergencyContact(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific emergencycontact by its primary key</summary>
        /// <param name="id">The primary key of the emergencycontact</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteEmergencyContact(id);
            return true;
        }
        #region
        private List<EmergencyContact> GetEmergencyContact(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.EmergencyContact.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<EmergencyContact>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(EmergencyContact), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<EmergencyContact, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateEmergencyContact(EmergencyContact model)
        {
            _dbContext.EmergencyContact.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateEmergencyContact(Guid id, EmergencyContact updatedEntity)
        {
            _dbContext.EmergencyContact.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteEmergencyContact(Guid id)
        {
            var entityData = _dbContext.EmergencyContact.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.EmergencyContact.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchEmergencyContact(Guid id, JsonPatchDocument<EmergencyContact> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.EmergencyContact.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.EmergencyContact.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}