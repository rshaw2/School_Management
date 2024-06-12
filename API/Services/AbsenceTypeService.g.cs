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
    /// The absencetypeService responsible for managing absencetype related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting absencetype information.
    /// </remarks>
    public interface IAbsenceTypeService
    {
        /// <summary>Retrieves a specific absencetype by its primary key</summary>
        /// <param name="id">The primary key of the absencetype</param>
        /// <returns>The absencetype data</returns>
        AbsenceType GetById(Guid id);

        /// <summary>Retrieves a list of absencetypes based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of absencetypes</returns>
        List<AbsenceType> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new absencetype</summary>
        /// <param name="model">The absencetype data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(AbsenceType model);

        /// <summary>Updates a specific absencetype by its primary key</summary>
        /// <param name="id">The primary key of the absencetype</param>
        /// <param name="updatedEntity">The absencetype data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, AbsenceType updatedEntity);

        /// <summary>Updates a specific absencetype by its primary key</summary>
        /// <param name="id">The primary key of the absencetype</param>
        /// <param name="updatedEntity">The absencetype data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<AbsenceType> updatedEntity);

        /// <summary>Deletes a specific absencetype by its primary key</summary>
        /// <param name="id">The primary key of the absencetype</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The absencetypeService responsible for managing absencetype related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting absencetype information.
    /// </remarks>
    public class AbsenceTypeService : IAbsenceTypeService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the AbsenceType class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public AbsenceTypeService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific absencetype by its primary key</summary>
        /// <param name="id">The primary key of the absencetype</param>
        /// <returns>The absencetype data</returns>
        public AbsenceType GetById(Guid id)
        {
            var entityData = _dbContext.AbsenceType.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of absencetypes based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of absencetypes</returns>/// <exception cref="Exception"></exception>
        public List<AbsenceType> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetAbsenceType(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new absencetype</summary>
        /// <param name="model">The absencetype data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(AbsenceType model)
        {
            model.Id = CreateAbsenceType(model);
            return model.Id;
        }

        /// <summary>Updates a specific absencetype by its primary key</summary>
        /// <param name="id">The primary key of the absencetype</param>
        /// <param name="updatedEntity">The absencetype data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, AbsenceType updatedEntity)
        {
            UpdateAbsenceType(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific absencetype by its primary key</summary>
        /// <param name="id">The primary key of the absencetype</param>
        /// <param name="updatedEntity">The absencetype data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<AbsenceType> updatedEntity)
        {
            PatchAbsenceType(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific absencetype by its primary key</summary>
        /// <param name="id">The primary key of the absencetype</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteAbsenceType(id);
            return true;
        }
        #region
        private List<AbsenceType> GetAbsenceType(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.AbsenceType.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<AbsenceType>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(AbsenceType), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<AbsenceType, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateAbsenceType(AbsenceType model)
        {
            _dbContext.AbsenceType.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateAbsenceType(Guid id, AbsenceType updatedEntity)
        {
            _dbContext.AbsenceType.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteAbsenceType(Guid id)
        {
            var entityData = _dbContext.AbsenceType.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.AbsenceType.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchAbsenceType(Guid id, JsonPatchDocument<AbsenceType> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.AbsenceType.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.AbsenceType.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}