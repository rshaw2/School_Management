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
    /// The academicsessionService responsible for managing academicsession related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting academicsession information.
    /// </remarks>
    public interface IAcademicSessionService
    {
        /// <summary>Retrieves a specific academicsession by its primary key</summary>
        /// <param name="id">The primary key of the academicsession</param>
        /// <returns>The academicsession data</returns>
        AcademicSession GetById(Guid id);

        /// <summary>Retrieves a list of academicsessions based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of academicsessions</returns>
        List<AcademicSession> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new academicsession</summary>
        /// <param name="model">The academicsession data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(AcademicSession model);

        /// <summary>Updates a specific academicsession by its primary key</summary>
        /// <param name="id">The primary key of the academicsession</param>
        /// <param name="updatedEntity">The academicsession data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, AcademicSession updatedEntity);

        /// <summary>Updates a specific academicsession by its primary key</summary>
        /// <param name="id">The primary key of the academicsession</param>
        /// <param name="updatedEntity">The academicsession data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<AcademicSession> updatedEntity);

        /// <summary>Deletes a specific academicsession by its primary key</summary>
        /// <param name="id">The primary key of the academicsession</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The academicsessionService responsible for managing academicsession related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting academicsession information.
    /// </remarks>
    public class AcademicSessionService : IAcademicSessionService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the AcademicSession class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public AcademicSessionService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific academicsession by its primary key</summary>
        /// <param name="id">The primary key of the academicsession</param>
        /// <returns>The academicsession data</returns>
        public AcademicSession GetById(Guid id)
        {
            var entityData = _dbContext.AcademicSession.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of academicsessions based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of academicsessions</returns>/// <exception cref="Exception"></exception>
        public List<AcademicSession> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetAcademicSession(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new academicsession</summary>
        /// <param name="model">The academicsession data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(AcademicSession model)
        {
            model.Id = CreateAcademicSession(model);
            return model.Id;
        }

        /// <summary>Updates a specific academicsession by its primary key</summary>
        /// <param name="id">The primary key of the academicsession</param>
        /// <param name="updatedEntity">The academicsession data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, AcademicSession updatedEntity)
        {
            UpdateAcademicSession(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific academicsession by its primary key</summary>
        /// <param name="id">The primary key of the academicsession</param>
        /// <param name="updatedEntity">The academicsession data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<AcademicSession> updatedEntity)
        {
            PatchAcademicSession(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific academicsession by its primary key</summary>
        /// <param name="id">The primary key of the academicsession</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteAcademicSession(id);
            return true;
        }
        #region
        private List<AcademicSession> GetAcademicSession(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.AcademicSession.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<AcademicSession>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(AcademicSession), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<AcademicSession, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateAcademicSession(AcademicSession model)
        {
            _dbContext.AcademicSession.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateAcademicSession(Guid id, AcademicSession updatedEntity)
        {
            _dbContext.AcademicSession.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteAcademicSession(Guid id)
        {
            var entityData = _dbContext.AcademicSession.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.AcademicSession.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchAcademicSession(Guid id, JsonPatchDocument<AcademicSession> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.AcademicSession.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.AcademicSession.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}