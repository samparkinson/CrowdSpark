using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using CrowdSpark.Common;
using CrowdSpark.Logic;
using CrowdSpark.Entitites;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace CrowdSpark.Web.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/v1/skills")]
    public class SkillController : Controller
    {
        private readonly ISkillLogic _skillLogic;

        public SkillController(ISkillLogic skillLogic)
        {
            _skillLogic = skillLogic;
        }

        // GET api/v1/skills
        // GET api/v1/skills?search=searchString
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery(Name = "search")] string searchString)
        {
            if (string.IsNullOrEmpty(searchString))
            {
                return Ok(await _skillLogic.GetAsync());
            }

            var skills = await _skillLogic.SearchAsync(searchString);

            if (skills.Count() == 0)
            {
                return NoContent();
            }
            else return Ok(skills);
        }

        // GET api/v1/skills/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var skill = await _skillLogic.GetAsync(id);

            if (skill is null)
            {
                return NotFound();
            }
            return Ok(skill);
        }

        // POST api/v1/skills
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]SkillCreateDTO skill)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _skillLogic.CreateAsync(skill);

            if (success == ResponseLogic.SUCCESS)
            {
                var createdSkill = await _skillLogic.FindExactAsync(skill.Name);
                return CreatedAtAction(nameof(Get), new { createdSkill.Id }, null);
            }
            else return StatusCode(500);
        }

        // PUT api/v1/skills/
        [HttpPut]
        public async Task<IActionResult> Put([FromBody]SkillDTO skill)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _skillLogic.UpdateAsync(skill);

            if (success == ResponseLogic.SUCCESS)
            {
                return NoContent();
            }
            else if (success == ResponseLogic.NOT_FOUND)
            {
                return NotFound();
            }
            else return StatusCode(500);
        }

        // DELETE api/v1/skills/
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int skillId)
        {
            var success = await _skillLogic.DeleteAsync(skillId);

            if (success == ResponseLogic.SUCCESS) //NOTE, this is probably condusing as success is returned if nothing is deleted (I.e. the skill is being used somewhere).... need to consider if we even want to expose this api
            {
                return NoContent();
            }
            else if (success == ResponseLogic.NOT_FOUND)
            {
                return NotFound();
            }
            else return StatusCode(500);
        }
    }

}
