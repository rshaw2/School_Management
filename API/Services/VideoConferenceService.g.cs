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
    /// The videoconferenceService responsible for managing videoconference related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting videoconference information.
    /// </remarks>
    public interface IVideoConferenceService
    {
        /// <summary>Retrieves a specific videoconference by its primary key</summary>
        /// <param name="id">The primary key of the videoconference</param>
        /// <returns>The videoconference data</returns>
        VideoConference GetById(Guid id);

        /// <summary>Retrieves a list of videoconferences based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of videoconferences</returns>
        List<VideoConference> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new videoconference</summary>
        /// <param name="model">The videoconference data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(VideoConference model);

        /// <summary>Updates a specific videoconference by its primary key</summary>
        /// <param name="id">The primary key of the videoconference</param>
        /// <param name="updatedEntity">The videoconference data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, VideoConference updatedEntity);

        /// <summary>Updates a specific videoconference by its primary key</summary>
        /// <param name="id">The primary key of the videoconference</param>
        /// <param name="updatedEntity">The videoconference data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<VideoConference> updatedEntity);

        /// <summary>Deletes a specific videoconference by its primary key</summary>
        /// <param name="id">The primary key of the videoconference</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The videoconferenceService responsible for managing videoconference related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting videoconference information.
    /// </remarks>
    public class VideoConferenceService : IVideoConferenceService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the VideoConference class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public VideoConferenceService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific videoconference by its primary key</summary>
        /// <param name="id">The primary key of the videoconference</param>
        /// <returns>The videoconference data</returns>
        public VideoConference GetById(Guid id)
        {
            var entityData = _dbContext.VideoConference.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of videoconferences based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of videoconferences</returns>/// <exception cref="Exception"></exception>
        public List<VideoConference> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetVideoConference(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new videoconference</summary>
        /// <param name="model">The videoconference data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(VideoConference model)
        {
            model.Id = CreateVideoConference(model);
            return model.Id;
        }

        /// <summary>Updates a specific videoconference by its primary key</summary>
        /// <param name="id">The primary key of the videoconference</param>
        /// <param name="updatedEntity">The videoconference data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, VideoConference updatedEntity)
        {
            UpdateVideoConference(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific videoconference by its primary key</summary>
        /// <param name="id">The primary key of the videoconference</param>
        /// <param name="updatedEntity">The videoconference data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<VideoConference> updatedEntity)
        {
            PatchVideoConference(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific videoconference by its primary key</summary>
        /// <param name="id">The primary key of the videoconference</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteVideoConference(id);
            return true;
        }
        #region
        private List<VideoConference> GetVideoConference(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.VideoConference.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<VideoConference>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(VideoConference), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<VideoConference, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateVideoConference(VideoConference model)
        {
            _dbContext.VideoConference.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateVideoConference(Guid id, VideoConference updatedEntity)
        {
            _dbContext.VideoConference.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteVideoConference(Guid id)
        {
            var entityData = _dbContext.VideoConference.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.VideoConference.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchVideoConference(Guid id, JsonPatchDocument<VideoConference> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.VideoConference.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.VideoConference.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}