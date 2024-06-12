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
    /// The attendancestatusService responsible for managing attendancestatus related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting attendancestatus information.
    /// </remarks>
    public interface IAttendanceStatusService
    {
        /// <summary>Retrieves a specific attendancestatus by its primary key</summary>
        /// <param name="id">The primary key of the attendancestatus</param>
        /// <returns>The attendancestatus data</returns>
        AttendanceStatus GetById(Guid id);

        /// <summary>Retrieves a list of attendancestatuss based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of attendancestatuss</returns>
        List<AttendanceStatus> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new attendancestatus</summary>
        /// <param name="model">The attendancestatus data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(AttendanceStatus model);

        /// <summary>Updates a specific attendancestatus by its primary key</summary>
        /// <param name="id">The primary key of the attendancestatus</param>
        /// <param name="updatedEntity">The attendancestatus data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, AttendanceStatus updatedEntity);

        /// <summary>Updates a specific attendancestatus by its primary key</summary>
        /// <param name="id">The primary key of the attendancestatus</param>
        /// <param name="updatedEntity">The attendancestatus data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<AttendanceStatus> updatedEntity);

        /// <summary>Deletes a specific attendancestatus by its primary key</summary>
        /// <param name="id">The primary key of the attendancestatus</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The attendancestatusService responsible for managing attendancestatus related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting attendancestatus information.
    /// </remarks>
    public class AttendanceStatusService : IAttendanceStatusService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the AttendanceStatus class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public AttendanceStatusService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific attendancestatus by its primary key</summary>
        /// <param name="id">The primary key of the attendancestatus</param>
        /// <returns>The attendancestatus data</returns>
        public AttendanceStatus GetById(Guid id)
        {
            var entityData = _dbContext.AttendanceStatus.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of attendancestatuss based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of attendancestatuss</returns>/// <exception cref="Exception"></exception>
        public List<AttendanceStatus> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetAttendanceStatus(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new attendancestatus</summary>
        /// <param name="model">The attendancestatus data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(AttendanceStatus model)
        {
            model.Id = CreateAttendanceStatus(model);
            return model.Id;
        }

        /// <summary>Updates a specific attendancestatus by its primary key</summary>
        /// <param name="id">The primary key of the attendancestatus</param>
        /// <param name="updatedEntity">The attendancestatus data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, AttendanceStatus updatedEntity)
        {
            UpdateAttendanceStatus(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific attendancestatus by its primary key</summary>
        /// <param name="id">The primary key of the attendancestatus</param>
        /// <param name="updatedEntity">The attendancestatus data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<AttendanceStatus> updatedEntity)
        {
            PatchAttendanceStatus(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific attendancestatus by its primary key</summary>
        /// <param name="id">The primary key of the attendancestatus</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteAttendanceStatus(id);
            return true;
        }
        #region
        private List<AttendanceStatus> GetAttendanceStatus(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.AttendanceStatus.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<AttendanceStatus>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(AttendanceStatus), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<AttendanceStatus, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateAttendanceStatus(AttendanceStatus model)
        {
            _dbContext.AttendanceStatus.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateAttendanceStatus(Guid id, AttendanceStatus updatedEntity)
        {
            _dbContext.AttendanceStatus.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteAttendanceStatus(Guid id)
        {
            var entityData = _dbContext.AttendanceStatus.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.AttendanceStatus.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchAttendanceStatus(Guid id, JsonPatchDocument<AttendanceStatus> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.AttendanceStatus.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.AttendanceStatus.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}