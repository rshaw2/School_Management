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
    /// The timetabletemplateService responsible for managing timetabletemplate related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting timetabletemplate information.
    /// </remarks>
    public interface ITimetableTemplateService
    {
        /// <summary>Retrieves a specific timetabletemplate by its primary key</summary>
        /// <param name="id">The primary key of the timetabletemplate</param>
        /// <returns>The timetabletemplate data</returns>
        TimetableTemplate GetById(Guid id);

        /// <summary>Retrieves a list of timetabletemplates based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of timetabletemplates</returns>
        List<TimetableTemplate> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new timetabletemplate</summary>
        /// <param name="model">The timetabletemplate data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(TimetableTemplate model);

        /// <summary>Updates a specific timetabletemplate by its primary key</summary>
        /// <param name="id">The primary key of the timetabletemplate</param>
        /// <param name="updatedEntity">The timetabletemplate data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, TimetableTemplate updatedEntity);

        /// <summary>Updates a specific timetabletemplate by its primary key</summary>
        /// <param name="id">The primary key of the timetabletemplate</param>
        /// <param name="updatedEntity">The timetabletemplate data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<TimetableTemplate> updatedEntity);

        /// <summary>Deletes a specific timetabletemplate by its primary key</summary>
        /// <param name="id">The primary key of the timetabletemplate</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The timetabletemplateService responsible for managing timetabletemplate related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting timetabletemplate information.
    /// </remarks>
    public class TimetableTemplateService : ITimetableTemplateService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the TimetableTemplate class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public TimetableTemplateService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific timetabletemplate by its primary key</summary>
        /// <param name="id">The primary key of the timetabletemplate</param>
        /// <returns>The timetabletemplate data</returns>
        public TimetableTemplate GetById(Guid id)
        {
            var entityData = _dbContext.TimetableTemplate.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of timetabletemplates based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of timetabletemplates</returns>/// <exception cref="Exception"></exception>
        public List<TimetableTemplate> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetTimetableTemplate(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new timetabletemplate</summary>
        /// <param name="model">The timetabletemplate data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(TimetableTemplate model)
        {
            model.Id = CreateTimetableTemplate(model);
            return model.Id;
        }

        /// <summary>Updates a specific timetabletemplate by its primary key</summary>
        /// <param name="id">The primary key of the timetabletemplate</param>
        /// <param name="updatedEntity">The timetabletemplate data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, TimetableTemplate updatedEntity)
        {
            UpdateTimetableTemplate(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific timetabletemplate by its primary key</summary>
        /// <param name="id">The primary key of the timetabletemplate</param>
        /// <param name="updatedEntity">The timetabletemplate data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<TimetableTemplate> updatedEntity)
        {
            PatchTimetableTemplate(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific timetabletemplate by its primary key</summary>
        /// <param name="id">The primary key of the timetabletemplate</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteTimetableTemplate(id);
            return true;
        }
        #region
        private List<TimetableTemplate> GetTimetableTemplate(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.TimetableTemplate.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<TimetableTemplate>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(TimetableTemplate), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<TimetableTemplate, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateTimetableTemplate(TimetableTemplate model)
        {
            _dbContext.TimetableTemplate.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateTimetableTemplate(Guid id, TimetableTemplate updatedEntity)
        {
            _dbContext.TimetableTemplate.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteTimetableTemplate(Guid id)
        {
            var entityData = _dbContext.TimetableTemplate.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.TimetableTemplate.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchTimetableTemplate(Guid id, JsonPatchDocument<TimetableTemplate> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.TimetableTemplate.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.TimetableTemplate.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}