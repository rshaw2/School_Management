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
    /// The agendaService responsible for managing agenda related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting agenda information.
    /// </remarks>
    public interface IAgendaService
    {
        /// <summary>Retrieves a specific agenda by its primary key</summary>
        /// <param name="id">The primary key of the agenda</param>
        /// <returns>The agenda data</returns>
        Agenda GetById(Guid id);

        /// <summary>Retrieves a list of agendas based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of agendas</returns>
        List<Agenda> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new agenda</summary>
        /// <param name="model">The agenda data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Agenda model);

        /// <summary>Updates a specific agenda by its primary key</summary>
        /// <param name="id">The primary key of the agenda</param>
        /// <param name="updatedEntity">The agenda data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Agenda updatedEntity);

        /// <summary>Updates a specific agenda by its primary key</summary>
        /// <param name="id">The primary key of the agenda</param>
        /// <param name="updatedEntity">The agenda data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Agenda> updatedEntity);

        /// <summary>Deletes a specific agenda by its primary key</summary>
        /// <param name="id">The primary key of the agenda</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The agendaService responsible for managing agenda related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting agenda information.
    /// </remarks>
    public class AgendaService : IAgendaService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Agenda class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public AgendaService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific agenda by its primary key</summary>
        /// <param name="id">The primary key of the agenda</param>
        /// <returns>The agenda data</returns>
        public Agenda GetById(Guid id)
        {
            var entityData = _dbContext.Agenda.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of agendas based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of agendas</returns>/// <exception cref="Exception"></exception>
        public List<Agenda> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetAgenda(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new agenda</summary>
        /// <param name="model">The agenda data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Agenda model)
        {
            model.Id = CreateAgenda(model);
            return model.Id;
        }

        /// <summary>Updates a specific agenda by its primary key</summary>
        /// <param name="id">The primary key of the agenda</param>
        /// <param name="updatedEntity">The agenda data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Agenda updatedEntity)
        {
            UpdateAgenda(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific agenda by its primary key</summary>
        /// <param name="id">The primary key of the agenda</param>
        /// <param name="updatedEntity">The agenda data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Agenda> updatedEntity)
        {
            PatchAgenda(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific agenda by its primary key</summary>
        /// <param name="id">The primary key of the agenda</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteAgenda(id);
            return true;
        }
        #region
        private List<Agenda> GetAgenda(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Agenda.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Agenda>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Agenda), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Agenda, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateAgenda(Agenda model)
        {
            _dbContext.Agenda.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateAgenda(Guid id, Agenda updatedEntity)
        {
            _dbContext.Agenda.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteAgenda(Guid id)
        {
            var entityData = _dbContext.Agenda.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Agenda.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchAgenda(Guid id, JsonPatchDocument<Agenda> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Agenda.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Agenda.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}