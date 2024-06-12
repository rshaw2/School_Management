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
    /// Controller responsible for managing emergencycontact related operations.
    /// </summary>
    /// <remarks>
    /// This Controller provides endpoints for adding, retrieving, updating, and deleting emergencycontact information.
    /// </remarks>
    [Route("api/emergencycontact")]
    [Authorize]
    public class EmergencyContactController : ControllerBase
    {
        private readonly IEmergencyContactService _emergencyContactService;

        /// <summary>
        /// Initializes a new instance of the EmergencyContactController class with the specified context.
        /// </summary>
        /// <param name="iemergencycontactservice">The iemergencycontactservice to be used by the controller.</param>
        public EmergencyContactController(IEmergencyContactService iemergencycontactservice)
        {
            _emergencyContactService = iemergencycontactservice;
        }

        /// <summary>Adds a new emergencycontact</summary>
        /// <param name="model">The emergencycontact data to be added</param>
        /// <returns>The result of the operation</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [UserAuthorize("EmergencyContact",Entitlements.Create)]
        public IActionResult Post([FromBody] EmergencyContact model)
        {
            var id = _emergencyContactService.Create(model);
            return Ok(new { id });
        }

        /// <summary>Retrieves a list of emergencycontacts based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of emergencycontacts</returns>
        [HttpGet]
        [UserAuthorize("EmergencyContact",Entitlements.Read)]
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

            var result = _emergencyContactService.Get(filterCriteria, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return Ok(result);
        }

        /// <summary>Retrieves a specific emergencycontact by its primary key</summary>
        /// <param name="id">The primary key of the emergencycontact</param>
        /// <returns>The emergencycontact data</returns>
        [HttpGet]
        [Route("{id:Guid}")]
        [UserAuthorize("EmergencyContact",Entitlements.Read)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public IActionResult GetById([FromRoute] Guid id)
        {
            var result = _emergencyContactService.GetById(id);
            return Ok(result);
        }

        /// <summary>Deletes a specific emergencycontact by its primary key</summary>
        /// <param name="id">The primary key of the emergencycontact</param>
        /// <returns>The result of the operation</returns>
        [HttpDelete]
        [UserAuthorize("EmergencyContact",Entitlements.Delete)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [Route("{id:Guid}")]
        public IActionResult DeleteById([FromRoute] Guid id)
        {
            var status = _emergencyContactService.Delete(id);
            return Ok(new { status });
        }

        /// <summary>Updates a specific emergencycontact by its primary key</summary>
        /// <param name="id">The primary key of the emergencycontact</param>
        /// <param name="updatedEntity">The emergencycontact data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPut]
        [UserAuthorize("EmergencyContact",Entitlements.Update)]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult UpdateById(Guid id, [FromBody] EmergencyContact updatedEntity)
        {
            if (id != updatedEntity.Id)
            {
                return BadRequest("Mismatched Id");
            }

            var status = _emergencyContactService.Update(id, updatedEntity);
            return Ok(new { status });
        }

        /// <summary>Updates a specific emergencycontact by its primary key</summary>
        /// <param name="id">The primary key of the emergencycontact</param>
        /// <param name="updatedEntity">The emergencycontact data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPatch]
        [UserAuthorize("EmergencyContact",Entitlements.Update)]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult UpdateById(Guid id, [FromBody] JsonPatchDocument<EmergencyContact> updatedEntity)
        {
            if (updatedEntity == null)
                return BadRequest("Patch document is missing.");
            var status = _emergencyContactService.Patch(id, updatedEntity);
            return Ok(new { status });
        }
    }
}