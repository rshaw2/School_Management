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
    /// Controller responsible for managing knowledgebase related operations.
    /// </summary>
    /// <remarks>
    /// This Controller provides endpoints for adding, retrieving, updating, and deleting knowledgebase information.
    /// </remarks>
    [Route("api/knowledgebase")]
    [Authorize]
    public class KnowledgeBaseController : ControllerBase
    {
        private readonly IKnowledgeBaseService _knowledgeBaseService;

        /// <summary>
        /// Initializes a new instance of the KnowledgeBaseController class with the specified context.
        /// </summary>
        /// <param name="iknowledgebaseservice">The iknowledgebaseservice to be used by the controller.</param>
        public KnowledgeBaseController(IKnowledgeBaseService iknowledgebaseservice)
        {
            _knowledgeBaseService = iknowledgebaseservice;
        }

        /// <summary>Adds a new knowledgebase</summary>
        /// <param name="model">The knowledgebase data to be added</param>
        /// <returns>The result of the operation</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [UserAuthorize("KnowledgeBase",Entitlements.Create)]
        public IActionResult Post([FromBody] KnowledgeBase model)
        {
            var id = _knowledgeBaseService.Create(model);
            return Ok(new { id });
        }

        /// <summary>Retrieves a list of knowledgebases based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of knowledgebases</returns>
        [HttpGet]
        [UserAuthorize("KnowledgeBase",Entitlements.Read)]
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

            var result = _knowledgeBaseService.Get(filterCriteria, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return Ok(result);
        }

        /// <summary>Retrieves a specific knowledgebase by its primary key</summary>
        /// <param name="id">The primary key of the knowledgebase</param>
        /// <returns>The knowledgebase data</returns>
        [HttpGet]
        [Route("{id:Guid}")]
        [UserAuthorize("KnowledgeBase",Entitlements.Read)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public IActionResult GetById([FromRoute] Guid id)
        {
            var result = _knowledgeBaseService.GetById(id);
            return Ok(result);
        }

        /// <summary>Deletes a specific knowledgebase by its primary key</summary>
        /// <param name="id">The primary key of the knowledgebase</param>
        /// <returns>The result of the operation</returns>
        [HttpDelete]
        [UserAuthorize("KnowledgeBase",Entitlements.Delete)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [Route("{id:Guid}")]
        public IActionResult DeleteById([FromRoute] Guid id)
        {
            var status = _knowledgeBaseService.Delete(id);
            return Ok(new { status });
        }

        /// <summary>Updates a specific knowledgebase by its primary key</summary>
        /// <param name="id">The primary key of the knowledgebase</param>
        /// <param name="updatedEntity">The knowledgebase data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPut]
        [UserAuthorize("KnowledgeBase",Entitlements.Update)]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult UpdateById(Guid id, [FromBody] KnowledgeBase updatedEntity)
        {
            if (id != updatedEntity.Id)
            {
                return BadRequest("Mismatched Id");
            }

            var status = _knowledgeBaseService.Update(id, updatedEntity);
            return Ok(new { status });
        }

        /// <summary>Updates a specific knowledgebase by its primary key</summary>
        /// <param name="id">The primary key of the knowledgebase</param>
        /// <param name="updatedEntity">The knowledgebase data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPatch]
        [UserAuthorize("KnowledgeBase",Entitlements.Update)]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult UpdateById(Guid id, [FromBody] JsonPatchDocument<KnowledgeBase> updatedEntity)
        {
            if (updatedEntity == null)
                return BadRequest("Patch document is missing.");
            var status = _knowledgeBaseService.Patch(id, updatedEntity);
            return Ok(new { status });
        }
    }
}