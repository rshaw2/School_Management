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
    /// The attendancereportService responsible for managing attendancereport related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting attendancereport information.
    /// </remarks>
    public interface IAttendanceReportService
    {
        /// <summary>Retrieves a specific attendancereport by its primary key</summary>
        /// <param name="id">The primary key of the attendancereport</param>
        /// <returns>The attendancereport data</returns>
        AttendanceReport GetById(Guid id);

        /// <summary>Retrieves a list of attendancereports based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of attendancereports</returns>
        List<AttendanceReport> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new attendancereport</summary>
        /// <param name="model">The attendancereport data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(AttendanceReport model);

        /// <summary>Updates a specific attendancereport by its primary key</summary>
        /// <param name="id">The primary key of the attendancereport</param>
        /// <param name="updatedEntity">The attendancereport data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, AttendanceReport updatedEntity);

        /// <summary>Updates a specific attendancereport by its primary key</summary>
        /// <param name="id">The primary key of the attendancereport</param>
        /// <param name="updatedEntity">The attendancereport data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<AttendanceReport> updatedEntity);

        /// <summary>Deletes a specific attendancereport by its primary key</summary>
        /// <param name="id">The primary key of the attendancereport</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The attendancereportService responsible for managing attendancereport related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting attendancereport information.
    /// </remarks>
    public class AttendanceReportService : IAttendanceReportService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the AttendanceReport class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public AttendanceReportService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific attendancereport by its primary key</summary>
        /// <param name="id">The primary key of the attendancereport</param>
        /// <returns>The attendancereport data</returns>
        public AttendanceReport GetById(Guid id)
        {
            var entityData = _dbContext.AttendanceReport.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of attendancereports based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of attendancereports</returns>/// <exception cref="Exception"></exception>
        public List<AttendanceReport> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetAttendanceReport(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new attendancereport</summary>
        /// <param name="model">The attendancereport data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(AttendanceReport model)
        {
            model.Id = CreateAttendanceReport(model);
            return model.Id;
        }

        /// <summary>Updates a specific attendancereport by its primary key</summary>
        /// <param name="id">The primary key of the attendancereport</param>
        /// <param name="updatedEntity">The attendancereport data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, AttendanceReport updatedEntity)
        {
            UpdateAttendanceReport(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific attendancereport by its primary key</summary>
        /// <param name="id">The primary key of the attendancereport</param>
        /// <param name="updatedEntity">The attendancereport data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<AttendanceReport> updatedEntity)
        {
            PatchAttendanceReport(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific attendancereport by its primary key</summary>
        /// <param name="id">The primary key of the attendancereport</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteAttendanceReport(id);
            return true;
        }
        #region
        private List<AttendanceReport> GetAttendanceReport(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.AttendanceReport.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<AttendanceReport>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(AttendanceReport), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<AttendanceReport, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateAttendanceReport(AttendanceReport model)
        {
            _dbContext.AttendanceReport.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateAttendanceReport(Guid id, AttendanceReport updatedEntity)
        {
            _dbContext.AttendanceReport.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteAttendanceReport(Guid id)
        {
            var entityData = _dbContext.AttendanceReport.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.AttendanceReport.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchAttendanceReport(Guid id, JsonPatchDocument<AttendanceReport> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.AttendanceReport.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.AttendanceReport.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}