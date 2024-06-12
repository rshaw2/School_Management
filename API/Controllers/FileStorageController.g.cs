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
    /// Controller responsible for managing filestorage related operations.
    /// </summary>
    /// <remarks>
    /// This Controller provides endpoints for adding, retrieving, updating, and deleting filestorage information.
    /// </remarks>
    [Route("api/filestorage")]
    [Authorize]
    public class FileStorageController : ControllerBase
    {
        private readonly IFileStorageService _fileStorageService;

        /// <summary>
        /// Initializes a new instance of the FileStorageController class with the specified context.
        /// </summary>
        /// <param name="ifilestorageservice">The ifilestorageservice to be used by the controller.</param>
        public FileStorageController(IFileStorageService ifilestorageservice)
        {
            _fileStorageService = ifilestorageservice;
        }

        /// <summary>Adds a new filestorage</summary>
        /// <param name="model">The filestorage data to be added</param>
        /// <returns>The result of the operation</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [UserAuthorize("FileStorage",Entitlements.Create)]
        public IActionResult Post([FromBody] FileStorage model)
        {
            var id = _fileStorageService.Create(model);
            return Ok(new { id });
        }

        /// <summary>Retrieves a list of filestorages based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of filestorages</returns>
        [HttpGet]
        [UserAuthorize("FileStorage",Entitlements.Read)]
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

            var result = _fileStorageService.Get(filterCriteria, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return Ok(result);
        }

        /// <summary>Retrieves a specific filestorage by its primary key</summary>
        /// <param name="id">The primary key of the filestorage</param>
        /// <returns>The filestorage data</returns>
        [HttpGet]
        [Route("{id:Guid}")]
        [UserAuthorize("FileStorage",Entitlements.Read)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public IActionResult GetById([FromRoute] Guid id)
        {
            var result = _fileStorageService.GetById(id);
            return Ok(result);
        }

        /// <summary>Deletes a specific filestorage by its primary key</summary>
        /// <param name="id">The primary key of the filestorage</param>
        /// <returns>The result of the operation</returns>
        [HttpDelete]
        [UserAuthorize("FileStorage",Entitlements.Delete)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [Route("{id:Guid}")]
        public IActionResult DeleteById([FromRoute] Guid id)
        {
            var status = _fileStorageService.Delete(id);
            return Ok(new { status });
        }

        /// <summary>Updates a specific filestorage by its primary key</summary>
        /// <param name="id">The primary key of the filestorage</param>
        /// <param name="updatedEntity">The filestorage data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPut]
        [UserAuthorize("FileStorage",Entitlements.Update)]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult UpdateById(Guid id, [FromBody] FileStorage updatedEntity)
        {
            if (id != updatedEntity.Id)
            {
                return BadRequest("Mismatched Id");
            }

            var status = _fileStorageService.Update(id, updatedEntity);
            return Ok(new { status });
        }

        /// <summary>Updates a specific filestorage by its primary key</summary>
        /// <param name="id">The primary key of the filestorage</param>
        /// <param name="updatedEntity">The filestorage data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPatch]
        [UserAuthorize("FileStorage",Entitlements.Update)]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult UpdateById(Guid id, [FromBody] JsonPatchDocument<FileStorage> updatedEntity)
        {
            if (updatedEntity == null)
                return BadRequest("Patch document is missing.");
            var status = _fileStorageService.Patch(id, updatedEntity);
            return Ok(new { status });
        }
    }
}