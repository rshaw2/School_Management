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
    /// Controller responsible for managing statusupdate related operations.
    /// </summary>
    /// <remarks>
    /// This Controller provides endpoints for adding, retrieving, updating, and deleting statusupdate information.
    /// </remarks>
    [Route("api/statusupdate")]
    [Authorize]
    public class StatusUpdateController : ControllerBase
    {
        private readonly IStatusUpdateService _statusUpdateService;

        /// <summary>
        /// Initializes a new instance of the StatusUpdateController class with the specified context.
        /// </summary>
        /// <param name="istatusupdateservice">The istatusupdateservice to be used by the controller.</param>
        public StatusUpdateController(IStatusUpdateService istatusupdateservice)
        {
            _statusUpdateService = istatusupdateservice;
        }

        /// <summary>Adds a new statusupdate</summary>
        /// <param name="model">The statusupdate data to be added</param>
        /// <returns>The result of the operation</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [UserAuthorize("StatusUpdate",Entitlements.Create)]
        public IActionResult Post([FromBody] StatusUpdate model)
        {
            var id = _statusUpdateService.Create(model);
            return Ok(new { id });
        }

        /// <summary>Retrieves a list of statusupdates based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of statusupdates</returns>
        [HttpGet]
        [UserAuthorize("StatusUpdate",Entitlements.Read)]
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

            var result = _statusUpdateService.Get(filterCriteria, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return Ok(result);
        }

        /// <summary>Retrieves a specific statusupdate by its primary key</summary>
        /// <param name="id">The primary key of the statusupdate</param>
        /// <returns>The statusupdate data</returns>
        [HttpGet]
        [Route("{id:Guid}")]
        [UserAuthorize("StatusUpdate",Entitlements.Read)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public IActionResult GetById([FromRoute] Guid id)
        {
            var result = _statusUpdateService.GetById(id);
            return Ok(result);
        }

        /// <summary>Deletes a specific statusupdate by its primary key</summary>
        /// <param name="id">The primary key of the statusupdate</param>
        /// <returns>The result of the operation</returns>
        [HttpDelete]
        [UserAuthorize("StatusUpdate",Entitlements.Delete)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [Route("{id:Guid}")]
        public IActionResult DeleteById([FromRoute] Guid id)
        {
            var status = _statusUpdateService.Delete(id);
            return Ok(new { status });
        }

        /// <summary>Updates a specific statusupdate by its primary key</summary>
        /// <param name="id">The primary key of the statusupdate</param>
        /// <param name="updatedEntity">The statusupdate data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPut]
        [UserAuthorize("StatusUpdate",Entitlements.Update)]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult UpdateById(Guid id, [FromBody] StatusUpdate updatedEntity)
        {
            if (id != updatedEntity.Id)
            {
                return BadRequest("Mismatched Id");
            }

            var status = _statusUpdateService.Update(id, updatedEntity);
            return Ok(new { status });
        }

        /// <summary>Updates a specific statusupdate by its primary key</summary>
        /// <param name="id">The primary key of the statusupdate</param>
        /// <param name="updatedEntity">The statusupdate data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPatch]
        [UserAuthorize("StatusUpdate",Entitlements.Update)]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult UpdateById(Guid id, [FromBody] JsonPatchDocument<StatusUpdate> updatedEntity)
        {
            if (updatedEntity == null)
                return BadRequest("Patch document is missing.");
            var status = _statusUpdateService.Patch(id, updatedEntity);
            return Ok(new { status });
        }
    }
}