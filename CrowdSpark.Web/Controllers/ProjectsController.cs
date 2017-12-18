using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CrowdSpark.Web.Models;
using Microsoft.AspNetCore.Http;
using CrowdSpark.Common;
using CrowdSpark.Logic;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CrowdSpark.Web.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/v1/projects")]
    public class ProjectsController : Controller
    {
        private readonly IProjectLogic _logic;
        private readonly IUserLogic _userLogic;
        private readonly ISparkLogic _sparkLogic;
        public Func<string> GetUserId; // Replaceable in testing

        public ProjectsController(IProjectLogic logic, IUserLogic userLogic, ISparkLogic sparkLogic)
        {
            _logic = logic;
            _userLogic = userLogic;
            _sparkLogic = sparkLogic;
            GetUserId = () => this.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        // GET api/v1/projects
        // GET api/v1/projects?search=searchString
        // GET api/v1/projects?category=42
        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery(Name = "search")] string searchString,
            [FromQuery(Name = "category")] int categoryId)
        {
            IEnumerable<ProjectSummaryDTO> projects;
            bool queryParameterUsed = true;

            if (!string.IsNullOrEmpty(searchString))
            {
                projects = await _logic.SearchAsync(searchString);
            }
            else if (categoryId != 0)
            {
                projects = await _logic.SearchAsync(categoryId);
            }
            else
            {
                projects = await _logic.GetAsync();
                queryParameterUsed = false;
            }

            if (projects is null)
            {
                return queryParameterUsed ? (IActionResult) NoContent() : Ok(new ProjectDTO[] { });
            }
            else return Ok(projects);            
        }

        // GET api/v1/projects/42
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var project = await _logic.GetDetailedAsync(id);

            if (project is null)
            {
                return NotFound();
            }
            else return Ok(project);
        }      

        // POST api/v1/projects
        [HttpPost]
        //     public async Task<IActionResult> Post([FromBody]CreateProjectDTO project)
        public async Task<IActionResult> Post([FromBody]CreateProjectDTO project)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = await _userLogic.GetIdFromAzureUIdAsync(GetUserId());

            var success = await _logic.CreateAsync(project, userId);

            if (success.outcome == ResponseLogic.SUCCESS)
            {
                return CreatedAtAction(nameof(Get), new { success.Id }, success.Id);
            }
            else return StatusCode(500);
        }

        // PUT api/v1/projects/42
        [HttpPut("{projectId}")]
        public async Task<IActionResult> Put(int projectId, [FromBody]ProjectDTO project)
        {
            var userId = await _userLogic.GetIdFromAzureUIdAsync(GetUserId());

            if (projectId != project.Id)
            {
                ModelState.AddModelError("Id", "Requesting routeId must match projectId");
            }
            if (project.Creator is null || project.Creator.Id != userId)
            {
                ModelState.AddModelError("Creator", "Updating userId must match requesting userId");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _logic.UpdateAsync(project, userId);

            if (success == ResponseLogic.SUCCESS)
            {
                return Ok();
            }
            else if (success == ResponseLogic.NOT_FOUND)
            {
                return NotFound();
            }
            else return StatusCode(500);
        }

        // DELETE api/v1/projects/42
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = await _userLogic.GetIdFromAzureUIdAsync(GetUserId());

            var success = await _logic.DeleteAsync(id, userId);

            if (success == ResponseLogic.SUCCESS)
            {
                return Ok();
            }
            else if (success == ResponseLogic.NOT_FOUND)
            {
                return NotFound();
            }
            else if (success == ResponseLogic.UNAUTHORISED)
            {
                return Unauthorized();
            }
            else return StatusCode(500);
        }

        [Route("skills")]
        [HttpPost("{projectId}")]
        public async Task<IActionResult> AddSkill(int projectId, [FromBody] SkillDTO skill)
        {
            var userId = await _userLogic.GetIdFromAzureUIdAsync(GetUserId());

            var success = await _logic.AddSkillAsync(projectId, skill, userId);

            if (success == ResponseLogic.SUCCESS)
            {
                return Ok();
            }
            else if (success == ResponseLogic.NOT_FOUND)
            {
                return NotFound();
            }
            else if (success == ResponseLogic.UNAUTHORISED)
            {
                return Unauthorized();
            }
            else return StatusCode(500);
        }

        [Route("skills")]
        [HttpGet("{projectId}")]
        public async Task<IActionResult> GetSkills(int projectId)
        {
            var project = await _logic.GetDetailedAsync(projectId);

            if (project is null)
            {
                return NotFound();
            }
            else if (project.Skills.Count() == 0)
            {
                return NoContent();
            }
            else return Ok(project.Skills);
        }

        [Route("spark")]
        [HttpGet("{projectId}")]
        public async Task<IActionResult> GetApprovedSparks(int projectId)
        {
            var sparks = await _logic.GetApprovedSparksAsync(projectId);

            if (sparks.Count() == 0) return NoContent();
            else return Ok(sparks);
        }

        [Route("spark")]
        [HttpPost("{projectId}")]
        public async Task<IActionResult> CreateSpark(int projectId)
        {
            var userId = await _userLogic.GetIdFromAzureUIdAsync(GetUserId());

            var success = await _sparkLogic.CreateAsync(projectId, userId);

            if (success == ResponseLogic.SUCCESS)
            {
                return CreatedAtAction(nameof(GetApprovedSparks), new { projectId }, null);
            }
            else return StatusCode(500);
        }
    }
}
