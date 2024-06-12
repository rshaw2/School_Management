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
    /// The invoicelineitemService responsible for managing invoicelineitem related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting invoicelineitem information.
    /// </remarks>
    public interface IInvoiceLineItemService
    {
        /// <summary>Retrieves a specific invoicelineitem by its primary key</summary>
        /// <param name="id">The primary key of the invoicelineitem</param>
        /// <returns>The invoicelineitem data</returns>
        InvoiceLineItem GetById(Guid id);

        /// <summary>Retrieves a list of invoicelineitems based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of invoicelineitems</returns>
        List<InvoiceLineItem> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new invoicelineitem</summary>
        /// <param name="model">The invoicelineitem data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(InvoiceLineItem model);

        /// <summary>Updates a specific invoicelineitem by its primary key</summary>
        /// <param name="id">The primary key of the invoicelineitem</param>
        /// <param name="updatedEntity">The invoicelineitem data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, InvoiceLineItem updatedEntity);

        /// <summary>Updates a specific invoicelineitem by its primary key</summary>
        /// <param name="id">The primary key of the invoicelineitem</param>
        /// <param name="updatedEntity">The invoicelineitem data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<InvoiceLineItem> updatedEntity);

        /// <summary>Deletes a specific invoicelineitem by its primary key</summary>
        /// <param name="id">The primary key of the invoicelineitem</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The invoicelineitemService responsible for managing invoicelineitem related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting invoicelineitem information.
    /// </remarks>
    public class InvoiceLineItemService : IInvoiceLineItemService
    {
        private SchoolManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the InvoiceLineItem class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public InvoiceLineItemService(SchoolManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific invoicelineitem by its primary key</summary>
        /// <param name="id">The primary key of the invoicelineitem</param>
        /// <returns>The invoicelineitem data</returns>
        public InvoiceLineItem GetById(Guid id)
        {
            var entityData = _dbContext.InvoiceLineItem.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of invoicelineitems based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of invoicelineitems</returns>/// <exception cref="Exception"></exception>
        public List<InvoiceLineItem> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetInvoiceLineItem(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new invoicelineitem</summary>
        /// <param name="model">The invoicelineitem data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(InvoiceLineItem model)
        {
            model.Id = CreateInvoiceLineItem(model);
            return model.Id;
        }

        /// <summary>Updates a specific invoicelineitem by its primary key</summary>
        /// <param name="id">The primary key of the invoicelineitem</param>
        /// <param name="updatedEntity">The invoicelineitem data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, InvoiceLineItem updatedEntity)
        {
            UpdateInvoiceLineItem(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific invoicelineitem by its primary key</summary>
        /// <param name="id">The primary key of the invoicelineitem</param>
        /// <param name="updatedEntity">The invoicelineitem data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<InvoiceLineItem> updatedEntity)
        {
            PatchInvoiceLineItem(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific invoicelineitem by its primary key</summary>
        /// <param name="id">The primary key of the invoicelineitem</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteInvoiceLineItem(id);
            return true;
        }
        #region
        private List<InvoiceLineItem> GetInvoiceLineItem(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.InvoiceLineItem.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<InvoiceLineItem>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(InvoiceLineItem), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<InvoiceLineItem, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateInvoiceLineItem(InvoiceLineItem model)
        {
            _dbContext.InvoiceLineItem.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateInvoiceLineItem(Guid id, InvoiceLineItem updatedEntity)
        {
            _dbContext.InvoiceLineItem.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteInvoiceLineItem(Guid id)
        {
            var entityData = _dbContext.InvoiceLineItem.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.InvoiceLineItem.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchInvoiceLineItem(Guid id, JsonPatchDocument<InvoiceLineItem> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.InvoiceLineItem.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.InvoiceLineItem.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}