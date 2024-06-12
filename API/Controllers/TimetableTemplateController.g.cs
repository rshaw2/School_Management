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
    /// Controller responsible for managing timetabletemplate related operations.
    /// </summary>
    /// <remarks>
    /// This Controller provides endpoints for adding, retrieving, updating, and deleting timetabletemplate information.
    /// </remarks>
    [Route("api/timetabletemplate")]
    [Authorize]
    public class TimetableTemplateController : ControllerBase
    {
        private readonly ITimetableTemplateService _timetableTemplateService;

        /// <summary>
        /// Initializes a new instance of the TimetableTemplateController class with the specified context.
        /// </summary>
        /// <param name="itimetabletemplateservice">The itimetabletemplateservice to be used by the controller.</param>
        public TimetableTemplateController(ITimetableTemplateService itimetabletemplateservice)
        {
            _timetableTemplateService = itimetabletemplateservice;
        }

        /// <summary>Adds a new timetabletemplate</summary>
        /// <param name="model">The timetabletemplate data to be added</param>
        /// <returns>The result of the operation</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [UserAuthorize("TimetableTemplate",Entitlements.Create)]
        public IActionResult Post([FromBody] TimetableTemplate model)
        {
            var id = _timetableTemplateService.Create(model);
            return Ok(new { id });
        }

        /// <summary>Retrieves a list of timetabletemplates based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of timetabletemplates</returns>
        [HttpGet]
        [UserAuthorize("TimetableTemplate",Entitlements.Read)]
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

            var result = _timetableTemplateService.Get(filterCriteria, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return Ok(result);
        }

        /// <summary>Retrieves a specific timetabletemplate by its primary key</summary>
        /// <param name="id">The primary key of the timetabletemplate</param>
        /// <returns>The timetabletemplate data</returns>
        [HttpGet]
        [Route("{id:Guid}")]
        [UserAuthorize("TimetableTemplate",Entitlements.Read)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public IActionResult GetById([FromRoute] Guid id)
        {
            var result = _timetableTemplateService.GetById(id);
            return Ok(result);
        }

        /// <summary>Deletes a specific timetabletemplate by its primary key</summary>
        /// <param name="id">The primary key of the timetabletemplate</param>
        /// <returns>The result of the operation</returns>
        [HttpDelete]
        [UserAuthorize("TimetableTemplate",Entitlements.Delete)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [Route("{id:Guid}")]
        public IActionResult DeleteById([FromRoute] Guid id)
        {
            var status = _timetableTemplateService.Delete(id);
            return Ok(new { status });
        }

        /// <summary>Updates a specific timetabletemplate by its primary key</summary>
        /// <param name="id">The primary key of the timetabletemplate</param>
        /// <param name="updatedEntity">The timetabletemplate data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPut]
        [UserAuthorize("TimetableTemplate",Entitlements.Update)]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult UpdateById(Guid id, [FromBody] TimetableTemplate updatedEntity)
        {
            if (id != updatedEntity.Id)
            {
                return BadRequest("Mismatched Id");
            }

            var status = _timetableTemplateService.Update(id, updatedEntity);
            return Ok(new { status });
        }

        /// <summary>Updates a specific timetabletemplate by its primary key</summary>
        /// <param name="id">The primary key of the timetabletemplate</param>
        /// <param name="updatedEntity">The timetabletemplate data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPatch]
        [UserAuthorize("TimetableTemplate",Entitlements.Update)]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult UpdateById(Guid id, [FromBody] JsonPatchDocument<TimetableTemplate> updatedEntity)
        {
            if (updatedEntity == null)
                return BadRequest("Patch document is missing.");
            var status = _timetableTemplateService.Patch(id, updatedEntity);
            return Ok(new { status });
        }
    }
}