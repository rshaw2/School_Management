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
    /// The academicyearService responsible for managing academicyear related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting academicyear information.
    /// </remarks>
    public interface IAcademicYearService
    {
        /// <summary>Retrieves a specific academicyear by its primary key</summary>
        /// <param name="id">The primary key of the academicyear</param>
        /// <returns>The academicyear data</returns>
        AcademicYear GetById(Guid id);

        /// <summary>Retrieves a list of academicyears based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of academicyears</returns>
        List<AcademicYear> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new academicyear</summary>
        /// <param name="model">The academicyear data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(AcademicYear model);

        /// <summary>Updates a specific academicyear by its primary key</summary>
        /// <param name="id">The primary key of the academicyear</param>
        /// <param name="updatedEntity">The academicyear data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, AcademicYear updatedEntity);

        /// <summary>Updates a specific academicyear by its primary key</summary>
        /// <param name="id">The primary key of the academicyear</param>
        /// <param name="updatedEntity">The academicyear data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<AcademicYear> updatedEntity);

        /// <summary>Deletes a specific academicyear by its primary key</summary>
        /// <param name="id">The primary key of the academicyear</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The academicyearService responsible for managing academicyear related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting academicyear information.
    /// </remarks>
    public class AcademicYearService : IAcademicYearService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the AcademicYear class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public AcademicYearService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific academicyear by its primary key</summary>
        /// <param name="id">The primary key of the academicyear</param>
        /// <returns>The academicyear data</returns>
        public AcademicYear GetById(Guid id)
        {
            var entityData = _dbContext.AcademicYear.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of academicyears based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of academicyears</returns>/// <exception cref="Exception"></exception>
        public List<AcademicYear> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetAcademicYear(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new academicyear</summary>
        /// <param name="model">The academicyear data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(AcademicYear model)
        {
            model.Id = CreateAcademicYear(model);
            return model.Id;
        }

        /// <summary>Updates a specific academicyear by its primary key</summary>
        /// <param name="id">The primary key of the academicyear</param>
        /// <param name="updatedEntity">The academicyear data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, AcademicYear updatedEntity)
        {
            UpdateAcademicYear(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific academicyear by its primary key</summary>
        /// <param name="id">The primary key of the academicyear</param>
        /// <param name="updatedEntity">The academicyear data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<AcademicYear> updatedEntity)
        {
            PatchAcademicYear(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific academicyear by its primary key</summary>
        /// <param name="id">The primary key of the academicyear</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteAcademicYear(id);
            return true;
        }
        #region
        private List<AcademicYear> GetAcademicYear(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.AcademicYear.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<AcademicYear>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(AcademicYear), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<AcademicYear, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateAcademicYear(AcademicYear model)
        {
            _dbContext.AcademicYear.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateAcademicYear(Guid id, AcademicYear updatedEntity)
        {
            _dbContext.AcademicYear.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteAcademicYear(Guid id)
        {
            var entityData = _dbContext.AcademicYear.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.AcademicYear.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchAcademicYear(Guid id, JsonPatchDocument<AcademicYear> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.AcademicYear.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.AcademicYear.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}