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
    /// The meetingpatternService responsible for managing meetingpattern related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting meetingpattern information.
    /// </remarks>
    public interface IMeetingPatternService
    {
        /// <summary>Retrieves a specific meetingpattern by its primary key</summary>
        /// <param name="id">The primary key of the meetingpattern</param>
        /// <returns>The meetingpattern data</returns>
        MeetingPattern GetById(Guid id);

        /// <summary>Retrieves a list of meetingpatterns based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of meetingpatterns</returns>
        List<MeetingPattern> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new meetingpattern</summary>
        /// <param name="model">The meetingpattern data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(MeetingPattern model);

        /// <summary>Updates a specific meetingpattern by its primary key</summary>
        /// <param name="id">The primary key of the meetingpattern</param>
        /// <param name="updatedEntity">The meetingpattern data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, MeetingPattern updatedEntity);

        /// <summary>Updates a specific meetingpattern by its primary key</summary>
        /// <param name="id">The primary key of the meetingpattern</param>
        /// <param name="updatedEntity">The meetingpattern data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<MeetingPattern> updatedEntity);

        /// <summary>Deletes a specific meetingpattern by its primary key</summary>
        /// <param name="id">The primary key of the meetingpattern</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The meetingpatternService responsible for managing meetingpattern related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting meetingpattern information.
    /// </remarks>
    public class MeetingPatternService : IMeetingPatternService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the MeetingPattern class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public MeetingPatternService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific meetingpattern by its primary key</summary>
        /// <param name="id">The primary key of the meetingpattern</param>
        /// <returns>The meetingpattern data</returns>
        public MeetingPattern GetById(Guid id)
        {
            var entityData = _dbContext.MeetingPattern.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of meetingpatterns based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of meetingpatterns</returns>/// <exception cref="Exception"></exception>
        public List<MeetingPattern> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetMeetingPattern(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new meetingpattern</summary>
        /// <param name="model">The meetingpattern data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(MeetingPattern model)
        {
            model.Id = CreateMeetingPattern(model);
            return model.Id;
        }

        /// <summary>Updates a specific meetingpattern by its primary key</summary>
        /// <param name="id">The primary key of the meetingpattern</param>
        /// <param name="updatedEntity">The meetingpattern data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, MeetingPattern updatedEntity)
        {
            UpdateMeetingPattern(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific meetingpattern by its primary key</summary>
        /// <param name="id">The primary key of the meetingpattern</param>
        /// <param name="updatedEntity">The meetingpattern data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<MeetingPattern> updatedEntity)
        {
            PatchMeetingPattern(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific meetingpattern by its primary key</summary>
        /// <param name="id">The primary key of the meetingpattern</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteMeetingPattern(id);
            return true;
        }
        #region
        private List<MeetingPattern> GetMeetingPattern(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.MeetingPattern.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<MeetingPattern>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(MeetingPattern), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<MeetingPattern, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateMeetingPattern(MeetingPattern model)
        {
            _dbContext.MeetingPattern.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateMeetingPattern(Guid id, MeetingPattern updatedEntity)
        {
            _dbContext.MeetingPattern.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteMeetingPattern(Guid id)
        {
            var entityData = _dbContext.MeetingPattern.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.MeetingPattern.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchMeetingPattern(Guid id, JsonPatchDocument<MeetingPattern> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.MeetingPattern.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.MeetingPattern.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}