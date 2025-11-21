using Microsoft.AspNetCore.Mvc;
using Requalify.DTOs.Requests;
using Requalify.DTOs.Responses;
using Requalify.Exceptions;
using Requalify.Hateoas;
using Requalify.Mappers;
using Requalify.Services;
using Requalify.Services.Abstractions;

namespace Requalify.Controllers.v1
{
    /// <summary>
    /// Controller responsible for managing Skills [api/v2].
    /// </summary>
    [ApiExplorerSettings(GroupName = "v2")]
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/skills")]
    public class SkillController : ControllerBase
    {
        private readonly ISkillService _skillService;
        private readonly LinkGenerator _linkGenerator;
        private readonly ILogger _logger;

        public SkillController(ISkillService skillService, LinkGenerator linkGenerator, ILogger<SkillController> logger)
        {
            _skillService = skillService;
            _linkGenerator = linkGenerator;
            _logger = logger;
        }

        /// <summary>
        /// Returns a paginated list of all skills.
        /// </summary>
        /// <param name="pageNumber">Page number (default = 1).</param>
        /// <param name="pageSize">Number of items per page (default = 10).</param>
        /// <returns>A paginated response containing skills and HATEOAS links.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<SkillResponse>), 200)]
        public async Task<ActionResult<PagedResponse<SkillResponse>>> GetAll(int pageNumber = 1, int pageSize = 10)
        {
            _logger.LogInformation("GET /skills?pageNumber={page}&pageSize={size}", pageNumber, pageSize);

            var version = HttpContext.GetRequestedApiVersion()?.ToString() ?? "2.0";
            var skills = await _skillService.GetAllAsync();

            _logger.LogInformation("{count} skills retrieved", skills.Count());

            var totalCount = skills.Count();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var items = skills
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(s =>
                {
                    var response = s.ToResponse();
                    response.AddLink("self", _linkGenerator.GetPathByAction("GetById", "Skill", new { version, id = s.Id })!, "GET");
                    response.AddLink("update", _linkGenerator.GetPathByAction("Update", "Skill", new { version, id = s.Id })!, "PUT");
                    response.AddLink("delete", _linkGenerator.GetPathByAction("Delete", "Skill", new { version, id = s.Id })!, "DELETE");

                    return response;
                })
                .ToList();

            var pagedResponse = new PagedResponse<SkillResponse>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };

            pagedResponse.AddLink("self", _linkGenerator.GetPathByAction("GetAll", "Skill", new { version, pageNumber, pageSize })!, "GET");

            return Ok(pagedResponse);
        }

        /// <summary>
        /// Returns a paginated list of all education entries of a user.
        /// </summary>
        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(PagedResponse<SkillResponse>), 200)]
        public async Task<ActionResult<PagedResponse<SkillResponse>>> GetByUser(int userId, int pageNumber = 1, int pageSize = 10)
        {
            _logger.LogInformation("GET /skills/user/{userId}", userId);

            var version = HttpContext.GetRequestedApiVersion()?.ToString() ?? "2.0";
            var skills = await _skillService.GetByUserIdAsync(userId);

            _logger.LogInformation("{count} skills retrieved for UserId {userId}", skills.Count(), userId);

            var totalCount = skills.Count();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var items = skills
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(e =>
                {
                    var resp = e.ToResponse();
                    resp.AddLink("self", _linkGenerator.GetPathByAction("GetById", "Skill", new { version, id = e.Id })!, "GET");
                    resp.AddLink("update", _linkGenerator.GetPathByAction("Update", "Skill", new { version, id = e.Id })!, "PUT");
                    resp.AddLink("delete", _linkGenerator.GetPathByAction("Delete", "Skill", new { version, id = e.Id })!, "DELETE");
                    return resp;
                })
                .ToList();

            var paged = new PagedResponse<SkillResponse>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };

            paged.AddLink("self", _linkGenerator.GetPathByAction("GetByUser", "Skill", new { version, userId, pageNumber, pageSize })!, "GET");

            return Ok(paged);
        }

        /// <summary>
        /// Returns a specific skill by ID.
        /// </summary>
        /// <param name="id">Skill ID.</param>
        /// <returns>The skill response or 404 if not found.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SkillResponse), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<SkillResponse>> GetById(int id)
        {
            _logger.LogInformation("GET /skills/{id}", id);

            try
            {
                var skill = await _skillService.GetByIdAsync(id);
                var version = HttpContext.GetRequestedApiVersion()?.ToString() ?? "2.0";

                var response = skill.ToResponse();
                response.AddLink("self", _linkGenerator.GetPathByAction("GetById", "Skill", new { version, id })!, "GET");

                return Ok(response);
            }
            catch (SkillNotFoundException ex)
            {
                _logger.LogWarning("Skill {id} not found: {msg}", id, ex.Message);
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Creates a new skill.
        /// </summary>
        /// <param name="request">Skill creation payload.</param>
        /// <returns>The created skill with HATEOAS links.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(SkillResponse), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<SkillResponse>> Create(CreateSkillRequest request)
        {
            _logger.LogInformation("POST /skills");

            var skill = await _skillService.CreateAsync(request);

            _logger.LogInformation("Skill {id} created successfully", skill.Id);

            var version = HttpContext.GetRequestedApiVersion()?.ToString() ?? "2.0";
            var response = skill.ToResponse();

            response.AddLink("self", _linkGenerator.GetPathByAction("GetById", "Skill", new { version, id = skill.Id })!, "GET");
            response.AddLink("update", _linkGenerator.GetPathByAction("Update", "Skill", new { version, id = skill.Id })!, "PUT");
            response.AddLink("delete", _linkGenerator.GetPathByAction("Delete", "Skill", new { version, id = skill.Id })!, "DELETE");

            return CreatedAtAction(nameof(GetById), new { id = skill.Id }, response);
        }

        /// <summary>
        /// Updates an existing skill.
        /// </summary>
        /// <param name="id">Skill ID.</param>
        /// <param name="request">Payload containing updated fields.</param>
        /// <returns>204 if updated successfully or 404 if not found.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, UpdateSkillRequest request)
        {
            _logger.LogInformation("PUT /skills/{id}", id);

            try
            {
                await _skillService.UpdateAsync(id, request);
                _logger.LogInformation("Skill {id} updated successfully", id);
                return NoContent();
            }
            catch (SkillNotFoundException ex)
            {
                _logger.LogWarning("Update failed for Skill {id}: {msg}", id, ex.Message);
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Deletes a skill.
        /// </summary>
        /// <param name="id">Skill ID.</param>
        /// <returns>204 if successfully deleted or 404 if not found.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("DELETE /skills/{id}", id);

            try
            {
                await _skillService.DeleteAsync(id);
                _logger.LogInformation("Skill {id} deleted successfully", id);
                return NoContent();
            }
            catch (SkillNotFoundException ex)
            {
                _logger.LogWarning("Delete failed for Skill {id}: {msg}", id, ex.Message);
                return NotFound(ex.Message);
            }
        }
    }
}
