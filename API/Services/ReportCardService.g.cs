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
    /// The reportcardService responsible for managing reportcard related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting reportcard information.
    /// </remarks>
    public interface IReportCardService
    {
        /// <summary>Retrieves a specific reportcard by its primary key</summary>
        /// <param name="id">The primary key of the reportcard</param>
        /// <returns>The reportcard data</returns>
        ReportCard GetById(Guid id);

        /// <summary>Retrieves a list of reportcards based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of reportcards</returns>
        List<ReportCard> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new reportcard</summary>
        /// <param name="model">The reportcard data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(ReportCard model);

        /// <summary>Updates a specific reportcard by its primary key</summary>
        /// <param name="id">The primary key of the reportcard</param>
        /// <param name="updatedEntity">The reportcard data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, ReportCard updatedEntity);

        /// <summary>Updates a specific reportcard by its primary key</summary>
        /// <param name="id">The primary key of the reportcard</param>
        /// <param name="updatedEntity">The reportcard data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<ReportCard> updatedEntity);

        /// <summary>Deletes a specific reportcard by its primary key</summary>
        /// <param name="id">The primary key of the reportcard</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The reportcardService responsible for managing reportcard related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting reportcard information.
    /// </remarks>
    public class ReportCardService : IReportCardService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the ReportCard class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public ReportCardService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific reportcard by its primary key</summary>
        /// <param name="id">The primary key of the reportcard</param>
        /// <returns>The reportcard data</returns>
        public ReportCard GetById(Guid id)
        {
            var entityData = _dbContext.ReportCard.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of reportcards based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of reportcards</returns>/// <exception cref="Exception"></exception>
        public List<ReportCard> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetReportCard(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new reportcard</summary>
        /// <param name="model">The reportcard data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(ReportCard model)
        {
            model.Id = CreateReportCard(model);
            return model.Id;
        }

        /// <summary>Updates a specific reportcard by its primary key</summary>
        /// <param name="id">The primary key of the reportcard</param>
        /// <param name="updatedEntity">The reportcard data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, ReportCard updatedEntity)
        {
            UpdateReportCard(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific reportcard by its primary key</summary>
        /// <param name="id">The primary key of the reportcard</param>
        /// <param name="updatedEntity">The reportcard data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<ReportCard> updatedEntity)
        {
            PatchReportCard(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific reportcard by its primary key</summary>
        /// <param name="id">The primary key of the reportcard</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteReportCard(id);
            return true;
        }
        #region
        private List<ReportCard> GetReportCard(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.ReportCard.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<ReportCard>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(ReportCard), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<ReportCard, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateReportCard(ReportCard model)
        {
            _dbContext.ReportCard.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateReportCard(Guid id, ReportCard updatedEntity)
        {
            _dbContext.ReportCard.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteReportCard(Guid id)
        {
            var entityData = _dbContext.ReportCard.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.ReportCard.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchReportCard(Guid id, JsonPatchDocument<ReportCard> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.ReportCard.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.ReportCard.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}