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
    /// Controller responsible for managing reviewschedule related operations.
    /// </summary>
    /// <remarks>
    /// This Controller provides endpoints for adding, retrieving, updating, and deleting reviewschedule information.
    /// </remarks>
    [Route("api/reviewschedule")]
    [Authorize]
    public class ReviewScheduleController : ControllerBase
    {
        private readonly IReviewScheduleService _reviewScheduleService;

        /// <summary>
        /// Initializes a new instance of the ReviewScheduleController class with the specified context.
        /// </summary>
        /// <param name="ireviewscheduleservice">The ireviewscheduleservice to be used by the controller.</param>
        public ReviewScheduleController(IReviewScheduleService ireviewscheduleservice)
        {
            _reviewScheduleService = ireviewscheduleservice;
        }

        /// <summary>Adds a new reviewschedule</summary>
        /// <param name="model">The reviewschedule data to be added</param>
        /// <returns>The result of the operation</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [UserAuthorize("ReviewSchedule",Entitlements.Create)]
        public IActionResult Post([FromBody] ReviewSchedule model)
        {
            var id = _reviewScheduleService.Create(model);
            return Ok(new { id });
        }

        /// <summary>Retrieves a list of reviewschedules based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of reviewschedules</returns>
        [HttpGet]
        [UserAuthorize("ReviewSchedule",Entitlements.Read)]
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

            var result = _reviewScheduleService.Get(filterCriteria, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return Ok(result);
        }

        /// <summary>Retrieves a specific reviewschedule by its primary key</summary>
        /// <param name="id">The primary key of the reviewschedule</param>
        /// <returns>The reviewschedule data</returns>
        [HttpGet]
        [Route("{id:Guid}")]
        [UserAuthorize("ReviewSchedule",Entitlements.Read)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public IActionResult GetById([FromRoute] Guid id)
        {
            var result = _reviewScheduleService.GetById(id);
            return Ok(result);
        }

        /// <summary>Deletes a specific reviewschedule by its primary key</summary>
        /// <param name="id">The primary key of the reviewschedule</param>
        /// <returns>The result of the operation</returns>
        [HttpDelete]
        [UserAuthorize("ReviewSchedule",Entitlements.Delete)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [Route("{id:Guid}")]
        public IActionResult DeleteById([FromRoute] Guid id)
        {
            var status = _reviewScheduleService.Delete(id);
            return Ok(new { status });
        }

        /// <summary>Updates a specific reviewschedule by its primary key</summary>
        /// <param name="id">The primary key of the reviewschedule</param>
        /// <param name="updatedEntity">The reviewschedule data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPut]
        [UserAuthorize("ReviewSchedule",Entitlements.Update)]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult UpdateById(Guid id, [FromBody] ReviewSchedule updatedEntity)
        {
            if (id != updatedEntity.Id)
            {
                return BadRequest("Mismatched Id");
            }

            var status = _reviewScheduleService.Update(id, updatedEntity);
            return Ok(new { status });
        }

        /// <summary>Updates a specific reviewschedule by its primary key</summary>
        /// <param name="id">The primary key of the reviewschedule</param>
        /// <param name="updatedEntity">The reviewschedule data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPatch]
        [UserAuthorize("ReviewSchedule",Entitlements.Update)]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult UpdateById(Guid id, [FromBody] JsonPatchDocument<ReviewSchedule> updatedEntity)
        {
            if (updatedEntity == null)
                return BadRequest("Patch document is missing.");
            var status = _reviewScheduleService.Patch(id, updatedEntity);
            return Ok(new { status });
        }
    }
}