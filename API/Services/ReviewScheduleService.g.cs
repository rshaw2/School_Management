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
    /// The reviewscheduleService responsible for managing reviewschedule related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting reviewschedule information.
    /// </remarks>
    public interface IReviewScheduleService
    {
        /// <summary>Retrieves a specific reviewschedule by its primary key</summary>
        /// <param name="id">The primary key of the reviewschedule</param>
        /// <returns>The reviewschedule data</returns>
        ReviewSchedule GetById(Guid id);

        /// <summary>Retrieves a list of reviewschedules based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of reviewschedules</returns>
        List<ReviewSchedule> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new reviewschedule</summary>
        /// <param name="model">The reviewschedule data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(ReviewSchedule model);

        /// <summary>Updates a specific reviewschedule by its primary key</summary>
        /// <param name="id">The primary key of the reviewschedule</param>
        /// <param name="updatedEntity">The reviewschedule data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, ReviewSchedule updatedEntity);

        /// <summary>Updates a specific reviewschedule by its primary key</summary>
        /// <param name="id">The primary key of the reviewschedule</param>
        /// <param name="updatedEntity">The reviewschedule data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<ReviewSchedule> updatedEntity);

        /// <summary>Deletes a specific reviewschedule by its primary key</summary>
        /// <param name="id">The primary key of the reviewschedule</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The reviewscheduleService responsible for managing reviewschedule related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting reviewschedule information.
    /// </remarks>
    public class ReviewScheduleService : IReviewScheduleService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the ReviewSchedule class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public ReviewScheduleService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific reviewschedule by its primary key</summary>
        /// <param name="id">The primary key of the reviewschedule</param>
        /// <returns>The reviewschedule data</returns>
        public ReviewSchedule GetById(Guid id)
        {
            var entityData = _dbContext.ReviewSchedule.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of reviewschedules based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of reviewschedules</returns>/// <exception cref="Exception"></exception>
        public List<ReviewSchedule> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetReviewSchedule(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new reviewschedule</summary>
        /// <param name="model">The reviewschedule data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(ReviewSchedule model)
        {
            model.Id = CreateReviewSchedule(model);
            return model.Id;
        }

        /// <summary>Updates a specific reviewschedule by its primary key</summary>
        /// <param name="id">The primary key of the reviewschedule</param>
        /// <param name="updatedEntity">The reviewschedule data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, ReviewSchedule updatedEntity)
        {
            UpdateReviewSchedule(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific reviewschedule by its primary key</summary>
        /// <param name="id">The primary key of the reviewschedule</param>
        /// <param name="updatedEntity">The reviewschedule data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<ReviewSchedule> updatedEntity)
        {
            PatchReviewSchedule(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific reviewschedule by its primary key</summary>
        /// <param name="id">The primary key of the reviewschedule</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteReviewSchedule(id);
            return true;
        }
        #region
        private List<ReviewSchedule> GetReviewSchedule(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.ReviewSchedule.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<ReviewSchedule>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(ReviewSchedule), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<ReviewSchedule, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateReviewSchedule(ReviewSchedule model)
        {
            _dbContext.ReviewSchedule.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateReviewSchedule(Guid id, ReviewSchedule updatedEntity)
        {
            _dbContext.ReviewSchedule.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteReviewSchedule(Guid id)
        {
            var entityData = _dbContext.ReviewSchedule.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.ReviewSchedule.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchReviewSchedule(Guid id, JsonPatchDocument<ReviewSchedule> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.ReviewSchedule.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.ReviewSchedule.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}