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
    /// The certificateService responsible for managing certificate related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting certificate information.
    /// </remarks>
    public interface ICertificateService
    {
        /// <summary>Retrieves a specific certificate by its primary key</summary>
        /// <param name="id">The primary key of the certificate</param>
        /// <returns>The certificate data</returns>
        Certificate GetById(Guid id);

        /// <summary>Retrieves a list of certificates based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of certificates</returns>
        List<Certificate> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new certificate</summary>
        /// <param name="model">The certificate data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Certificate model);

        /// <summary>Updates a specific certificate by its primary key</summary>
        /// <param name="id">The primary key of the certificate</param>
        /// <param name="updatedEntity">The certificate data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Certificate updatedEntity);

        /// <summary>Updates a specific certificate by its primary key</summary>
        /// <param name="id">The primary key of the certificate</param>
        /// <param name="updatedEntity">The certificate data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Certificate> updatedEntity);

        /// <summary>Deletes a specific certificate by its primary key</summary>
        /// <param name="id">The primary key of the certificate</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The certificateService responsible for managing certificate related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting certificate information.
    /// </remarks>
    public class CertificateService : ICertificateService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Certificate class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public CertificateService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific certificate by its primary key</summary>
        /// <param name="id">The primary key of the certificate</param>
        /// <returns>The certificate data</returns>
        public Certificate GetById(Guid id)
        {
            var entityData = _dbContext.Certificate.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of certificates based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of certificates</returns>/// <exception cref="Exception"></exception>
        public List<Certificate> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetCertificate(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new certificate</summary>
        /// <param name="model">The certificate data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Certificate model)
        {
            model.Id = CreateCertificate(model);
            return model.Id;
        }

        /// <summary>Updates a specific certificate by its primary key</summary>
        /// <param name="id">The primary key of the certificate</param>
        /// <param name="updatedEntity">The certificate data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Certificate updatedEntity)
        {
            UpdateCertificate(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific certificate by its primary key</summary>
        /// <param name="id">The primary key of the certificate</param>
        /// <param name="updatedEntity">The certificate data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Certificate> updatedEntity)
        {
            PatchCertificate(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific certificate by its primary key</summary>
        /// <param name="id">The primary key of the certificate</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteCertificate(id);
            return true;
        }
        #region
        private List<Certificate> GetCertificate(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Certificate.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Certificate>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Certificate), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Certificate, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateCertificate(Certificate model)
        {
            _dbContext.Certificate.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateCertificate(Guid id, Certificate updatedEntity)
        {
            _dbContext.Certificate.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteCertificate(Guid id)
        {
            var entityData = _dbContext.Certificate.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Certificate.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchCertificate(Guid id, JsonPatchDocument<Certificate> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Certificate.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Certificate.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}