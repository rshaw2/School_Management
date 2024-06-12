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
    /// The studentdocumentsService responsible for managing studentdocuments related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting studentdocuments information.
    /// </remarks>
    public interface IStudentDocumentsService
    {
        /// <summary>Retrieves a specific studentdocuments by its primary key</summary>
        /// <param name="id">The primary key of the studentdocuments</param>
        /// <returns>The studentdocuments data</returns>
        StudentDocuments GetById(Guid id);

        /// <summary>Retrieves a list of studentdocumentss based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of studentdocumentss</returns>
        List<StudentDocuments> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new studentdocuments</summary>
        /// <param name="model">The studentdocuments data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(StudentDocuments model);

        /// <summary>Updates a specific studentdocuments by its primary key</summary>
        /// <param name="id">The primary key of the studentdocuments</param>
        /// <param name="updatedEntity">The studentdocuments data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, StudentDocuments updatedEntity);

        /// <summary>Updates a specific studentdocuments by its primary key</summary>
        /// <param name="id">The primary key of the studentdocuments</param>
        /// <param name="updatedEntity">The studentdocuments data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<StudentDocuments> updatedEntity);

        /// <summary>Deletes a specific studentdocuments by its primary key</summary>
        /// <param name="id">The primary key of the studentdocuments</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The studentdocumentsService responsible for managing studentdocuments related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting studentdocuments information.
    /// </remarks>
    public class StudentDocumentsService : IStudentDocumentsService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the StudentDocuments class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public StudentDocumentsService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific studentdocuments by its primary key</summary>
        /// <param name="id">The primary key of the studentdocuments</param>
        /// <returns>The studentdocuments data</returns>
        public StudentDocuments GetById(Guid id)
        {
            var entityData = _dbContext.StudentDocuments.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of studentdocumentss based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of studentdocumentss</returns>/// <exception cref="Exception"></exception>
        public List<StudentDocuments> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetStudentDocuments(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new studentdocuments</summary>
        /// <param name="model">The studentdocuments data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(StudentDocuments model)
        {
            model.Id = CreateStudentDocuments(model);
            return model.Id;
        }

        /// <summary>Updates a specific studentdocuments by its primary key</summary>
        /// <param name="id">The primary key of the studentdocuments</param>
        /// <param name="updatedEntity">The studentdocuments data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, StudentDocuments updatedEntity)
        {
            UpdateStudentDocuments(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific studentdocuments by its primary key</summary>
        /// <param name="id">The primary key of the studentdocuments</param>
        /// <param name="updatedEntity">The studentdocuments data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<StudentDocuments> updatedEntity)
        {
            PatchStudentDocuments(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific studentdocuments by its primary key</summary>
        /// <param name="id">The primary key of the studentdocuments</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteStudentDocuments(id);
            return true;
        }
        #region
        private List<StudentDocuments> GetStudentDocuments(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.StudentDocuments.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<StudentDocuments>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(StudentDocuments), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<StudentDocuments, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateStudentDocuments(StudentDocuments model)
        {
            _dbContext.StudentDocuments.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateStudentDocuments(Guid id, StudentDocuments updatedEntity)
        {
            _dbContext.StudentDocuments.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteStudentDocuments(Guid id)
        {
            var entityData = _dbContext.StudentDocuments.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.StudentDocuments.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchStudentDocuments(Guid id, JsonPatchDocument<StudentDocuments> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.StudentDocuments.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.StudentDocuments.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}