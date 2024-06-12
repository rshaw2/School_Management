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
    /// Controller responsible for managing documenttypes related operations.
    /// </summary>
    /// <remarks>
    /// This Controller provides endpoints for adding, retrieving, updating, and deleting documenttypes information.
    /// </remarks>
    [Route("api/documenttypes")]
    [Authorize]
    public class DocumentTypesController : ControllerBase
    {
        private readonly IDocumentTypesService _documentTypesService;

        /// <summary>
        /// Initializes a new instance of the DocumentTypesController class with the specified context.
        /// </summary>
        /// <param name="idocumenttypesservice">The idocumenttypesservice to be used by the controller.</param>
        public DocumentTypesController(IDocumentTypesService idocumenttypesservice)
        {
            _documentTypesService = idocumenttypesservice;
        }

        /// <summary>Adds a new documenttypes</summary>
        /// <param name="model">The documenttypes data to be added</param>
        /// <returns>The result of the operation</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [UserAuthorize("DocumentTypes",Entitlements.Create)]
        public IActionResult Post([FromBody] DocumentTypes model)
        {
            var id = _documentTypesService.Create(model);
            return Ok(new { id });
        }

        /// <summary>Retrieves a list of documenttypess based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of documenttypess</returns>
        [HttpGet]
        [UserAuthorize("DocumentTypes",Entitlements.Read)]
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

            var result = _documentTypesService.Get(filterCriteria, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return Ok(result);
        }

        /// <summary>Retrieves a specific documenttypes by its primary key</summary>
        /// <param name="id">The primary key of the documenttypes</param>
        /// <returns>The documenttypes data</returns>
        [HttpGet]
        [Route("{id:Guid}")]
        [UserAuthorize("DocumentTypes",Entitlements.Read)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public IActionResult GetById([FromRoute] Guid id)
        {
            var result = _documentTypesService.GetById(id);
            return Ok(result);
        }

        /// <summary>Deletes a specific documenttypes by its primary key</summary>
        /// <param name="id">The primary key of the documenttypes</param>
        /// <returns>The result of the operation</returns>
        [HttpDelete]
        [UserAuthorize("DocumentTypes",Entitlements.Delete)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [Route("{id:Guid}")]
        public IActionResult DeleteById([FromRoute] Guid id)
        {
            var status = _documentTypesService.Delete(id);
            return Ok(new { status });
        }

        /// <summary>Updates a specific documenttypes by its primary key</summary>
        /// <param name="id">The primary key of the documenttypes</param>
        /// <param name="updatedEntity">The documenttypes data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPut]
        [UserAuthorize("DocumentTypes",Entitlements.Update)]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult UpdateById(Guid id, [FromBody] DocumentTypes updatedEntity)
        {
            if (id != updatedEntity.Id)
            {
                return BadRequest("Mismatched Id");
            }

            var status = _documentTypesService.Update(id, updatedEntity);
            return Ok(new { status });
        }

        /// <summary>Updates a specific documenttypes by its primary key</summary>
        /// <param name="id">The primary key of the documenttypes</param>
        /// <param name="updatedEntity">The documenttypes data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPatch]
        [UserAuthorize("DocumentTypes",Entitlements.Update)]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult UpdateById(Guid id, [FromBody] JsonPatchDocument<DocumentTypes> updatedEntity)
        {
            if (updatedEntity == null)
                return BadRequest("Patch document is missing.");
            var status = _documentTypesService.Patch(id, updatedEntity);
            return Ok(new { status });
        }
    }
}