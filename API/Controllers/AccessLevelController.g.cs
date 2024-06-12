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
    /// Controller responsible for managing accesslevel related operations.
    /// </summary>
    /// <remarks>
    /// This Controller provides endpoints for adding, retrieving, updating, and deleting accesslevel information.
    /// </remarks>
    [Route("api/accesslevel")]
    [Authorize]
    public class AccessLevelController : ControllerBase
    {
        private readonly IAccessLevelService _accessLevelService;

        /// <summary>
        /// Initializes a new instance of the AccessLevelController class with the specified context.
        /// </summary>
        /// <param name="iaccesslevelservice">The iaccesslevelservice to be used by the controller.</param>
        public AccessLevelController(IAccessLevelService iaccesslevelservice)
        {
            _accessLevelService = iaccesslevelservice;
        }

        /// <summary>Adds a new accesslevel</summary>
        /// <param name="model">The accesslevel data to be added</param>
        /// <returns>The result of the operation</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [UserAuthorize("AccessLevel",Entitlements.Create)]
        public IActionResult Post([FromBody] AccessLevel model)
        {
            var id = _accessLevelService.Create(model);
            return Ok(new { id });
        }

        /// <summary>Retrieves a list of accesslevels based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of accesslevels</returns>
        [HttpGet]
        [UserAuthorize("AccessLevel",Entitlements.Read)]
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

            var result = _accessLevelService.Get(filterCriteria, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return Ok(result);
        }

        /// <summary>Retrieves a specific accesslevel by its primary key</summary>
        /// <param name="id">The primary key of the accesslevel</param>
        /// <returns>The accesslevel data</returns>
        [HttpGet]
        [Route("{id:Guid}")]
        [UserAuthorize("AccessLevel",Entitlements.Read)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public IActionResult GetById([FromRoute] Guid id)
        {
            var result = _accessLevelService.GetById(id);
            return Ok(result);
        }

        /// <summary>Deletes a specific accesslevel by its primary key</summary>
        /// <param name="id">The primary key of the accesslevel</param>
        /// <returns>The result of the operation</returns>
        [HttpDelete]
        [UserAuthorize("AccessLevel",Entitlements.Delete)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [Route("{id:Guid}")]
        public IActionResult DeleteById([FromRoute] Guid id)
        {
            var status = _accessLevelService.Delete(id);
            return Ok(new { status });
        }

        /// <summary>Updates a specific accesslevel by its primary key</summary>
        /// <param name="id">The primary key of the accesslevel</param>
        /// <param name="updatedEntity">The accesslevel data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPut]
        [UserAuthorize("AccessLevel",Entitlements.Update)]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult UpdateById(Guid id, [FromBody] AccessLevel updatedEntity)
        {
            if (id != updatedEntity.Id)
            {
                return BadRequest("Mismatched Id");
            }

            var status = _accessLevelService.Update(id, updatedEntity);
            return Ok(new { status });
        }

        /// <summary>Updates a specific accesslevel by its primary key</summary>
        /// <param name="id">The primary key of the accesslevel</param>
        /// <param name="updatedEntity">The accesslevel data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPatch]
        [UserAuthorize("AccessLevel",Entitlements.Update)]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult UpdateById(Guid id, [FromBody] JsonPatchDocument<AccessLevel> updatedEntity)
        {
            if (updatedEntity == null)
                return BadRequest("Patch document is missing.");
            var status = _accessLevelService.Patch(id, updatedEntity);
            return Ok(new { status });
        }
    }
}