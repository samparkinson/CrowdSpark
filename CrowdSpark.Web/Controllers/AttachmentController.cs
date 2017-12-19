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
    [Route("api/v1/attachments")]
    public class AttachmentController : Controller
    {
        private readonly IAttachmentLogic _attachmentLogic;

        public AttachmentController(IAttachmentLogic attachmentLogic)
        {
            _attachmentLogic = attachmentLogic;
        }

        // GET api/v1/attachments?project=42
        [HttpGet]
        public async Task<IActionResult> GetForProject([FromQuery(Name = "project")] int projectId)
        {
            return Ok(await _attachmentLogic.GetForProjectAsync(projectId));
        }

        // GET api/v1/attachments/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var attachment = await _attachmentLogic.GetAsync(id);

            if (attachment is null)
            {
                return NotFound();
            }
            return Ok(attachment);
        }

        // POST api/v1/attachments
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]AttachmentCreateDTO attachment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _attachmentLogic.CreateAsync(attachment);

            if (success.Outcome == ResponseLogic.SUCCESS)
            {
                return CreatedAtAction(nameof(Get), new { success.Id }, success.Id);
            }
            else return StatusCode(500);
        }

        // PUT api/v1/attachments/
        [HttpPut]
        public async Task<IActionResult> Put([FromBody]AttachmentDTO attachment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _attachmentLogic.UpdateAsync(attachment);

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

        // DELETE api/v1/attachments/
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int attachmentId)
        {
            var success = await _attachmentLogic.DeleteAsync(attachmentId);

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
