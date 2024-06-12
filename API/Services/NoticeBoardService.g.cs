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
    /// The noticeboardService responsible for managing noticeboard related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting noticeboard information.
    /// </remarks>
    public interface INoticeBoardService
    {
        /// <summary>Retrieves a specific noticeboard by its primary key</summary>
        /// <param name="id">The primary key of the noticeboard</param>
        /// <returns>The noticeboard data</returns>
        NoticeBoard GetById(Guid id);

        /// <summary>Retrieves a list of noticeboards based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of noticeboards</returns>
        List<NoticeBoard> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new noticeboard</summary>
        /// <param name="model">The noticeboard data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(NoticeBoard model);

        /// <summary>Updates a specific noticeboard by its primary key</summary>
        /// <param name="id">The primary key of the noticeboard</param>
        /// <param name="updatedEntity">The noticeboard data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, NoticeBoard updatedEntity);

        /// <summary>Updates a specific noticeboard by its primary key</summary>
        /// <param name="id">The primary key of the noticeboard</param>
        /// <param name="updatedEntity">The noticeboard data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<NoticeBoard> updatedEntity);

        /// <summary>Deletes a specific noticeboard by its primary key</summary>
        /// <param name="id">The primary key of the noticeboard</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The noticeboardService responsible for managing noticeboard related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting noticeboard information.
    /// </remarks>
    public class NoticeBoardService : INoticeBoardService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the NoticeBoard class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public NoticeBoardService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific noticeboard by its primary key</summary>
        /// <param name="id">The primary key of the noticeboard</param>
        /// <returns>The noticeboard data</returns>
        public NoticeBoard GetById(Guid id)
        {
            var entityData = _dbContext.NoticeBoard.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of noticeboards based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of noticeboards</returns>/// <exception cref="Exception"></exception>
        public List<NoticeBoard> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetNoticeBoard(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new noticeboard</summary>
        /// <param name="model">The noticeboard data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(NoticeBoard model)
        {
            model.Id = CreateNoticeBoard(model);
            return model.Id;
        }

        /// <summary>Updates a specific noticeboard by its primary key</summary>
        /// <param name="id">The primary key of the noticeboard</param>
        /// <param name="updatedEntity">The noticeboard data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, NoticeBoard updatedEntity)
        {
            UpdateNoticeBoard(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific noticeboard by its primary key</summary>
        /// <param name="id">The primary key of the noticeboard</param>
        /// <param name="updatedEntity">The noticeboard data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<NoticeBoard> updatedEntity)
        {
            PatchNoticeBoard(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific noticeboard by its primary key</summary>
        /// <param name="id">The primary key of the noticeboard</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteNoticeBoard(id);
            return true;
        }
        #region
        private List<NoticeBoard> GetNoticeBoard(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.NoticeBoard.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<NoticeBoard>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(NoticeBoard), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<NoticeBoard, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateNoticeBoard(NoticeBoard model)
        {
            _dbContext.NoticeBoard.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateNoticeBoard(Guid id, NoticeBoard updatedEntity)
        {
            _dbContext.NoticeBoard.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteNoticeBoard(Guid id)
        {
            var entityData = _dbContext.NoticeBoard.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.NoticeBoard.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchNoticeBoard(Guid id, JsonPatchDocument<NoticeBoard> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.NoticeBoard.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.NoticeBoard.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}