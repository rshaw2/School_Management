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
    /// The knowledgebaseService responsible for managing knowledgebase related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting knowledgebase information.
    /// </remarks>
    public interface IKnowledgeBaseService
    {
        /// <summary>Retrieves a specific knowledgebase by its primary key</summary>
        /// <param name="id">The primary key of the knowledgebase</param>
        /// <returns>The knowledgebase data</returns>
        KnowledgeBase GetById(Guid id);

        /// <summary>Retrieves a list of knowledgebases based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of knowledgebases</returns>
        List<KnowledgeBase> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new knowledgebase</summary>
        /// <param name="model">The knowledgebase data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(KnowledgeBase model);

        /// <summary>Updates a specific knowledgebase by its primary key</summary>
        /// <param name="id">The primary key of the knowledgebase</param>
        /// <param name="updatedEntity">The knowledgebase data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, KnowledgeBase updatedEntity);

        /// <summary>Updates a specific knowledgebase by its primary key</summary>
        /// <param name="id">The primary key of the knowledgebase</param>
        /// <param name="updatedEntity">The knowledgebase data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<KnowledgeBase> updatedEntity);

        /// <summary>Deletes a specific knowledgebase by its primary key</summary>
        /// <param name="id">The primary key of the knowledgebase</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The knowledgebaseService responsible for managing knowledgebase related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting knowledgebase information.
    /// </remarks>
    public class KnowledgeBaseService : IKnowledgeBaseService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the KnowledgeBase class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public KnowledgeBaseService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific knowledgebase by its primary key</summary>
        /// <param name="id">The primary key of the knowledgebase</param>
        /// <returns>The knowledgebase data</returns>
        public KnowledgeBase GetById(Guid id)
        {
            var entityData = _dbContext.KnowledgeBase.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of knowledgebases based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of knowledgebases</returns>/// <exception cref="Exception"></exception>
        public List<KnowledgeBase> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetKnowledgeBase(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new knowledgebase</summary>
        /// <param name="model">The knowledgebase data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(KnowledgeBase model)
        {
            model.Id = CreateKnowledgeBase(model);
            return model.Id;
        }

        /// <summary>Updates a specific knowledgebase by its primary key</summary>
        /// <param name="id">The primary key of the knowledgebase</param>
        /// <param name="updatedEntity">The knowledgebase data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, KnowledgeBase updatedEntity)
        {
            UpdateKnowledgeBase(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific knowledgebase by its primary key</summary>
        /// <param name="id">The primary key of the knowledgebase</param>
        /// <param name="updatedEntity">The knowledgebase data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<KnowledgeBase> updatedEntity)
        {
            PatchKnowledgeBase(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific knowledgebase by its primary key</summary>
        /// <param name="id">The primary key of the knowledgebase</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteKnowledgeBase(id);
            return true;
        }
        #region
        private List<KnowledgeBase> GetKnowledgeBase(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.KnowledgeBase.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<KnowledgeBase>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(KnowledgeBase), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<KnowledgeBase, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateKnowledgeBase(KnowledgeBase model)
        {
            _dbContext.KnowledgeBase.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateKnowledgeBase(Guid id, KnowledgeBase updatedEntity)
        {
            _dbContext.KnowledgeBase.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteKnowledgeBase(Guid id)
        {
            var entityData = _dbContext.KnowledgeBase.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.KnowledgeBase.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchKnowledgeBase(Guid id, JsonPatchDocument<KnowledgeBase> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.KnowledgeBase.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.KnowledgeBase.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}