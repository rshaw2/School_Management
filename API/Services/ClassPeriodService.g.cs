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
    /// The classperiodService responsible for managing classperiod related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting classperiod information.
    /// </remarks>
    public interface IClassPeriodService
    {
        /// <summary>Retrieves a specific classperiod by its primary key</summary>
        /// <param name="id">The primary key of the classperiod</param>
        /// <returns>The classperiod data</returns>
        ClassPeriod GetById(Guid id);

        /// <summary>Retrieves a list of classperiods based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of classperiods</returns>
        List<ClassPeriod> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new classperiod</summary>
        /// <param name="model">The classperiod data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(ClassPeriod model);

        /// <summary>Updates a specific classperiod by its primary key</summary>
        /// <param name="id">The primary key of the classperiod</param>
        /// <param name="updatedEntity">The classperiod data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, ClassPeriod updatedEntity);

        /// <summary>Updates a specific classperiod by its primary key</summary>
        /// <param name="id">The primary key of the classperiod</param>
        /// <param name="updatedEntity">The classperiod data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<ClassPeriod> updatedEntity);

        /// <summary>Deletes a specific classperiod by its primary key</summary>
        /// <param name="id">The primary key of the classperiod</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The classperiodService responsible for managing classperiod related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting classperiod information.
    /// </remarks>
    public class ClassPeriodService : IClassPeriodService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the ClassPeriod class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public ClassPeriodService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific classperiod by its primary key</summary>
        /// <param name="id">The primary key of the classperiod</param>
        /// <returns>The classperiod data</returns>
        public ClassPeriod GetById(Guid id)
        {
            var entityData = _dbContext.ClassPeriod.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of classperiods based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of classperiods</returns>/// <exception cref="Exception"></exception>
        public List<ClassPeriod> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetClassPeriod(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new classperiod</summary>
        /// <param name="model">The classperiod data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(ClassPeriod model)
        {
            model.Id = CreateClassPeriod(model);
            return model.Id;
        }

        /// <summary>Updates a specific classperiod by its primary key</summary>
        /// <param name="id">The primary key of the classperiod</param>
        /// <param name="updatedEntity">The classperiod data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, ClassPeriod updatedEntity)
        {
            UpdateClassPeriod(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific classperiod by its primary key</summary>
        /// <param name="id">The primary key of the classperiod</param>
        /// <param name="updatedEntity">The classperiod data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<ClassPeriod> updatedEntity)
        {
            PatchClassPeriod(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific classperiod by its primary key</summary>
        /// <param name="id">The primary key of the classperiod</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteClassPeriod(id);
            return true;
        }
        #region
        private List<ClassPeriod> GetClassPeriod(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.ClassPeriod.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<ClassPeriod>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(ClassPeriod), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<ClassPeriod, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateClassPeriod(ClassPeriod model)
        {
            _dbContext.ClassPeriod.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateClassPeriod(Guid id, ClassPeriod updatedEntity)
        {
            _dbContext.ClassPeriod.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteClassPeriod(Guid id)
        {
            var entityData = _dbContext.ClassPeriod.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.ClassPeriod.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchClassPeriod(Guid id, JsonPatchDocument<ClassPeriod> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.ClassPeriod.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.ClassPeriod.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}