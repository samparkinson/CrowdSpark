using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CrowdSpark.Web.Controllers
{

    public class NameO {
        public string name { get; set; }
    }

    [Produces("application/json")]
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
        [Route("skills")]
        [HttpGet]
        public IEnumerable<string> GetSkills()
        {
            return new string[] { "programming", "pyrotechnics" };
        }

        //[HttpPost("skills{name}")]
        [Route("skills")]
        [HttpPost]
        public string PostSkill([FromBody]NameDTO name)
        {
//            return "ok, skill already present!";
            return "skill "+ name.name + " added!";
        }

        /*   [Route("api/users/[controller]")]
           public class SkillsController
           {

           }*/
    }

}
