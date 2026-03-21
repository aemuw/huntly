using Huntly.Application.DTOs;
using Huntly.Application.DTOs.JobApplication;
using Huntly.Application.Exceptions;
using Huntly.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Huntly.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class JobApplicationsController : ControllerBase
    {
        private readonly IJobApplicationService _jobService;

        public JobApplicationsController(IJobApplicationService jobService)
        {
            _jobService = jobService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<JobApplicationResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponse<JobApplicationResponse>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _jobService.GetPagedByUserIdAsync(GetUserId(), page, pageSize);
            return Ok(result);
        }

        private Guid GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(claim))
                throw new UnauthorizedException("Користувач не авторизований");
            return Guid.Parse(claim);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(JobApplicationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<JobApplicationResponse>> GetById(Guid id)
        {
            var result = await _jobService.GetByIdAsync(id, GetUserId());
            if (result is null)
                return NotFound();
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(JobApplicationResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<JobApplicationResponse>> Create([FromBody] CreateJobApplicationRequest request)
        {
            var result = await _jobService.CreateAsync(GetUserId(), request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateJobApplicationRequest request)
        {
            await _jobService.UpdateAsync(id, GetUserId(), request);
            return NoContent();
        }

        [HttpPut("{id}/status")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ChangeStatusRequest(Guid id, [FromBody] ChangeStatusRequest request)
        {
            await _jobService.ChangeStatusAsync(id, GetUserId(), request);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _jobService.DeleteAsync(id, GetUserId());
            return NoContent();
        }
    }
}
