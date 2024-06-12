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
    /// Controller responsible for managing resourcebooking related operations.
    /// </summary>
    /// <remarks>
    /// This Controller provides endpoints for adding, retrieving, updating, and deleting resourcebooking information.
    /// </remarks>
    [Route("api/resourcebooking")]
    [Authorize]
    public class ResourceBookingController : ControllerBase
    {
        private readonly IResourceBookingService _resourceBookingService;

        /// <summary>
        /// Initializes a new instance of the ResourceBookingController class with the specified context.
        /// </summary>
        /// <param name="iresourcebookingservice">The iresourcebookingservice to be used by the controller.</param>
        public ResourceBookingController(IResourceBookingService iresourcebookingservice)
        {
            _resourceBookingService = iresourcebookingservice;
        }

        /// <summary>Adds a new resourcebooking</summary>
        /// <param name="model">The resourcebooking data to be added</param>
        /// <returns>The result of the operation</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [UserAuthorize("ResourceBooking",Entitlements.Create)]
        public IActionResult Post([FromBody] ResourceBooking model)
        {
            var id = _resourceBookingService.Create(model);
            return Ok(new { id });
        }

        /// <summary>Retrieves a list of resourcebookings based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of resourcebookings</returns>
        [HttpGet]
        [UserAuthorize("ResourceBooking",Entitlements.Read)]
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

            var result = _resourceBookingService.Get(filterCriteria, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return Ok(result);
        }

        /// <summary>Retrieves a specific resourcebooking by its primary key</summary>
        /// <param name="id">The primary key of the resourcebooking</param>
        /// <returns>The resourcebooking data</returns>
        [HttpGet]
        [Route("{id:Guid}")]
        [UserAuthorize("ResourceBooking",Entitlements.Read)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public IActionResult GetById([FromRoute] Guid id)
        {
            var result = _resourceBookingService.GetById(id);
            return Ok(result);
        }

        /// <summary>Deletes a specific resourcebooking by its primary key</summary>
        /// <param name="id">The primary key of the resourcebooking</param>
        /// <returns>The result of the operation</returns>
        [HttpDelete]
        [UserAuthorize("ResourceBooking",Entitlements.Delete)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [Route("{id:Guid}")]
        public IActionResult DeleteById([FromRoute] Guid id)
        {
            var status = _resourceBookingService.Delete(id);
            return Ok(new { status });
        }

        /// <summary>Updates a specific resourcebooking by its primary key</summary>
        /// <param name="id">The primary key of the resourcebooking</param>
        /// <param name="updatedEntity">The resourcebooking data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPut]
        [UserAuthorize("ResourceBooking",Entitlements.Update)]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult UpdateById(Guid id, [FromBody] ResourceBooking updatedEntity)
        {
            if (id != updatedEntity.Id)
            {
                return BadRequest("Mismatched Id");
            }

            var status = _resourceBookingService.Update(id, updatedEntity);
            return Ok(new { status });
        }

        /// <summary>Updates a specific resourcebooking by its primary key</summary>
        /// <param name="id">The primary key of the resourcebooking</param>
        /// <param name="updatedEntity">The resourcebooking data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPatch]
        [UserAuthorize("ResourceBooking",Entitlements.Update)]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult UpdateById(Guid id, [FromBody] JsonPatchDocument<ResourceBooking> updatedEntity)
        {
            if (updatedEntity == null)
                return BadRequest("Patch document is missing.");
            var status = _resourceBookingService.Patch(id, updatedEntity);
            return Ok(new { status });
        }
    }
}