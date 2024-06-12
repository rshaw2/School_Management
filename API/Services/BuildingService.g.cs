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
    /// The buildingService responsible for managing building related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting building information.
    /// </remarks>
    public interface IBuildingService
    {
        /// <summary>Retrieves a specific building by its primary key</summary>
        /// <param name="id">The primary key of the building</param>
        /// <returns>The building data</returns>
        Building GetById(Guid id);

        /// <summary>Retrieves a list of buildings based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of buildings</returns>
        List<Building> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new building</summary>
        /// <param name="model">The building data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Building model);

        /// <summary>Updates a specific building by its primary key</summary>
        /// <param name="id">The primary key of the building</param>
        /// <param name="updatedEntity">The building data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Building updatedEntity);

        /// <summary>Updates a specific building by its primary key</summary>
        /// <param name="id">The primary key of the building</param>
        /// <param name="updatedEntity">The building data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Building> updatedEntity);

        /// <summary>Deletes a specific building by its primary key</summary>
        /// <param name="id">The primary key of the building</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The buildingService responsible for managing building related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting building information.
    /// </remarks>
    public class BuildingService : IBuildingService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Building class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public BuildingService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific building by its primary key</summary>
        /// <param name="id">The primary key of the building</param>
        /// <returns>The building data</returns>
        public Building GetById(Guid id)
        {
            var entityData = _dbContext.Building.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of buildings based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of buildings</returns>/// <exception cref="Exception"></exception>
        public List<Building> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetBuilding(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new building</summary>
        /// <param name="model">The building data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Building model)
        {
            model.Id = CreateBuilding(model);
            return model.Id;
        }

        /// <summary>Updates a specific building by its primary key</summary>
        /// <param name="id">The primary key of the building</param>
        /// <param name="updatedEntity">The building data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Building updatedEntity)
        {
            UpdateBuilding(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific building by its primary key</summary>
        /// <param name="id">The primary key of the building</param>
        /// <param name="updatedEntity">The building data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Building> updatedEntity)
        {
            PatchBuilding(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific building by its primary key</summary>
        /// <param name="id">The primary key of the building</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteBuilding(id);
            return true;
        }
        #region
        private List<Building> GetBuilding(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Building.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Building>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Building), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Building, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateBuilding(Building model)
        {
            _dbContext.Building.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateBuilding(Guid id, Building updatedEntity)
        {
            _dbContext.Building.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteBuilding(Guid id)
        {
            var entityData = _dbContext.Building.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Building.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchBuilding(Guid id, JsonPatchDocument<Building> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Building.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Building.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}