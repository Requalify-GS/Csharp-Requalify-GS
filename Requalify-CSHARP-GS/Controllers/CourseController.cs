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
    /// Controller responsible for managing Courses [api/v4].
    /// </summary>
    [ApiExplorerSettings(GroupName = "v4")]
    [ApiController]
    [ApiVersion("4.0")]
    [Route("api/v{version:apiVersion}/courses")]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;
        private readonly LinkGenerator _linkGenerator;

        public CourseController(ICourseService courseService, LinkGenerator linkGenerator)
        {
            _courseService = courseService;
            _linkGenerator = linkGenerator;
        }

        /// <summary>
        /// Returns a paginated list of all courses.
        /// </summary>
        /// <param name="pageNumber">Page number (default = 1).</param>
        /// <param name="pageSize">Number of items per page (default = 10).</param>
        /// <returns>A paginated response containing courses and HATEOAS links.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<CourseResponse>), 200)]
        public async Task<ActionResult<PagedResponse<CourseResponse>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var version = HttpContext.GetRequestedApiVersion()?.ToString() ?? "4.0";

            var courses = await _courseService.GetAllAsync();
            var totalCount = courses.Count();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var items = courses
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(s =>
                {
                    var response = s.ToResponse();
                    response.AddLink("self", _linkGenerator.GetPathByAction("GetById", "Course", new { version, id = s.Id })!, "GET");
                    response.AddLink("update", _linkGenerator.GetPathByAction("Update", "Course", new { version, id = s.Id })!, "PUT");
                    response.AddLink("delete", _linkGenerator.GetPathByAction("Delete", "Course", new { version, id = s.Id })!, "DELETE");
                    return response;
                })
                .ToList();

            var pagedResponse = new PagedResponse<CourseResponse>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                Links = new List<LinkDto>
                {
                    new LinkDto("self", _linkGenerator.GetPathByAction("GetAll", "Course", new { version, pageNumber, pageSize })!, "GET"),
                    new LinkDto("next", pageNumber < totalPages ? _linkGenerator.GetPathByAction("GetAll", "Course", new { version, pageNumber = pageNumber + 1, pageSize })! : null, "GET"),
                    new LinkDto("prev", pageNumber > 1 ? _linkGenerator.GetPathByAction("GetAll", "Course   ", new { version, pageNumber = pageNumber - 1, pageSize })! : null, "GET")
                }
            };

            return Ok(pagedResponse);
        }

        /// <summary>
        /// Returns a paginated list of all courses from a specific user.
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <param name="pageNumber">Page number (default = 1).</param>
        /// <param name="pageSize">Items per page (default = 10).</param>
        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(PagedResponse<CourseResponse>), 200)]
        public async Task<ActionResult<PagedResponse<CourseResponse>>> GetByUser(
            int userId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var version = HttpContext.GetRequestedApiVersion()?.ToString() ?? "4.0";

            var courses = await _courseService.GetByUserIdAsync(userId);

            var totalCount = courses.Count();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var items = courses
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(c =>
                {
                    var resp = c.ToResponse();
                    resp.AddLink("self", _linkGenerator.GetPathByAction("GetById", "Course", new { version, id = c.Id })!, "GET");
                    resp.AddLink("update", _linkGenerator.GetPathByAction("Update", "Course", new { version, id = c.Id })!, "PUT");
                    resp.AddLink("delete", _linkGenerator.GetPathByAction("Delete", "Course", new { version, id = c.Id })!, "DELETE");
                    return resp;
                }).ToList();

            var paged = new PagedResponse<CourseResponse>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };

            paged.AddLink("self", _linkGenerator.GetPathByAction("GetByUser", "Course", new { version, userId, pageNumber, pageSize })!, "GET");
            paged.AddLink("next", pageNumber < totalPages ? _linkGenerator.GetPathByAction("GetByUser", "Course", new { version, userId, pageNumber = pageNumber + 1, pageSize })! : null, "GET");
            paged.AddLink("prev", pageNumber > 1 ? _linkGenerator.GetPathByAction("GetByUser", "Course", new { version, userId, pageNumber = pageNumber - 1, pageSize })! : null, "GET");

            return Ok(paged);
        }

        /// <summary>
        /// Returns a course by its ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CourseResponse), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<CourseResponse>> GetById(int id)
        {
            try
            {
                var course = await _courseService.GetByIdAsync(id);
                var version = HttpContext.GetRequestedApiVersion()?.ToString() ?? "4.0";

                var response = course.ToResponse();
                response.AddLink("self", _linkGenerator.GetPathByAction("GetById", "Course", new { version, id })!, "GET");
                response.AddLink("update", _linkGenerator.GetPathByAction("Update", "Course", new { version, id })!, "PUT");
                response.AddLink("delete", _linkGenerator.GetPathByAction("Delete", "Course", new { version, id })!, "DELETE");

                return Ok(response);
            }
            catch (CourseNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Creates a new course.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(CourseResponse), 201)]
        public async Task<ActionResult<CourseResponse>> Create(CreateCourseRequest request)
        {
            var course = await _courseService.CreateAsync(request);

            var version = HttpContext.GetRequestedApiVersion()?.ToString() ?? "4.0";

            var resp = course.ToResponse();
            resp.AddLink("self", _linkGenerator.GetPathByAction("GetById", "Course", new { version, id = course.Id })!, "GET");
            resp.AddLink("update", _linkGenerator.GetPathByAction("Update", "Course", new { version, id = course.Id })!, "PUT");
            resp.AddLink("delete", _linkGenerator.GetPathByAction("Delete", "Course", new { version, id = course.Id })!, "DELETE");

            return CreatedAtAction(nameof(GetById), new { id = course.Id }, resp);
        }

        /// <summary>
        /// Updates an existing course.
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, UpdateCourseRequest request)
        {
            try
            {
                await _courseService.UpdateAsync(id, request);
                return NoContent();
            }
            catch (CourseNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Deletes a course.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _courseService.DeleteAsync(id);
                return NoContent();
            }
            catch (CourseNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
