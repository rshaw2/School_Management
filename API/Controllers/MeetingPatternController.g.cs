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
    /// Controller responsible for managing meetingpattern related operations.
    /// </summary>
    /// <remarks>
    /// This Controller provides endpoints for adding, retrieving, updating, and deleting meetingpattern information.
    /// </remarks>
    [Route("api/meetingpattern")]
    [Authorize]
    public class MeetingPatternController : ControllerBase
    {
        private readonly IMeetingPatternService _meetingPatternService;

        /// <summary>
        /// Initializes a new instance of the MeetingPatternController class with the specified context.
        /// </summary>
        /// <param name="imeetingpatternservice">The imeetingpatternservice to be used by the controller.</param>
        public MeetingPatternController(IMeetingPatternService imeetingpatternservice)
        {
            _meetingPatternService = imeetingpatternservice;
        }

        /// <summary>Adds a new meetingpattern</summary>
        /// <param name="model">The meetingpattern data to be added</param>
        /// <returns>The result of the operation</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [UserAuthorize("MeetingPattern",Entitlements.Create)]
        public IActionResult Post([FromBody] MeetingPattern model)
        {
            var id = _meetingPatternService.Create(model);
            return Ok(new { id });
        }

        /// <summary>Retrieves a list of meetingpatterns based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of meetingpatterns</returns>
        [HttpGet]
        [UserAuthorize("MeetingPattern",Entitlements.Read)]
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

            var result = _meetingPatternService.Get(filterCriteria, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return Ok(result);
        }

        /// <summary>Retrieves a specific meetingpattern by its primary key</summary>
        /// <param name="id">The primary key of the meetingpattern</param>
        /// <returns>The meetingpattern data</returns>
        [HttpGet]
        [Route("{id:Guid}")]
        [UserAuthorize("MeetingPattern",Entitlements.Read)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public IActionResult GetById([FromRoute] Guid id)
        {
            var result = _meetingPatternService.GetById(id);
            return Ok(result);
        }

        /// <summary>Deletes a specific meetingpattern by its primary key</summary>
        /// <param name="id">The primary key of the meetingpattern</param>
        /// <returns>The result of the operation</returns>
        [HttpDelete]
        [UserAuthorize("MeetingPattern",Entitlements.Delete)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [Route("{id:Guid}")]
        public IActionResult DeleteById([FromRoute] Guid id)
        {
            var status = _meetingPatternService.Delete(id);
            return Ok(new { status });
        }

        /// <summary>Updates a specific meetingpattern by its primary key</summary>
        /// <param name="id">The primary key of the meetingpattern</param>
        /// <param name="updatedEntity">The meetingpattern data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPut]
        [UserAuthorize("MeetingPattern",Entitlements.Update)]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult UpdateById(Guid id, [FromBody] MeetingPattern updatedEntity)
        {
            if (id != updatedEntity.Id)
            {
                return BadRequest("Mismatched Id");
            }

            var status = _meetingPatternService.Update(id, updatedEntity);
            return Ok(new { status });
        }

        /// <summary>Updates a specific meetingpattern by its primary key</summary>
        /// <param name="id">The primary key of the meetingpattern</param>
        /// <param name="updatedEntity">The meetingpattern data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPatch]
        [UserAuthorize("MeetingPattern",Entitlements.Update)]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult UpdateById(Guid id, [FromBody] JsonPatchDocument<MeetingPattern> updatedEntity)
        {
            if (updatedEntity == null)
                return BadRequest("Patch document is missing.");
            var status = _meetingPatternService.Patch(id, updatedEntity);
            return Ok(new { status });
        }
    }
}