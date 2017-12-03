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
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly IUserRepository _repository;

        public UsersController(IUserRepository repository)
        {
            _repository = repository;
        }

        // GET api/users/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "specific user";
        }

        // GET api/users/skills
        [Route("skills")]
        [HttpGet]
        public IEnumerable<string> GetSkills()
        {
            return new string[] { "programming", "pyrotechnics" };
        }

        //[HttpPost("skills{name}")]
        [Route("skills")]
        [HttpPost]
        public async Task<IActionResult> PostSkill([FromBody]SkillDTO skill)
        {
            if (skill.PId != null)
            {
                ModelState.AddModelError(string.Empty, "PId must be null when creating a skill for a user.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

        //    Skill skill_ = await repo_.createUserSkill();

            return Created("users/skills", 3);
        }

        /*   [Route("api/users/[controller]")]
           public class SkillsController
           {

           }*/
    }

}
