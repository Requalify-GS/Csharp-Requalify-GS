using Microsoft.AspNetCore.Mvc;
using Requalify.DTOs.Requests;
using Requalify.DTOs.Responses;
using Requalify.Exceptions;
using Requalify.Hateoas;
using Requalify.Mappers;
using Requalify.Model;
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
        private readonly ILogger _logger;

        public CourseController(ICourseService courseService, LinkGenerator linkGenerator, ILogger<CourseController> logger)
        {
            _courseService = courseService;
            _linkGenerator = linkGenerator;
            _logger = logger;
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
            _logger.LogInformation("Fetching all courses with pagination: page {pageNumber}, size {pageSize}", pageNumber, pageSize);

            var version = HttpContext.GetRequestedApiVersion()?.ToString() ?? "4.0";
            var courses = await _courseService.GetAllAsync();

            _logger.LogInformation("{count} courses retrieved", courses.Count());

            var totalCount = courses.Count();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var items = courses
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(c =>
                {
                    var response = c.ToResponse();
                    response.AddLink("self", _linkGenerator.GetPathByAction("GetById", "Course", new { version, id = c.Id })!, "GET");
                    response.AddLink("update", _linkGenerator.GetPathByAction("Update", "Course", new { version, id = c.Id })!, "PUT");
                    response.AddLink("delete", _linkGenerator.GetPathByAction("Delete", "Course", new { version, id = c.Id })!, "DELETE");
                    return response;
                })
                .ToList();

            var paged = new PagedResponse<CourseResponse>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };

            paged.AddLink("self", _linkGenerator.GetPathByAction("GetAll", "Course", new { version, pageNumber, pageSize })!, "GET");

            return Ok(paged);
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
            _logger.LogInformation("Fetching courses for userId {userId}", userId);

            var version = HttpContext.GetRequestedApiVersion()?.ToString() ?? "4.0";
            var courses = await _courseService.GetByUserIdAsync(userId);

            _logger.LogInformation("{count} courses found for userId {userId}", courses.Count(), userId);

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
                })
                .ToList();

            var paged = new PagedResponse<CourseResponse>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };

            paged.AddLink("self", _linkGenerator.GetPathByAction("GetByUser", "Course", new { version, userId, pageNumber, pageSize })!, "GET");

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
            _logger.LogInformation("Fetching course with id {id}", id);

            try
            {
                var course = await _courseService.GetByIdAsync(id);
                var version = HttpContext.GetRequestedApiVersion()?.ToString() ?? "4.0";

                var resp = course.ToResponse();
                resp.AddLink("self", _linkGenerator.GetPathByAction("GetById", "Course", new { version, id })!, "GET");
                resp.AddLink("update", _linkGenerator.GetPathByAction("Update", "Course", new { version, id })!, "PUT");
                resp.AddLink("delete", _linkGenerator.GetPathByAction("Delete", "Course", new { version, id })!, "DELETE");

                return Ok(resp);
            }
            catch (CourseNotFoundException ex)
            {
                _logger.LogWarning("Course id {id} not found: {message}", id, ex.Message);
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
            _logger.LogInformation("Creating a new course for userId {userId}", request.UserId);

            var course = await _courseService.CreateAsync(request);
            var version = HttpContext.GetRequestedApiVersion()?.ToString() ?? "4.0";

            _logger.LogInformation("Course created successfully with id {id}", course.Id);

            var response = course.ToResponse();
            response.AddLink("self", _linkGenerator.GetPathByAction("GetById", "Course", new { version, id = course.Id })!, "GET");
            response.AddLink("update", _linkGenerator.GetPathByAction("Update", "Course", new { version, id = course.Id })!, "PUT");
            response.AddLink("delete", _linkGenerator.GetPathByAction("Delete", "Course", new { version, id = course.Id })!, "DELETE");
            return CreatedAtAction(nameof(GetById), new { id = course.Id }, response);
        }

        /// <summary>
        /// Updates an existing course.
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, UpdateCourseRequest request)
        {
            _logger.LogInformation("Updating course with id {id}", id);

            try
            {
                await _courseService.UpdateAsync(id, request);
                _logger.LogInformation("Course id {id} updated successfully", id);
                return NoContent();
            }
            catch (CourseNotFoundException ex)
            {
                _logger.LogWarning("Course update failed for id {id}: {message}", id, ex.Message);
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
            _logger.LogInformation("Deleting course with id {id}", id);

            try
            {
                await _courseService.DeleteAsync(id);
                _logger.LogInformation("Course id {id} deleted successfully", id);
                return NoContent();
            }
            catch (CourseNotFoundException ex)
            {
                _logger.LogWarning("Course deletion failed for id {id}: {message}", id, ex.Message);
                return NotFound(ex.Message);
            }
        }
    }
}
