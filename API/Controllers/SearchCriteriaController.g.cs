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
    /// Controller responsible for managing searchcriteria related operations.
    /// </summary>
    /// <remarks>
    /// This Controller provides endpoints for adding, retrieving, updating, and deleting searchcriteria information.
    /// </remarks>
    [Route("api/searchcriteria")]
    [Authorize]
    public class SearchCriteriaController : ControllerBase
    {
        private readonly ISearchCriteriaService _searchCriteriaService;

        /// <summary>
        /// Initializes a new instance of the SearchCriteriaController class with the specified context.
        /// </summary>
        /// <param name="isearchcriteriaservice">The isearchcriteriaservice to be used by the controller.</param>
        public SearchCriteriaController(ISearchCriteriaService isearchcriteriaservice)
        {
            _searchCriteriaService = isearchcriteriaservice;
        }

        /// <summary>Adds a new searchcriteria</summary>
        /// <param name="model">The searchcriteria data to be added</param>
        /// <returns>The result of the operation</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [UserAuthorize("SearchCriteria",Entitlements.Create)]
        public IActionResult Post([FromBody] SearchCriteria model)
        {
            var id = _searchCriteriaService.Create(model);
            return Ok(new { id });
        }

        /// <summary>Retrieves a list of searchcriterias based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of searchcriterias</returns>
        [HttpGet]
        [UserAuthorize("SearchCriteria",Entitlements.Read)]
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

            var result = _searchCriteriaService.Get(filterCriteria, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return Ok(result);
        }

        /// <summary>Retrieves a specific searchcriteria by its primary key</summary>
        /// <param name="id">The primary key of the searchcriteria</param>
        /// <returns>The searchcriteria data</returns>
        [HttpGet]
        [Route("{id:Guid}")]
        [UserAuthorize("SearchCriteria",Entitlements.Read)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public IActionResult GetById([FromRoute] Guid id)
        {
            var result = _searchCriteriaService.GetById(id);
            return Ok(result);
        }

        /// <summary>Deletes a specific searchcriteria by its primary key</summary>
        /// <param name="id">The primary key of the searchcriteria</param>
        /// <returns>The result of the operation</returns>
        [HttpDelete]
        [UserAuthorize("SearchCriteria",Entitlements.Delete)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [Route("{id:Guid}")]
        public IActionResult DeleteById([FromRoute] Guid id)
        {
            var status = _searchCriteriaService.Delete(id);
            return Ok(new { status });
        }

        /// <summary>Updates a specific searchcriteria by its primary key</summary>
        /// <param name="id">The primary key of the searchcriteria</param>
        /// <param name="updatedEntity">The searchcriteria data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPut]
        [UserAuthorize("SearchCriteria",Entitlements.Update)]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult UpdateById(Guid id, [FromBody] SearchCriteria updatedEntity)
        {
            if (id != updatedEntity.Id)
            {
                return BadRequest("Mismatched Id");
            }

            var status = _searchCriteriaService.Update(id, updatedEntity);
            return Ok(new { status });
        }

        /// <summary>Updates a specific searchcriteria by its primary key</summary>
        /// <param name="id">The primary key of the searchcriteria</param>
        /// <param name="updatedEntity">The searchcriteria data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPatch]
        [UserAuthorize("SearchCriteria",Entitlements.Update)]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult UpdateById(Guid id, [FromBody] JsonPatchDocument<SearchCriteria> updatedEntity)
        {
            if (updatedEntity == null)
                return BadRequest("Patch document is missing.");
            var status = _searchCriteriaService.Patch(id, updatedEntity);
            return Ok(new { status });
        }
    }
}