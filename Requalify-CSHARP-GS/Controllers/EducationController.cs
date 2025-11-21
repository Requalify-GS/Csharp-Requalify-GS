using Microsoft.AspNetCore.Mvc;
using Requalify.DTOs.Requests;
using Requalify.DTOs.Responses;
using Requalify.Exceptions;
using Requalify.Hateoas;
using Requalify.Mappers;
using Requalify.Model;
using Requalify.Services.Abstractions;

namespace Requalify.Controllers.v1
{
    /// <summary>
    /// Controller responsible for managing Education records [api/v3].
    /// </summary>
    [ApiExplorerSettings(GroupName = "v3")]
    [ApiController]
    [ApiVersion("3.0")]
    [Route("api/v{version:apiVersion}/education")]
    public class EducationController : ControllerBase
    {
        private readonly IEducationService _educationService;
        private readonly LinkGenerator _linkGenerator;
        private readonly ILogger _logger;

        public EducationController(IEducationService educationService, LinkGenerator linkGenerator, ILogger<EducationController> logger)
        {
            _educationService = educationService;
            _linkGenerator = linkGenerator;
            _logger = logger;
        }

        /// <summary>
        /// Returns a paginated list of all education entries of a user.
        /// </summary>
        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(PagedResponse<EducationResponse>), 200)]
        public async Task<ActionResult<PagedResponse<EducationResponse>>> GetByUser(
            int userId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("Fetching education records for UserId {userId}", userId);

            var version = HttpContext.GetRequestedApiVersion()?.ToString() ?? "3.0";
            var educations = await _educationService.GetByUserIdAsync(userId);

            _logger.LogInformation("{count} education entries found for UserId {userId}", educations.Count(), userId);

            var totalCount = educations.Count();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var items = educations
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(e =>
                {
                    var resp = e.ToResponse();
                    resp.AddLink("self", _linkGenerator.GetPathByAction("GetById", "Education", new { version, id = e.Id })!, "GET");
                    resp.AddLink("update", _linkGenerator.GetPathByAction("Update", "Education", new { version, id = e.Id })!, "PUT");
                    resp.AddLink("delete", _linkGenerator.GetPathByAction("Delete", "Education", new { version, id = e.Id })!, "DELETE");
                    return resp;
                })
                .ToList();

            var paged = new PagedResponse<EducationResponse>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };

            paged.AddLink("self", _linkGenerator.GetPathByAction("GetByUser", "Education", new { version, userId, pageNumber, pageSize })!, "GET");

            return Ok(paged);
        }

        /// <summary>
        /// Returns all education entries (paginated).
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<EducationResponse>), 200)]
        public async Task<ActionResult<PagedResponse<EducationResponse>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("Fetching all education entries");

            var version = HttpContext.GetRequestedApiVersion()?.ToString() ?? "3.0";
            var educations = await _educationService.GetAllAsync();

            _logger.LogInformation("{count} education records retrieved", educations.Count());

            var totalCount = educations.Count();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var items = educations
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(e =>
                {
                    var resp = e.ToResponse();
                    resp.AddLink("self", _linkGenerator.GetPathByAction("GetById", "Education", new { version, id = e.Id })!, "GET");
                    resp.AddLink("update", _linkGenerator.GetPathByAction("Update", "Education", new { version, id = e.Id })!, "PUT");
                    resp.AddLink("delete", _linkGenerator.GetPathByAction("Delete", "Education", new { version, id = e.Id })!, "DELETE");
                    return resp;
                })
                .ToList();

            var paged = new PagedResponse<EducationResponse>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };

            paged.AddLink("self", _linkGenerator.GetPathByAction("GetAll", "Education", new { version, pageNumber, pageSize })!, "GET");

            return Ok(paged);
        }


        /// <summary>
        /// Returns an education entry by ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(EducationResponse), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<EducationResponse>> GetById(int id)
        {
            _logger.LogInformation("Fetching education entry with ID {id}", id);

            try
            {
                var education = await _educationService.GetByIdAsync(id);
                var version = HttpContext.GetRequestedApiVersion()?.ToString() ?? "3.0";

                var resp = education.ToResponse();
                resp.AddLink("self", _linkGenerator.GetPathByAction("GetById", "Education", new { version, id })!, "GET");

                return Ok(resp);
            }
            catch (EducationNotFoundException ex)
            {
                _logger.LogWarning("Education entry with ID {id} not found: {msg}", id, ex.Message);
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Creates an education entry.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(EducationResponse), 201)]
        public async Task<ActionResult<EducationResponse>> Create(CreateEducationRequest request)
        {
            _logger.LogInformation("Creating new education entry for UserId {userId}", request.UserId);

            var education = await _educationService.CreateAsync(request);
            var version = HttpContext.GetRequestedApiVersion()?.ToString() ?? "3.0";

            _logger.LogInformation("Education entry created with ID {id}", education.Id);

            var response = education.ToResponse();
            response.AddLink("self", _linkGenerator.GetPathByAction("GetById", "Education", new { version, id = education.Id })!, "GET");
            response.AddLink("update", _linkGenerator.GetPathByAction("Update", "Education", new { version, id = education.Id })!, "PUT");
            response.AddLink("delete", _linkGenerator.GetPathByAction("Delete", "Education", new { version, id = education.Id })!, "DELETE");
            return CreatedAtAction(nameof(GetById), new { id = education.Id }, response);
        }

        /// <summary>
        /// Updates an education entry.
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, UpdateEducationRequest request)
        {
            _logger.LogInformation("Updating education entry with ID {id}", id);

            try
            {
                await _educationService.UpdateAsync(id, request);
                return NoContent();
            }
            catch (EducationNotFoundException ex)
            {
                _logger.LogWarning("Update failed for education ID {id}: {msg}", id, ex.Message);
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Deletes an education entry.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Deleting education entry with ID {id}", id);

            try
            {
                await _educationService.DeleteAsync(id);
                return NoContent();
            }
            catch (EducationNotFoundException ex)
            {
                _logger.LogWarning("Deletion failed for education ID {id}: {msg}", id, ex.Message);
                return NotFound(ex.Message);
            }
        }
    }
}
