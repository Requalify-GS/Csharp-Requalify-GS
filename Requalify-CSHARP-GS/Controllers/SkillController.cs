using Requalify.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;
using Requalify.DTOs.Requests;
using Requalify.Exceptions;
using Requalify.Hateoas;
using Requalify.Mappers;
using Requalify.Services.Abstractions;

namespace Requalify.Controllers.v1
{
    /// <summary>
    /// Controller responsible for managing Skills [api/v2].
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/skills")]
    public class SkillController : ControllerBase
    {
        private readonly ISkillService _skillService;
        private readonly LinkGenerator _linkGenerator;

        public SkillController(ISkillService skillService, LinkGenerator linkGenerator)
        {
            _skillService = skillService;
            _linkGenerator = linkGenerator;
        }

        /// <summary>
        /// Returns a paginated list of all skills.
        /// </summary>
        /// <param name="pageNumber">Page number (default = 1).</param>
        /// <param name="pageSize">Number of items per page (default = 10).</param>
        /// <returns>A paginated response containing skills and HATEOAS links.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<SkillResponse>), 200)]
        public async Task<ActionResult<PagedResponse<SkillResponse>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var version = HttpContext.GetRequestedApiVersion()?.ToString() ?? "2.0";

            var skills = await _skillService.GetByUserIdAsync(0); // Caso queira listar todas, ajuste o service
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
                TotalPages = totalPages,
                Links = new List<LinkDto>
                {
                    new LinkDto("self", _linkGenerator.GetPathByAction("GetAll", "Skill", new { version, pageNumber, pageSize })!, "GET"),
                    new LinkDto("next", pageNumber < totalPages ? _linkGenerator.GetPathByAction("GetAll", "Skill", new { version, pageNumber = pageNumber + 1, pageSize })! : null, "GET"),
                    new LinkDto("prev", pageNumber > 1 ? _linkGenerator.GetPathByAction("GetAll", "Skill", new { version, pageNumber = pageNumber - 1, pageSize })! : null, "GET")
                }
            };

            return Ok(pagedResponse);
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
            try
            {
                var skill = await _skillService.GetByIdAsync(id);
                var version = HttpContext.GetRequestedApiVersion()?.ToString() ?? "2.0";

                var response = skill.ToResponse();
                response.AddLink("self", _linkGenerator.GetPathByAction("GetById", "Skill", new { version, id })!, "GET");
                response.AddLink("update", _linkGenerator.GetPathByAction("Update", "Skill", new { version, id })!, "PUT");
                response.AddLink("delete", _linkGenerator.GetPathByAction("Delete", "Skill", new { version, id })!, "DELETE");

                return Ok(response);
            }
            catch (SkillNotFoundException ex)
            {
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
            var skill = await _skillService.CreateAsync(request);
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
            try
            {
                await _skillService.UpdateAsync(id, request);
                return NoContent();
            }
            catch (SkillNotFoundException ex)
            {
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
            try
            {
                await _skillService.DeleteAsync(id);
                return NoContent();
            }
            catch (SkillNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
