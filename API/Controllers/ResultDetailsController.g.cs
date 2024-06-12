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
    /// Controller responsible for managing resultdetails related operations.
    /// </summary>
    /// <remarks>
    /// This Controller provides endpoints for adding, retrieving, updating, and deleting resultdetails information.
    /// </remarks>
    [Route("api/resultdetails")]
    [Authorize]
    public class ResultDetailsController : ControllerBase
    {
        private readonly IResultDetailsService _resultDetailsService;

        /// <summary>
        /// Initializes a new instance of the ResultDetailsController class with the specified context.
        /// </summary>
        /// <param name="iresultdetailsservice">The iresultdetailsservice to be used by the controller.</param>
        public ResultDetailsController(IResultDetailsService iresultdetailsservice)
        {
            _resultDetailsService = iresultdetailsservice;
        }

        /// <summary>Adds a new resultdetails</summary>
        /// <param name="model">The resultdetails data to be added</param>
        /// <returns>The result of the operation</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [UserAuthorize("ResultDetails",Entitlements.Create)]
        public IActionResult Post([FromBody] ResultDetails model)
        {
            var id = _resultDetailsService.Create(model);
            return Ok(new { id });
        }

        /// <summary>Retrieves a list of resultdetailss based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of resultdetailss</returns>
        [HttpGet]
        [UserAuthorize("ResultDetails",Entitlements.Read)]
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

            var result = _resultDetailsService.Get(filterCriteria, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return Ok(result);
        }

        /// <summary>Retrieves a specific resultdetails by its primary key</summary>
        /// <param name="id">The primary key of the resultdetails</param>
        /// <returns>The resultdetails data</returns>
        [HttpGet]
        [Route("{id:Guid}")]
        [UserAuthorize("ResultDetails",Entitlements.Read)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public IActionResult GetById([FromRoute] Guid id)
        {
            var result = _resultDetailsService.GetById(id);
            return Ok(result);
        }

        /// <summary>Deletes a specific resultdetails by its primary key</summary>
        /// <param name="id">The primary key of the resultdetails</param>
        /// <returns>The result of the operation</returns>
        [HttpDelete]
        [UserAuthorize("ResultDetails",Entitlements.Delete)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [Route("{id:Guid}")]
        public IActionResult DeleteById([FromRoute] Guid id)
        {
            var status = _resultDetailsService.Delete(id);
            return Ok(new { status });
        }

        /// <summary>Updates a specific resultdetails by its primary key</summary>
        /// <param name="id">The primary key of the resultdetails</param>
        /// <param name="updatedEntity">The resultdetails data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPut]
        [UserAuthorize("ResultDetails",Entitlements.Update)]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult UpdateById(Guid id, [FromBody] ResultDetails updatedEntity)
        {
            if (id != updatedEntity.Id)
            {
                return BadRequest("Mismatched Id");
            }

            var status = _resultDetailsService.Update(id, updatedEntity);
            return Ok(new { status });
        }

        /// <summary>Updates a specific resultdetails by its primary key</summary>
        /// <param name="id">The primary key of the resultdetails</param>
        /// <param name="updatedEntity">The resultdetails data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPatch]
        [UserAuthorize("ResultDetails",Entitlements.Update)]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult UpdateById(Guid id, [FromBody] JsonPatchDocument<ResultDetails> updatedEntity)
        {
            if (updatedEntity == null)
                return BadRequest("Patch document is missing.");
            var status = _resultDetailsService.Patch(id, updatedEntity);
            return Ok(new { status });
        }
    }
}