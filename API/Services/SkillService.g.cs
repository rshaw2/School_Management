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
    /// The skillService responsible for managing skill related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting skill information.
    /// </remarks>
    public interface ISkillService
    {
        /// <summary>Retrieves a specific skill by its primary key</summary>
        /// <param name="id">The primary key of the skill</param>
        /// <returns>The skill data</returns>
        Skill GetById(Guid id);

        /// <summary>Retrieves a list of skills based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of skills</returns>
        List<Skill> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new skill</summary>
        /// <param name="model">The skill data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Skill model);

        /// <summary>Updates a specific skill by its primary key</summary>
        /// <param name="id">The primary key of the skill</param>
        /// <param name="updatedEntity">The skill data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Skill updatedEntity);

        /// <summary>Updates a specific skill by its primary key</summary>
        /// <param name="id">The primary key of the skill</param>
        /// <param name="updatedEntity">The skill data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Skill> updatedEntity);

        /// <summary>Deletes a specific skill by its primary key</summary>
        /// <param name="id">The primary key of the skill</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The skillService responsible for managing skill related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting skill information.
    /// </remarks>
    public class SkillService : ISkillService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Skill class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public SkillService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific skill by its primary key</summary>
        /// <param name="id">The primary key of the skill</param>
        /// <returns>The skill data</returns>
        public Skill GetById(Guid id)
        {
            var entityData = _dbContext.Skill.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of skills based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of skills</returns>/// <exception cref="Exception"></exception>
        public List<Skill> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetSkill(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new skill</summary>
        /// <param name="model">The skill data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Skill model)
        {
            model.Id = CreateSkill(model);
            return model.Id;
        }

        /// <summary>Updates a specific skill by its primary key</summary>
        /// <param name="id">The primary key of the skill</param>
        /// <param name="updatedEntity">The skill data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Skill updatedEntity)
        {
            UpdateSkill(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific skill by its primary key</summary>
        /// <param name="id">The primary key of the skill</param>
        /// <param name="updatedEntity">The skill data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Skill> updatedEntity)
        {
            PatchSkill(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific skill by its primary key</summary>
        /// <param name="id">The primary key of the skill</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteSkill(id);
            return true;
        }
        #region
        private List<Skill> GetSkill(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Skill.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Skill>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Skill), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Skill, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateSkill(Skill model)
        {
            _dbContext.Skill.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateSkill(Guid id, Skill updatedEntity)
        {
            _dbContext.Skill.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteSkill(Guid id)
        {
            var entityData = _dbContext.Skill.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Skill.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchSkill(Guid id, JsonPatchDocument<Skill> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Skill.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Skill.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}