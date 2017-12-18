﻿using System;
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
    [Route("api/v1/locations")]
    public class LocationController : Controller
    {
        private readonly ILocationLogic _locationLogic;

        public LocationController(ILocationLogic locationLogic)
        {
            _locationLogic = locationLogic;
        }

        // GET api/v1/locations
        // GET api/v1/locations?search=searchString
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery(Name = "city")] string citySearchString, [FromQuery(Name = "country")] string countrySearchString)
        {
            if (string.IsNullOrEmpty(citySearchString) && string.IsNullOrEmpty(countrySearchString))
            {
                return Ok(await _locationLogic.GetAsync());
            }

            var skills = await _locationLogic.FindAsync(citySearchString, countrySearchString);

            if (skills.Count() == 0)
            {
                return NoContent();
            }
            else return Ok(skills);
        }

        // GET api/v1/locations/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var skill = await _locationLogic.GetAsync(id);

            if (skill is null)
            {
                return NotFound();
            }
            return Ok(skill);
        }

        // POST api/v1/locations
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]LocationCreateDTO location)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _locationLogic.CreateAsync(location);

            if (success == ResponseLogic.SUCCESS)
            {
                var createdLocation = await _locationLogic.FindExactAsync(location.City, location.Country);
                return CreatedAtAction(nameof(Get), new { createdLocation.Id }, createdLocation.Id);
            }
            else return StatusCode(500);
        }

        // PUT api/v1/locations/
        [HttpPut]
        public async Task<IActionResult> Put([FromBody]LocationDTO location)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _locationLogic.UpdateAsync(location);

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

        // DELETE api/v1/locations/
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int locationId)
        {
            var success = await _locationLogic.DeleteAsync(locationId);

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