using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CrowdSpark.Common;

namespace CrowdSpark.Controllers
{
    [Produces("application/json")]
    [Route("api/projects/[controller]")]
    public class ProjectsController : Controller
    {
        private readonly IProjectRepository _repository;

        public ProjectsController(IProjectRepository repository )
        {
            _repository = repository;
        }

        // GET api/projects
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var projects = await _repository.ReadAsync();

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
            var project = await _repository.FindAsync(id);

            if (project is null)
            {
                return NotFound();
            }
            else return Ok(project);
        }

        // POST api/projects
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]ProjectDTO project)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var projectId = await _repository.CreateAsync(project);

            return CreatedAtAction(nameof(Get), new {projectId}, null);
        }

        // PUT api/projects/42
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody]ProjectDetailsDTO project)
        {
            if (id != project.Id)
            {
                ModelState.AddModelError(string.Empty, "project.id id must match route id");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _repository.UpdateAsync(project);

            if (success)
            {
                return NoContent();
            }
            else return NotFound();
        }

        // DELETE api/projects/42
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _repository.DeleteAsync(id);

            if (success)
            {
                return NoContent();
            }
            else return NotFound();
        }
    }
}
