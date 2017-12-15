using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using CrowdSpark.Common;
using CrowdSpark.Logic;
using CrowdSpark.Entitites;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace CrowdSpark.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/users")]
    public class UsersController : Controller
    {
        class UserIdentity : IdentityUser
        {

        }

        private readonly IUserLogic _userLogic;

        public UsersController(IUserLogic userLogic/*, UserManager<UserIdentity> userman */)
        {
         //   _userManager = userman;
            _userLogic = userLogic;
        }

        // GET api/v1/users
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _userLogic.GetAsync()); // users can never be null as calling user must be logged in, thus in the db
        }

        // GET api/v1/users/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> Get(int id)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            string st = userId;
            Console.WriteLine(st);
            return Ok(st);
           // return Ok(await _userLogic.GetAsync(id)); //TODO, decided if users can look at the profile of other users
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetToken()
        {
            return Ok(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
        }


        // POST api/v1/users
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]UserDTO user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var azureUId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var success = await _userLogic.CreateAsync(user, azureUId);

            var userId = 0; //TODO, get userID from auth

            if (success == ResponseLogic.SUCCESS)
            {
                return CreatedAtAction(nameof(Get), new { userId }, null);
            }
            else return StatusCode(500);
        }

        // PUT api/v1/users/
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Put([FromBody]UserDTO user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = 0; // TODO, get user ID from auth

            var success = await _userLogic.UpdateAsync(userId, user);

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

        // DELETE api/v1/users/
        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> Delete()
        {
            var userId = 0; //TODO, get userId from auth

            var success = await _userLogic.DeleteAsync(userId);

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

        // GET api/v1/users/skills
        [Route("skills")]
        [HttpGet]
        public async Task<IActionResult> GetSkills()
        {
            var userId = 0; //TODO, get user id from auth

            var user = await _userLogic.GetAsync(userId);

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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = 0; //TODO, get userId from auth

            var success = await _userLogic.AddSkillAsync(userId, skill);

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
    }
}
