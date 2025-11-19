using Microsoft.AspNetCore.Mvc;
using Requalify.Services.Abstractions;
using Requalify.DTOs.Requests;
using Requalify.DTOs.Responses;
using Requalify.Mappers;
using Requalify.Exceptions;
using Requalify.Hateoas;

namespace Requalify.Controllers
{
    /// <summary>
    /// Controller responsible for managing Users [api/v1].
    /// </summary>
    [ApiExplorerSettings(GroupName = "v1")]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly LinkGenerator _linkGenerator;

        public UserController(IUserService userService, LinkGenerator linkGenerator)
        {
            _userService = userService;
            _linkGenerator = linkGenerator;
        }

        /// <summary>
        /// Returns a paginated list of all users.
        /// </summary>
        /// <param name="pageNumber">Page number (default = 1).</param>
        /// <param name="pageSize">Items per page (default = 10).</param>
        /// <returns>A paginated response with HATEOAS links.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<UserResponse>), 200)]
        public async Task<ActionResult<PagedResponse<UserResponse>>> GetAll(
      [FromQuery] int pageNumber = 1,
      [FromQuery] int pageSize = 10)
        {
            var version = HttpContext.GetRequestedApiVersion()?.ToString() ?? "1.0";

            var users = await _userService.GetAllAsync();

            var totalCount = users.Count();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var items = users
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(u =>
                {
                    var response = u.ToResponse();

                    // ================= USER LINKS =================
                    response.AddLink("self", _linkGenerator.GetPathByAction(
                        "GetById", "User", new { version, id = u.Id })!, "GET");

                    response.AddLink("update", _linkGenerator.GetPathByAction(
                        "Update", "User", new { version, id = u.Id })!, "PUT");

                    response.AddLink("delete", _linkGenerator.GetPathByAction(
                        "Delete", "User", new { version, id = u.Id })!, "DELETE");


                    // ================= COURSE LINKS =================
                    foreach (var course in response.Courses)
                    {
                        course.AddLink("self", _linkGenerator.GetPathByAction(
                            "GetById", "Course", new { version = "4", id = course.Id })!, "GET");

                        course.AddLink("update", _linkGenerator.GetPathByAction(
                            "Update", "Course", new { version = "4", id = course.Id })!, "PUT");

                        course.AddLink("delete", _linkGenerator.GetPathByAction(
                            "Delete", "Course", new { version = "4", id = course.Id })!, "DELETE");
                    }

                    // ================= EDUCATION LINKS =================
                    foreach (var edu in response.Educations)
                    {
                        edu.AddLink("self", _linkGenerator.GetPathByAction(
                            "GetById", "Education", new { version = "3", id = edu.Id })!, "GET");

                        edu.AddLink("update", _linkGenerator.GetPathByAction(
                            "Update", "Education", new { version = "3", id = edu.Id })!, "PUT");

                        edu.AddLink("delete", _linkGenerator.GetPathByAction(
                            "Delete", "Education", new { version = "3", id = edu.Id })!, "DELETE");
                    }

                    // ================= SKILL LINKS =================
                    foreach (var skill in response.Skills)
                    {
                        skill.AddLink("self", _linkGenerator.GetPathByAction(
                            "GetById", "Skill", new { version = "2", id = skill.Id })!, "GET");

                        skill.AddLink("update", _linkGenerator.GetPathByAction(
                            "Update", "Skill", new { version = "2", id = skill.Id })!, "PUT");

                        skill.AddLink("delete", _linkGenerator.GetPathByAction(
                            "Delete", "Skill", new { version = "2", id = skill.Id })!, "DELETE");
                    }

                    return response;
                })
                .ToList();

            var pagedResponse = new PagedResponse<UserResponse>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };

            pagedResponse.AddLink("self", _linkGenerator.GetPathByAction(
                "GetAll", "User", new { version, pageNumber, pageSize })!, "GET");

            pagedResponse.AddLink("next", pageNumber < totalPages
                ? _linkGenerator.GetPathByAction("GetAll", "User",
                    new { version, pageNumber = pageNumber + 1, pageSize })!
                : null, "GET");

            pagedResponse.AddLink("prev", pageNumber > 1
                ? _linkGenerator.GetPathByAction("GetAll", "User",
                    new { version, pageNumber = pageNumber - 1, pageSize })!
                : null, "GET");

            return Ok(pagedResponse);
        }

        /// <summary>
        /// Returns a specific user by ID.
        /// </summary>
        /// <param name="id">User ID.</param>
        /// <returns>The user or 404 if not found.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserResponse), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<UserResponse>> GetById(int id)
        {
            try
            {
                var user = await _userService.GetByIdAsync(id);
                var version = HttpContext.GetRequestedApiVersion()?.ToString() ?? "1.0";

                var response = user.ToResponse();
                response.AddLink("self", _linkGenerator.GetPathByAction("GetById", "User", new { version, id })!, "GET");
                response.AddLink("update", _linkGenerator.GetPathByAction("Update", "User", new { version, id })!, "PUT");
                response.AddLink("delete", _linkGenerator.GetPathByAction("Delete", "User", new { version, id })!, "DELETE");

                return Ok(response);
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="request">The user creation payload.</param>
        /// <returns>The created user with HATEOAS links.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(UserResponse), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<UserResponse>> Create(CreateUserRequest request)
        {
            var user = await _userService.CreateAsync(request);
            var version = HttpContext.GetRequestedApiVersion()?.ToString() ?? "1.0";

            var response = user.ToResponse();
            response.AddLink("self", _linkGenerator.GetPathByAction("GetById", "User", new { version, id = user.Id })!, "GET");
            response.AddLink("update", _linkGenerator.GetPathByAction("Update", "User", new { version, id = user.Id })!, "PUT");
            response.AddLink("delete", _linkGenerator.GetPathByAction("Delete", "User", new { version, id = user.Id })!, "DELETE");

            return CreatedAtAction(nameof(GetById), new { id = user.Id }, response);
        }

        /// <summary>
        /// Updates an existing user.
        /// </summary>
        /// <param name="id">User ID.</param>
        /// <param name="request">Payload with updated fields.</param>
        /// <returns>204 if updated, 404 if not found.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, UpdateUserRequest request)
        {
            try
            {
                await _userService.UpdateAsync(id, request);
                return NoContent();
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Deletes a user.
        /// </summary>
        /// <param name="id">User ID.</param>
        /// <returns>204 if deleted, 404 if not found.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _userService.DeleteAsync(id);
                return NoContent();
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
