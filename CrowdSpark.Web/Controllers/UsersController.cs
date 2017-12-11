﻿using System;
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

namespace CrowdSpark.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/users/[controller]")]
    public class UsersController : Controller
    {
        private readonly IUserLogic _userLogic;

        public UsersController(IUserLogic userLogic)
        {
            _userLogic = userLogic;
        }

        // GET api/users
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _userLogic.GetAsync()); // users can never be null as calling user must be logged in, thus in the db
        }

        // GET api/users/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            //var userId = HttpContext.User.Identity.GetUserId();

            return Ok(await _userLogic.GetAsync(id)); //TODO, decided if users can look at the profile of other users
        }

        // POST api/users
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]UserDTO user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _userLogic.CreateAsync(user);

            var userId = 0; //TODO, get userID from auth

            if (success == ResponseLogic.SUCCESS)
            {
                return CreatedAtAction(nameof(Get), new { userId }, null);
            }
            else return StatusCode(500);
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

        // DELETE api/users/
        [HttpDelete]
        public async Task<IActionResult> Delete()
        {
            var userId = 0; //TODO, get userId from auth

            var success = await _userLogic.DeleteAsync(userId);

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

        // GET api/users/skills
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
        public async Task<IActionResult> PostSkill([FromBody]Skill skill)
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
