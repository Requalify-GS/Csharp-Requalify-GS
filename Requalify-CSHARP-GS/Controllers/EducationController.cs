using Microsoft.AspNetCore.Mvc;
using Requalify.DTOs.Requests;
using Requalify.DTOs.Responses;
using Requalify.Exceptions;
using Requalify.Hateoas;
using Requalify.Mappers;
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

        public EducationController(IEducationService educationService, LinkGenerator linkGenerator)
        {
            _educationService = educationService;
            _linkGenerator = linkGenerator;
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
            var version = HttpContext.GetRequestedApiVersion()?.ToString() ?? "3.0";

            var educations = await _educationService.GetByUserIdAsync(userId);

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
            paged.AddLink("next", pageNumber < totalPages ? _linkGenerator.GetPathByAction("GetByUser", "Education", new { version, userId, pageNumber = pageNumber + 1, pageSize })! : null, "GET");
            paged.AddLink("prev", pageNumber > 1 ? _linkGenerator.GetPathByAction("GetByUser", "Education", new { version, userId, pageNumber = pageNumber - 1, pageSize })! : null, "GET");

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
            var version = HttpContext.GetRequestedApiVersion()?.ToString() ?? "3.0";

            var educations = await _educationService.GetAllAsync();

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
            paged.AddLink("next", pageNumber < totalPages ? _linkGenerator.GetPathByAction("GetAll", "Education", new { version, pageNumber = pageNumber + 1, pageSize })! : null, "GET");
            paged.AddLink("prev", pageNumber > 1 ? _linkGenerator.GetPathByAction("GetAll", "Education", new { version, pageNumber = pageNumber - 1, pageSize })! : null, "GET");

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
            try
            {
                var education = await _educationService.GetByIdAsync(id);
                var version = HttpContext.GetRequestedApiVersion()?.ToString() ?? "3.0";

                var resp = education.ToResponse();
                resp.AddLink("self", _linkGenerator.GetPathByAction("GetById", "Education", new { version, id })!, "GET");
                resp.AddLink("update", _linkGenerator.GetPathByAction("Update", "Education", new { version, id })!, "PUT");
                resp.AddLink("delete", _linkGenerator.GetPathByAction("Delete", "Education", new { version, id })!, "DELETE");

                return Ok(resp);
            }
            catch (EducationNotFoundException ex)
            {
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
            var result = await _educationService.CreateAsync(request);
            var version = HttpContext.GetRequestedApiVersion()?.ToString() ?? "3.0";

            var resp = result.ToResponse();
            resp.AddLink("self", _linkGenerator.GetPathByAction("GetById", "Education", new { version, id = result.Id })!, "GET");
            resp.AddLink("update", _linkGenerator.GetPathByAction("Update", "Education", new { version, id = result.Id })!, "PUT");
            resp.AddLink("delete", _linkGenerator.GetPathByAction("Delete", "Education", new { version, id = result.Id })!, "DELETE");

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, resp);
        }

        /// <summary>
        /// Updates an education entry.
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, UpdateEducationRequest request)
        {
            try
            {
                await _educationService.UpdateAsync(id, request);
                return NoContent();
            }
            catch (EducationNotFoundException ex)
            {
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
            try
            {
                await _educationService.DeleteAsync(id);
                return NoContent();
            }
            catch (EducationNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
