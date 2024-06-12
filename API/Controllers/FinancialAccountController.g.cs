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
    /// Controller responsible for managing financialaccount related operations.
    /// </summary>
    /// <remarks>
    /// This Controller provides endpoints for adding, retrieving, updating, and deleting financialaccount information.
    /// </remarks>
    [Route("api/financialaccount")]
    [Authorize]
    public class FinancialAccountController : ControllerBase
    {
        private readonly IFinancialAccountService _financialAccountService;

        /// <summary>
        /// Initializes a new instance of the FinancialAccountController class with the specified context.
        /// </summary>
        /// <param name="ifinancialaccountservice">The ifinancialaccountservice to be used by the controller.</param>
        public FinancialAccountController(IFinancialAccountService ifinancialaccountservice)
        {
            _financialAccountService = ifinancialaccountservice;
        }

        /// <summary>Adds a new financialaccount</summary>
        /// <param name="model">The financialaccount data to be added</param>
        /// <returns>The result of the operation</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [UserAuthorize("FinancialAccount",Entitlements.Create)]
        public IActionResult Post([FromBody] FinancialAccount model)
        {
            var id = _financialAccountService.Create(model);
            return Ok(new { id });
        }

        /// <summary>Retrieves a list of financialaccounts based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of financialaccounts</returns>
        [HttpGet]
        [UserAuthorize("FinancialAccount",Entitlements.Read)]
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

            var result = _financialAccountService.Get(filterCriteria, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return Ok(result);
        }

        /// <summary>Retrieves a specific financialaccount by its primary key</summary>
        /// <param name="id">The primary key of the financialaccount</param>
        /// <returns>The financialaccount data</returns>
        [HttpGet]
        [Route("{id:Guid}")]
        [UserAuthorize("FinancialAccount",Entitlements.Read)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public IActionResult GetById([FromRoute] Guid id)
        {
            var result = _financialAccountService.GetById(id);
            return Ok(result);
        }

        /// <summary>Deletes a specific financialaccount by its primary key</summary>
        /// <param name="id">The primary key of the financialaccount</param>
        /// <returns>The result of the operation</returns>
        [HttpDelete]
        [UserAuthorize("FinancialAccount",Entitlements.Delete)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [Route("{id:Guid}")]
        public IActionResult DeleteById([FromRoute] Guid id)
        {
            var status = _financialAccountService.Delete(id);
            return Ok(new { status });
        }

        /// <summary>Updates a specific financialaccount by its primary key</summary>
        /// <param name="id">The primary key of the financialaccount</param>
        /// <param name="updatedEntity">The financialaccount data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPut]
        [UserAuthorize("FinancialAccount",Entitlements.Update)]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult UpdateById(Guid id, [FromBody] FinancialAccount updatedEntity)
        {
            if (id != updatedEntity.Id)
            {
                return BadRequest("Mismatched Id");
            }

            var status = _financialAccountService.Update(id, updatedEntity);
            return Ok(new { status });
        }

        /// <summary>Updates a specific financialaccount by its primary key</summary>
        /// <param name="id">The primary key of the financialaccount</param>
        /// <param name="updatedEntity">The financialaccount data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPatch]
        [UserAuthorize("FinancialAccount",Entitlements.Update)]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult UpdateById(Guid id, [FromBody] JsonPatchDocument<FinancialAccount> updatedEntity)
        {
            if (updatedEntity == null)
                return BadRequest("Patch document is missing.");
            var status = _financialAccountService.Patch(id, updatedEntity);
            return Ok(new { status });
        }
    }
}