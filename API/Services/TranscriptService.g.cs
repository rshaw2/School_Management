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
    /// The transcriptService responsible for managing transcript related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting transcript information.
    /// </remarks>
    public interface ITranscriptService
    {
        /// <summary>Retrieves a specific transcript by its primary key</summary>
        /// <param name="id">The primary key of the transcript</param>
        /// <returns>The transcript data</returns>
        Transcript GetById(Guid id);

        /// <summary>Retrieves a list of transcripts based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of transcripts</returns>
        List<Transcript> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new transcript</summary>
        /// <param name="model">The transcript data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Transcript model);

        /// <summary>Updates a specific transcript by its primary key</summary>
        /// <param name="id">The primary key of the transcript</param>
        /// <param name="updatedEntity">The transcript data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Transcript updatedEntity);

        /// <summary>Updates a specific transcript by its primary key</summary>
        /// <param name="id">The primary key of the transcript</param>
        /// <param name="updatedEntity">The transcript data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Transcript> updatedEntity);

        /// <summary>Deletes a specific transcript by its primary key</summary>
        /// <param name="id">The primary key of the transcript</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The transcriptService responsible for managing transcript related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting transcript information.
    /// </remarks>
    public class TranscriptService : ITranscriptService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Transcript class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public TranscriptService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific transcript by its primary key</summary>
        /// <param name="id">The primary key of the transcript</param>
        /// <returns>The transcript data</returns>
        public Transcript GetById(Guid id)
        {
            var entityData = _dbContext.Transcript.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of transcripts based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of transcripts</returns>/// <exception cref="Exception"></exception>
        public List<Transcript> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetTranscript(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new transcript</summary>
        /// <param name="model">The transcript data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Transcript model)
        {
            model.Id = CreateTranscript(model);
            return model.Id;
        }

        /// <summary>Updates a specific transcript by its primary key</summary>
        /// <param name="id">The primary key of the transcript</param>
        /// <param name="updatedEntity">The transcript data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Transcript updatedEntity)
        {
            UpdateTranscript(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific transcript by its primary key</summary>
        /// <param name="id">The primary key of the transcript</param>
        /// <param name="updatedEntity">The transcript data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Transcript> updatedEntity)
        {
            PatchTranscript(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific transcript by its primary key</summary>
        /// <param name="id">The primary key of the transcript</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteTranscript(id);
            return true;
        }
        #region
        private List<Transcript> GetTranscript(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Transcript.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Transcript>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Transcript), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Transcript, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateTranscript(Transcript model)
        {
            _dbContext.Transcript.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateTranscript(Guid id, Transcript updatedEntity)
        {
            _dbContext.Transcript.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteTranscript(Guid id)
        {
            var entityData = _dbContext.Transcript.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Transcript.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchTranscript(Guid id, JsonPatchDocument<Transcript> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Transcript.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Transcript.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}