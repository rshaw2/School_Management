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
    /// The trainingService responsible for managing training related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting training information.
    /// </remarks>
    public interface ITrainingService
    {
        /// <summary>Retrieves a specific training by its primary key</summary>
        /// <param name="id">The primary key of the training</param>
        /// <returns>The training data</returns>
        Training GetById(Guid id);

        /// <summary>Retrieves a list of trainings based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of trainings</returns>
        List<Training> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new training</summary>
        /// <param name="model">The training data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Training model);

        /// <summary>Updates a specific training by its primary key</summary>
        /// <param name="id">The primary key of the training</param>
        /// <param name="updatedEntity">The training data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Training updatedEntity);

        /// <summary>Updates a specific training by its primary key</summary>
        /// <param name="id">The primary key of the training</param>
        /// <param name="updatedEntity">The training data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Training> updatedEntity);

        /// <summary>Deletes a specific training by its primary key</summary>
        /// <param name="id">The primary key of the training</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The trainingService responsible for managing training related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting training information.
    /// </remarks>
    public class TrainingService : ITrainingService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Training class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public TrainingService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific training by its primary key</summary>
        /// <param name="id">The primary key of the training</param>
        /// <returns>The training data</returns>
        public Training GetById(Guid id)
        {
            var entityData = _dbContext.Training.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of trainings based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of trainings</returns>/// <exception cref="Exception"></exception>
        public List<Training> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetTraining(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new training</summary>
        /// <param name="model">The training data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Training model)
        {
            model.Id = CreateTraining(model);
            return model.Id;
        }

        /// <summary>Updates a specific training by its primary key</summary>
        /// <param name="id">The primary key of the training</param>
        /// <param name="updatedEntity">The training data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Training updatedEntity)
        {
            UpdateTraining(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific training by its primary key</summary>
        /// <param name="id">The primary key of the training</param>
        /// <param name="updatedEntity">The training data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Training> updatedEntity)
        {
            PatchTraining(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific training by its primary key</summary>
        /// <param name="id">The primary key of the training</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteTraining(id);
            return true;
        }
        #region
        private List<Training> GetTraining(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Training.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Training>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Training), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Training, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateTraining(Training model)
        {
            _dbContext.Training.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateTraining(Guid id, Training updatedEntity)
        {
            _dbContext.Training.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteTraining(Guid id)
        {
            var entityData = _dbContext.Training.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Training.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchTraining(Guid id, JsonPatchDocument<Training> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Training.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Training.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}