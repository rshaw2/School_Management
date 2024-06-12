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
    /// The classtypeService responsible for managing classtype related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting classtype information.
    /// </remarks>
    public interface IClassTypeService
    {
        /// <summary>Retrieves a specific classtype by its primary key</summary>
        /// <param name="id">The primary key of the classtype</param>
        /// <returns>The classtype data</returns>
        ClassType GetById(Guid id);

        /// <summary>Retrieves a list of classtypes based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of classtypes</returns>
        List<ClassType> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new classtype</summary>
        /// <param name="model">The classtype data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(ClassType model);

        /// <summary>Updates a specific classtype by its primary key</summary>
        /// <param name="id">The primary key of the classtype</param>
        /// <param name="updatedEntity">The classtype data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, ClassType updatedEntity);

        /// <summary>Updates a specific classtype by its primary key</summary>
        /// <param name="id">The primary key of the classtype</param>
        /// <param name="updatedEntity">The classtype data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<ClassType> updatedEntity);

        /// <summary>Deletes a specific classtype by its primary key</summary>
        /// <param name="id">The primary key of the classtype</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The classtypeService responsible for managing classtype related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting classtype information.
    /// </remarks>
    public class ClassTypeService : IClassTypeService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the ClassType class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public ClassTypeService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific classtype by its primary key</summary>
        /// <param name="id">The primary key of the classtype</param>
        /// <returns>The classtype data</returns>
        public ClassType GetById(Guid id)
        {
            var entityData = _dbContext.ClassType.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of classtypes based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of classtypes</returns>/// <exception cref="Exception"></exception>
        public List<ClassType> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetClassType(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new classtype</summary>
        /// <param name="model">The classtype data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(ClassType model)
        {
            model.Id = CreateClassType(model);
            return model.Id;
        }

        /// <summary>Updates a specific classtype by its primary key</summary>
        /// <param name="id">The primary key of the classtype</param>
        /// <param name="updatedEntity">The classtype data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, ClassType updatedEntity)
        {
            UpdateClassType(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific classtype by its primary key</summary>
        /// <param name="id">The primary key of the classtype</param>
        /// <param name="updatedEntity">The classtype data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<ClassType> updatedEntity)
        {
            PatchClassType(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific classtype by its primary key</summary>
        /// <param name="id">The primary key of the classtype</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteClassType(id);
            return true;
        }
        #region
        private List<ClassType> GetClassType(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.ClassType.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<ClassType>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(ClassType), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<ClassType, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateClassType(ClassType model)
        {
            _dbContext.ClassType.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateClassType(Guid id, ClassType updatedEntity)
        {
            _dbContext.ClassType.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteClassType(Guid id)
        {
            var entityData = _dbContext.ClassType.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.ClassType.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchClassType(Guid id, JsonPatchDocument<ClassType> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.ClassType.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.ClassType.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}