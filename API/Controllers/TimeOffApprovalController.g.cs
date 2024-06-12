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
    /// Controller responsible for managing timeoffapproval related operations.
    /// </summary>
    /// <remarks>
    /// This Controller provides endpoints for adding, retrieving, updating, and deleting timeoffapproval information.
    /// </remarks>
    [Route("api/timeoffapproval")]
    [Authorize]
    public class TimeOffApprovalController : ControllerBase
    {
        private readonly ITimeOffApprovalService _timeOffApprovalService;

        /// <summary>
        /// Initializes a new instance of the TimeOffApprovalController class with the specified context.
        /// </summary>
        /// <param name="itimeoffapprovalservice">The itimeoffapprovalservice to be used by the controller.</param>
        public TimeOffApprovalController(ITimeOffApprovalService itimeoffapprovalservice)
        {
            _timeOffApprovalService = itimeoffapprovalservice;
        }

        /// <summary>Adds a new timeoffapproval</summary>
        /// <param name="model">The timeoffapproval data to be added</param>
        /// <returns>The result of the operation</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [UserAuthorize("TimeOffApproval",Entitlements.Create)]
        public IActionResult Post([FromBody] TimeOffApproval model)
        {
            var id = _timeOffApprovalService.Create(model);
            return Ok(new { id });
        }

        /// <summary>Retrieves a list of timeoffapprovals based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of timeoffapprovals</returns>
        [HttpGet]
        [UserAuthorize("TimeOffApproval",Entitlements.Read)]
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

            var result = _timeOffApprovalService.Get(filterCriteria, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return Ok(result);
        }

        /// <summary>Retrieves a specific timeoffapproval by its primary key</summary>
        /// <param name="id">The primary key of the timeoffapproval</param>
        /// <returns>The timeoffapproval data</returns>
        [HttpGet]
        [Route("{id:Guid}")]
        [UserAuthorize("TimeOffApproval",Entitlements.Read)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public IActionResult GetById([FromRoute] Guid id)
        {
            var result = _timeOffApprovalService.GetById(id);
            return Ok(result);
        }

        /// <summary>Deletes a specific timeoffapproval by its primary key</summary>
        /// <param name="id">The primary key of the timeoffapproval</param>
        /// <returns>The result of the operation</returns>
        [HttpDelete]
        [UserAuthorize("TimeOffApproval",Entitlements.Delete)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [Route("{id:Guid}")]
        public IActionResult DeleteById([FromRoute] Guid id)
        {
            var status = _timeOffApprovalService.Delete(id);
            return Ok(new { status });
        }

        /// <summary>Updates a specific timeoffapproval by its primary key</summary>
        /// <param name="id">The primary key of the timeoffapproval</param>
        /// <param name="updatedEntity">The timeoffapproval data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPut]
        [UserAuthorize("TimeOffApproval",Entitlements.Update)]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult UpdateById(Guid id, [FromBody] TimeOffApproval updatedEntity)
        {
            if (id != updatedEntity.Id)
            {
                return BadRequest("Mismatched Id");
            }

            var status = _timeOffApprovalService.Update(id, updatedEntity);
            return Ok(new { status });
        }

        /// <summary>Updates a specific timeoffapproval by its primary key</summary>
        /// <param name="id">The primary key of the timeoffapproval</param>
        /// <param name="updatedEntity">The timeoffapproval data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPatch]
        [UserAuthorize("TimeOffApproval",Entitlements.Update)]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult UpdateById(Guid id, [FromBody] JsonPatchDocument<TimeOffApproval> updatedEntity)
        {
            if (updatedEntity == null)
                return BadRequest("Patch document is missing.");
            var status = _timeOffApprovalService.Patch(id, updatedEntity);
            return Ok(new { status });
        }
    }
}