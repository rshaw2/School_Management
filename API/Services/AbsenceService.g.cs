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
    /// The absenceService responsible for managing absence related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting absence information.
    /// </remarks>
    public interface IAbsenceService
    {
        /// <summary>Retrieves a specific absence by its primary key</summary>
        /// <param name="id">The primary key of the absence</param>
        /// <returns>The absence data</returns>
        Absence GetById(Guid id);

        /// <summary>Retrieves a list of absences based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of absences</returns>
        List<Absence> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new absence</summary>
        /// <param name="model">The absence data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Absence model);

        /// <summary>Updates a specific absence by its primary key</summary>
        /// <param name="id">The primary key of the absence</param>
        /// <param name="updatedEntity">The absence data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Absence updatedEntity);

        /// <summary>Updates a specific absence by its primary key</summary>
        /// <param name="id">The primary key of the absence</param>
        /// <param name="updatedEntity">The absence data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Absence> updatedEntity);

        /// <summary>Deletes a specific absence by its primary key</summary>
        /// <param name="id">The primary key of the absence</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The absenceService responsible for managing absence related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting absence information.
    /// </remarks>
    public class AbsenceService : IAbsenceService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Absence class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public AbsenceService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific absence by its primary key</summary>
        /// <param name="id">The primary key of the absence</param>
        /// <returns>The absence data</returns>
        public Absence GetById(Guid id)
        {
            var entityData = _dbContext.Absence.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of absences based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of absences</returns>/// <exception cref="Exception"></exception>
        public List<Absence> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetAbsence(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new absence</summary>
        /// <param name="model">The absence data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Absence model)
        {
            model.Id = CreateAbsence(model);
            return model.Id;
        }

        /// <summary>Updates a specific absence by its primary key</summary>
        /// <param name="id">The primary key of the absence</param>
        /// <param name="updatedEntity">The absence data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Absence updatedEntity)
        {
            UpdateAbsence(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific absence by its primary key</summary>
        /// <param name="id">The primary key of the absence</param>
        /// <param name="updatedEntity">The absence data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Absence> updatedEntity)
        {
            PatchAbsence(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific absence by its primary key</summary>
        /// <param name="id">The primary key of the absence</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteAbsence(id);
            return true;
        }
        #region
        private List<Absence> GetAbsence(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Absence.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Absence>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Absence), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Absence, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateAbsence(Absence model)
        {
            _dbContext.Absence.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateAbsence(Guid id, Absence updatedEntity)
        {
            _dbContext.Absence.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteAbsence(Guid id)
        {
            var entityData = _dbContext.Absence.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Absence.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchAbsence(Guid id, JsonPatchDocument<Absence> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Absence.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Absence.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}