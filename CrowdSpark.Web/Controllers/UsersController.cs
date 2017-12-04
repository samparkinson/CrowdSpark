using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using CrowdSpark.Common;

namespace CrowdSpark.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/users/[controller]")]
    public class UsersController : Controller
    {
        private readonly IUserRepository _repository;

        public UsersController(IUserRepository repository)
        {
            _repository = repository;
        }

        // GET api/users
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var users = await _repository.ReadAsync();

            if (users is null) //NOTE can user actually be null?
            {
                return Ok(new UserDTO[] { });
            }
            else return Ok(users);
        }

        // GET api/users/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var user = await _repository.FindAsync(id);

            if (user is null) //NOTE can user actually be null?
            {
                return NotFound();
            }
            else return Ok(user);
        }

        // PUT api/users/
        [HttpPut]
        public async Task<IActionResult> Put([FromBody]UserDTO user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = 0; // TODO, get user ID from auth

            var success = await _repository.UpdateAsync(userId, user);

            if (success)
            {
                return NoContent();
            }
            else return NotFound();
        }

        // DELETE api/users/
        [HttpDelete]
        public async Task<IActionResult> Delete()
        {
            var userId = 0; //TODO, get userId from auth

            var success = await _repository.DeleteAsync(userId);

            if (success)
            {
                return NoContent();
            }
            else return NotFound();
        }

        // GET api/users/skills
        [Route("skills")]
        [HttpGet]
        public async Task<IActionResult> GetSkills()
        {
            var userId = 0; //TODO, get user id from auth

            var user = await _repository.FindAsync(userId);

            if (user is null) //NOTE can user actually be null?
            {
                return NotFound();
            }
            return Ok(user.Skills);
        }

        //[HttpPost("skills{name}")]
        [Route("skills")] //NOTE,Is this to create a skill for the user or to create a new skill in our skill db??
        [HttpPost]
        public async Task<IActionResult> PostSkill([FromBody]SkillDTO skill)
        {
            throw new NotImplementedException();

            //NOTE, I think we should create a new project which contains "logic", e.g. adding skills to users and projects, this should also check that the skill is in the skill db etc, if removing skills this is where the check to see if they live in other places as well, this can be called in a non blocking way. I.e. code adds skill to user, says done, but then does the maintainance of the skill task seperatley

            if (skill.PId != null)
            {
                ModelState.AddModelError(string.Empty, "PId must be null when creating a skill for a user.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = 0; //TODO, get userId from auth

            var success = await _repository.AddSkillAsync(userId, skill);

        }
    }

}
