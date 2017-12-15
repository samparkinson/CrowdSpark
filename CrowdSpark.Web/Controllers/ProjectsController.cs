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

namespace CrowdSpark.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/projects")]
    public class ProjectsController : Controller
    {
        private readonly IProjectLogic _logic;

        public ProjectsController(IProjectLogic logic )
        {
            _logic = logic;
        }

        // GET api/projects
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

        // GET api/projects/42
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

        // POST api/projects
        [HttpPost]
        [Authorize]
        //     public async Task<IActionResult> Post([FromBody]CreateProjectDTO project)
        public async Task<IActionResult> Post([FromBody]CreateProjectDTO project)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var projectId = await _logic.CreateAsync(project);

            return CreatedAtAction(nameof(Get), new {projectId}, null);
        }

        // PUT api/projects/42
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody]ProjectSummaryDTO project)
        {
            if (id != project.Id)
            {
                ModelState.AddModelError(string.Empty, "project.id id must match route id");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _logic.UpdateAsync(project);

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

        // DELETE api/projects/42
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // TODO check that the signed in user has permission
            var success = await _logic.DeleteAsync(id);

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
        [HttpPost("{projectId}")]
        public async Task<IActionResult> CreateSpark(int projectId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            throw new NotImplementedException();
        }
    }
}
