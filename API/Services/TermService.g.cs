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
    /// The termService responsible for managing term related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting term information.
    /// </remarks>
    public interface ITermService
    {
        /// <summary>Retrieves a specific term by its primary key</summary>
        /// <param name="id">The primary key of the term</param>
        /// <returns>The term data</returns>
        Term GetById(Guid id);

        /// <summary>Retrieves a list of terms based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of terms</returns>
        List<Term> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new term</summary>
        /// <param name="model">The term data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Term model);

        /// <summary>Updates a specific term by its primary key</summary>
        /// <param name="id">The primary key of the term</param>
        /// <param name="updatedEntity">The term data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Term updatedEntity);

        /// <summary>Updates a specific term by its primary key</summary>
        /// <param name="id">The primary key of the term</param>
        /// <param name="updatedEntity">The term data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Term> updatedEntity);

        /// <summary>Deletes a specific term by its primary key</summary>
        /// <param name="id">The primary key of the term</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The termService responsible for managing term related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting term information.
    /// </remarks>
    public class TermService : ITermService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Term class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public TermService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific term by its primary key</summary>
        /// <param name="id">The primary key of the term</param>
        /// <returns>The term data</returns>
        public Term GetById(Guid id)
        {
            var entityData = _dbContext.Term.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of terms based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of terms</returns>/// <exception cref="Exception"></exception>
        public List<Term> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetTerm(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new term</summary>
        /// <param name="model">The term data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Term model)
        {
            model.Id = CreateTerm(model);
            return model.Id;
        }

        /// <summary>Updates a specific term by its primary key</summary>
        /// <param name="id">The primary key of the term</param>
        /// <param name="updatedEntity">The term data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Term updatedEntity)
        {
            UpdateTerm(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific term by its primary key</summary>
        /// <param name="id">The primary key of the term</param>
        /// <param name="updatedEntity">The term data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Term> updatedEntity)
        {
            PatchTerm(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific term by its primary key</summary>
        /// <param name="id">The primary key of the term</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteTerm(id);
            return true;
        }
        #region
        private List<Term> GetTerm(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Term.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Term>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Term), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Term, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateTerm(Term model)
        {
            _dbContext.Term.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateTerm(Guid id, Term updatedEntity)
        {
            _dbContext.Term.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteTerm(Guid id)
        {
            var entityData = _dbContext.Term.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Term.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchTerm(Guid id, JsonPatchDocument<Term> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Term.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Term.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}