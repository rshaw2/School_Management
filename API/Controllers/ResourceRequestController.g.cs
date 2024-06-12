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
    /// Controller responsible for managing resourcerequest related operations.
    /// </summary>
    /// <remarks>
    /// This Controller provides endpoints for adding, retrieving, updating, and deleting resourcerequest information.
    /// </remarks>
    [Route("api/resourcerequest")]
    [Authorize]
    public class ResourceRequestController : ControllerBase
    {
        private readonly IResourceRequestService _resourceRequestService;

        /// <summary>
        /// Initializes a new instance of the ResourceRequestController class with the specified context.
        /// </summary>
        /// <param name="iresourcerequestservice">The iresourcerequestservice to be used by the controller.</param>
        public ResourceRequestController(IResourceRequestService iresourcerequestservice)
        {
            _resourceRequestService = iresourcerequestservice;
        }

        /// <summary>Adds a new resourcerequest</summary>
        /// <param name="model">The resourcerequest data to be added</param>
        /// <returns>The result of the operation</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [UserAuthorize("ResourceRequest",Entitlements.Create)]
        public IActionResult Post([FromBody] ResourceRequest model)
        {
            var id = _resourceRequestService.Create(model);
            return Ok(new { id });
        }

        /// <summary>Retrieves a list of resourcerequests based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of resourcerequests</returns>
        [HttpGet]
        [UserAuthorize("ResourceRequest",Entitlements.Read)]
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

            var result = _resourceRequestService.Get(filterCriteria, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return Ok(result);
        }

        /// <summary>Retrieves a specific resourcerequest by its primary key</summary>
        /// <param name="id">The primary key of the resourcerequest</param>
        /// <returns>The resourcerequest data</returns>
        [HttpGet]
        [Route("{id:Guid}")]
        [UserAuthorize("ResourceRequest",Entitlements.Read)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public IActionResult GetById([FromRoute] Guid id)
        {
            var result = _resourceRequestService.GetById(id);
            return Ok(result);
        }

        /// <summary>Deletes a specific resourcerequest by its primary key</summary>
        /// <param name="id">The primary key of the resourcerequest</param>
        /// <returns>The result of the operation</returns>
        [HttpDelete]
        [UserAuthorize("ResourceRequest",Entitlements.Delete)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [Route("{id:Guid}")]
        public IActionResult DeleteById([FromRoute] Guid id)
        {
            var status = _resourceRequestService.Delete(id);
            return Ok(new { status });
        }

        /// <summary>Updates a specific resourcerequest by its primary key</summary>
        /// <param name="id">The primary key of the resourcerequest</param>
        /// <param name="updatedEntity">The resourcerequest data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPut]
        [UserAuthorize("ResourceRequest",Entitlements.Update)]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult UpdateById(Guid id, [FromBody] ResourceRequest updatedEntity)
        {
            if (id != updatedEntity.Id)
            {
                return BadRequest("Mismatched Id");
            }

            var status = _resourceRequestService.Update(id, updatedEntity);
            return Ok(new { status });
        }

        /// <summary>Updates a specific resourcerequest by its primary key</summary>
        /// <param name="id">The primary key of the resourcerequest</param>
        /// <param name="updatedEntity">The resourcerequest data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPatch]
        [UserAuthorize("ResourceRequest",Entitlements.Update)]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult UpdateById(Guid id, [FromBody] JsonPatchDocument<ResourceRequest> updatedEntity)
        {
            if (updatedEntity == null)
                return BadRequest("Patch document is missing.");
            var status = _resourceRequestService.Patch(id, updatedEntity);
            return Ok(new { status });
        }
    }
}