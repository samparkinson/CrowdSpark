using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CrowdSpark.Web.Controllers
{
    [Route("api/[controller]")]
    public class UsersController
    {
        // GET api/users/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "specific user";
        }

        // GET api/users/skills
        [HttpGet("skills")]
        public IEnumerable<string> GetSkills()
        {
            return new string[] { "programming", "pyrotechnics" };
        }

        [HttpPost("skills/{name}")]
        public string GetSkill([FromBody]string name)
        {
//            return "ok, skill already present!";
            return "skill "+ name + " added!";
        }

        /*   [Route("api/users/[controller]")]
           public class SkillsController
           {

           }*/
    }

}
