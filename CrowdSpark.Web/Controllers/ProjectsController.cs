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
    [Produces("application/json")]
    [Route("api/v1/projects")]
    public class ProjectsController : Controller
    {
        private readonly IProjectLogic _logic;
        private readonly IUserLogic _userLogic;
        public Func<string> GetUserId; // Replaceable in testing

        public ProjectsController(IProjectLogic logic, IUserLogic userLogic )
        {
            _logic = logic;
            _userLogic = userLogic;
            GetUserId = () => this.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        // GET api/v1/projects
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var projects = await _logic.GetAsync();

            if (projects is null)
            {
                return Ok(new ProjectDTO[] { });
            }
            else return Ok(projects);
        }

        // GET api/v1/projects/42
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var project = await _logic.GetAsync(id);

            if (project is null)
            {
                return NotFound();
            }
            else return Ok(project);
        }

        
        [HttpGet("{searchString}", Name = "search")]
        public async Task<IActionResult> GetFromSearch([FromQuery] string searchString)
        {
            var projects = await _logic.SearchAsync(searchString);

            if (projects is null)
            {
                return NotFound();
            }
            else return Ok(projects);
        }

        [HttpGet("{categoryId}", Name = "category")]
        public async Task<IActionResult> GetFromCategory([FromQuery] int categoryId)
        {
            var projects = await _logic.SearchAsync(categoryId);

            if (projects is null)
            {
                return NotFound();
            }
            else return Ok(projects);
        }

        // POST api/v1/projects
        [HttpPost]
        [Authorize]
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
                return CreatedAtAction(nameof(Get), new { success.Id }, null);
            }
            else return StatusCode(500);
        }

        // PUT api/v1/projects/42
        [HttpPut]
        [Authorize]
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
        [Authorize]
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

        [Authorize]
        [Route("skills")]
        [HttpPost("{projectId}")]
        public async Task<IActionResult> AddSkill(int projectId, [FromBody] SkillCreateDTO skill)
        {
            // if skill.Name does not exist in Skill DB, create new SkillDTO with new Id;
            throw new NotImplementedException();
        }

        [Route("skills")]
        [HttpGet("{projectId}")]
        public async Task<IActionResult> GetSkills(int projectId)
        {
            throw new NotImplementedException();
            //return Ok(new SkillDTO());
        }

        [Authorize]
        [Route("spark")]
        [HttpGet("{projectId}")]
        public async Task<IActionResult> GetApprovedSparks(int projectId)
        {
            throw new NotImplementedException();
        }

        [Authorize]
        [Route("spark")]
        [HttpPost("{projectId}")]
        public async Task<IActionResult> CreateSpark(int projectId)
        {
            throw new NotImplementedException();
        }
    }
}
