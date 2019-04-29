using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LocationCapture.BL;
using LocationCapture.Models;
using LocationCapture.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LocationCapture.WebAPI.Controllers
{
    [Route("api/locations")]
    public class LocationsController : Controller
    {
        private readonly ILocationDataService _locationDataService;
        private readonly ILogger<LocationsController> _logger;

        public LocationsController(ILocationDataService locationDataService,
            ILogger<LocationsController> logger)
        {
            _locationDataService = locationDataService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLocations()
        {
            var locations = await _locationDataService.GetAllLocationsAsync();
            var result = Mapper.Map<IEnumerable<Location>, IEnumerable<LocationDto>>(locations);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddLocation([FromBody]LocationForCreationDto locationForCreation)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Bad request. Model state invalid: {@ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            var locationToAdd = Mapper.Map<LocationForCreationDto, Location>(locationForCreation);
            var addedLocation = await _locationDataService.AddLocationAsync(locationToAdd);
            var result = Mapper.Map<Location, LocationDto>(addedLocation);

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLocation(int id, [FromBody]LocationForUpdateDto locationForUpdate)
        {
            if (id != locationForUpdate.Id)
            {
                _logger.LogWarning("Bad request. Location ID mismatch: {0} != {Id}", id, locationForUpdate.Id);
                return BadRequest();
            }

            if(!ModelState.IsValid)
            {
                _logger.LogWarning("Bad request. Model state invalid: {@ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            var changeTarget = (await _locationDataService.GetAllLocationsAsync())
                .FirstOrDefault(_ => _.Id == id);
            if (changeTarget == null)
            {
                _logger.LogWarning("Location not found. Location ID = {0}", id);
                return NotFound();
            }

            var locationToUpdate = Mapper.Map<LocationForUpdateDto, Location>(locationForUpdate);
            var updatedLocation = await _locationDataService.RenameLocationAsync(id, locationForUpdate.Name);
            var result = Mapper.Map<Location, LocationDto>(updatedLocation);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLocation(int id)
        {
            var changeTarget = (await _locationDataService.GetAllLocationsAsync())
                .FirstOrDefault(_ => _.Id == id);
            if (changeTarget == null)
            {
                _logger.LogWarning("Location not found. Location ID = {0}", id);
                return NotFound();
            }

            var deletedLocation = await _locationDataService.RemoveLocationAsync(id);
            var result = Mapper.Map<Location, LocationDto>(deletedLocation);

            return Ok(result);
        }
    }
}
