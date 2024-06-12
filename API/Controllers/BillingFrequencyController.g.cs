using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Models;
using SchoolManagement.Services;
using SchoolManagement.Entities;
using SchoolManagement.Filter;
using SchoolManagement.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;

namespace SchoolManagement.Controllers
{
    /// <summary>
    /// Controller responsible for managing billingfrequency related operations.
    /// </summary>
    /// <remarks>
    /// This Controller provides endpoints for adding, retrieving, updating, and deleting billingfrequency information.
    /// </remarks>
    [Route("api/billingfrequency")]
    [Authorize]
    public class BillingFrequencyController : ControllerBase
    {
        private readonly IBillingFrequencyService _billingFrequencyService;

        /// <summary>
        /// Initializes a new instance of the BillingFrequencyController class with the specified context.
        /// </summary>
        /// <param name="ibillingfrequencyservice">The ibillingfrequencyservice to be used by the controller.</param>
        public BillingFrequencyController(IBillingFrequencyService ibillingfrequencyservice)
        {
            _billingFrequencyService = ibillingfrequencyservice;
        }

        /// <summary>Adds a new billingfrequency</summary>
        /// <param name="model">The billingfrequency data to be added</param>
        /// <returns>The result of the operation</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [UserAuthorize("BillingFrequency",Entitlements.Create)]
        public IActionResult Post([FromBody] BillingFrequency model)
        {
            var id = _billingFrequencyService.Create(model);
            return Ok(new { id });
        }

        /// <summary>Retrieves a list of billingfrequencys based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of billingfrequencys</returns>
        [HttpGet]
        [UserAuthorize("BillingFrequency",Entitlements.Read)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult Get([FromQuery] string filters, string searchTerm, int pageNumber = 1, int pageSize = 10, string sortField = null, string sortOrder = "asc")
        {
            List<FilterCriteria> filterCriteria = null;
            if (pageSize < 1)
            {
                return BadRequest("Page size invalid.");
            }

            if (pageNumber < 1)
            {
                return BadRequest("Page mumber invalid.");
            }

            if (!string.IsNullOrEmpty(filters))
            {
                filterCriteria = JsonHelper.Deserialize<List<FilterCriteria>>(filters);
            }

            var result = _billingFrequencyService.Get(filterCriteria, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return Ok(result);
        }

        /// <summary>Retrieves a specific billingfrequency by its primary key</summary>
        /// <param name="id">The primary key of the billingfrequency</param>
        /// <returns>The billingfrequency data</returns>
        [HttpGet]
        [Route("{id:Guid}")]
        [UserAuthorize("BillingFrequency",Entitlements.Read)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public IActionResult GetById([FromRoute] Guid id)
        {
            var result = _billingFrequencyService.GetById(id);
            return Ok(result);
        }

        /// <summary>Deletes a specific billingfrequency by its primary key</summary>
        /// <param name="id">The primary key of the billingfrequency</param>
        /// <returns>The result of the operation</returns>
        [HttpDelete]
        [UserAuthorize("BillingFrequency",Entitlements.Delete)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [Route("{id:Guid}")]
        public IActionResult DeleteById([FromRoute] Guid id)
        {
            var status = _billingFrequencyService.Delete(id);
            return Ok(new { status });
        }

        /// <summary>Updates a specific billingfrequency by its primary key</summary>
        /// <param name="id">The primary key of the billingfrequency</param>
        /// <param name="updatedEntity">The billingfrequency data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPut]
        [UserAuthorize("BillingFrequency",Entitlements.Update)]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult UpdateById(Guid id, [FromBody] BillingFrequency updatedEntity)
        {
            if (id != updatedEntity.Id)
            {
                return BadRequest("Mismatched Id");
            }

            var status = _billingFrequencyService.Update(id, updatedEntity);
            return Ok(new { status });
        }

        /// <summary>Updates a specific billingfrequency by its primary key</summary>
        /// <param name="id">The primary key of the billingfrequency</param>
        /// <param name="updatedEntity">The billingfrequency data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPatch]
        [UserAuthorize("BillingFrequency",Entitlements.Update)]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult UpdateById(Guid id, [FromBody] JsonPatchDocument<BillingFrequency> updatedEntity)
        {
            if (updatedEntity == null)
                return BadRequest("Patch document is missing.");
            var status = _billingFrequencyService.Patch(id, updatedEntity);
            return Ok(new { status });
        }
    }
}