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
    /// The roomService responsible for managing room related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting room information.
    /// </remarks>
    public interface IRoomService
    {
        /// <summary>Retrieves a specific room by its primary key</summary>
        /// <param name="id">The primary key of the room</param>
        /// <returns>The room data</returns>
        Room GetById(Guid id);

        /// <summary>Retrieves a list of rooms based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of rooms</returns>
        List<Room> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new room</summary>
        /// <param name="model">The room data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Room model);

        /// <summary>Updates a specific room by its primary key</summary>
        /// <param name="id">The primary key of the room</param>
        /// <param name="updatedEntity">The room data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Room updatedEntity);

        /// <summary>Updates a specific room by its primary key</summary>
        /// <param name="id">The primary key of the room</param>
        /// <param name="updatedEntity">The room data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Room> updatedEntity);

        /// <summary>Deletes a specific room by its primary key</summary>
        /// <param name="id">The primary key of the room</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The roomService responsible for managing room related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting room information.
    /// </remarks>
    public class RoomService : IRoomService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Room class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public RoomService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific room by its primary key</summary>
        /// <param name="id">The primary key of the room</param>
        /// <returns>The room data</returns>
        public Room GetById(Guid id)
        {
            var entityData = _dbContext.Room.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of rooms based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of rooms</returns>/// <exception cref="Exception"></exception>
        public List<Room> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetRoom(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new room</summary>
        /// <param name="model">The room data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Room model)
        {
            model.Id = CreateRoom(model);
            return model.Id;
        }

        /// <summary>Updates a specific room by its primary key</summary>
        /// <param name="id">The primary key of the room</param>
        /// <param name="updatedEntity">The room data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Room updatedEntity)
        {
            UpdateRoom(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific room by its primary key</summary>
        /// <param name="id">The primary key of the room</param>
        /// <param name="updatedEntity">The room data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Room> updatedEntity)
        {
            PatchRoom(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific room by its primary key</summary>
        /// <param name="id">The primary key of the room</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteRoom(id);
            return true;
        }
        #region
        private List<Room> GetRoom(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Room.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Room>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Room), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Room, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateRoom(Room model)
        {
            _dbContext.Room.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateRoom(Guid id, Room updatedEntity)
        {
            _dbContext.Room.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteRoom(Guid id)
        {
            var entityData = _dbContext.Room.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Room.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchRoom(Guid id, JsonPatchDocument<Room> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Room.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Room.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}