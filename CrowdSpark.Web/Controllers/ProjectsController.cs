using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CrowdSpark.Controllers
{
    [Route("api/[controller]")]
    public class ProjectsController : Controller
    {
        // GET api/projects/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "specific project";
        }

        // POST api/values
        [HttpPost("{title},{description}")]
        public void Post([FromBody]string title)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
