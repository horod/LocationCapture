using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LocationCapture.BL;
using LocationCapture.Models;
using LocationCapture.WebAPI.Helpers;
using LocationCapture.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LocationCapture.WebAPI.Controllers
{
    [Route("api/snapshots")]
    public class LocationSnapshotsController : Controller
    {
        private readonly ILocationSnapshotDataService _locationSnapshotDataService;
        private readonly ILogger<LocationSnapshotsController> _logger;

        public LocationSnapshotsController(ILocationSnapshotDataService locationSnapshotDataService,
            ILogger<LocationSnapshotsController> logger)
        {
            _locationSnapshotDataService = locationSnapshotDataService;
            _logger = logger;
        }

        [HttpGet("by-location-id")]
        public async Task<IActionResult> GetSnapshotsByLocationId(int locationId)
        {
            var snapshots = await _locationSnapshotDataService.GetSnapshotsByLocationIdAsync(locationId);
            var result = Mapper.Map<IEnumerable<LocationSnapshot>, IEnumerable<LocationSnapshotDto>>(snapshots);

            return Ok(result);
        }

        [HttpGet("by-ids")]
        public async Task<IActionResult> GetSnapshotsByIds(
            [ModelBinder(BinderType = typeof(IntArrayModelBinder), Name = "snapshotsIds")]IEnumerable<int> snapshotsIds)
        {
            if (snapshotsIds == null)
            {
                _logger.LogWarning("Bad request. Unable to deserialize snapshots IDs.");
                return BadRequest();
            }

            var snapshots = await _locationSnapshotDataService.GetSnapshotsByIdsAsync(snapshotsIds);
            var result = Mapper.Map<IEnumerable<LocationSnapshot>, IEnumerable<LocationSnapshotDto>>(snapshots);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddSnapshot([FromBody]LocationSnapshotForCreationDto snapshotForCreation)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Bad request. Model state invalid: {@ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            var snapshotToAdd = Mapper.Map<LocationSnapshotForCreationDto, LocationSnapshot>(snapshotForCreation);
            var addedSnapshot = await _locationSnapshotDataService.AddSnapshotAsync(snapshotToAdd);
            var result = Mapper.Map<LocationSnapshot, LocationSnapshotDto>(addedSnapshot);

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSnapshot(int id, [FromBody]LocationSnapshotForUpdateDto snapshotForUpdate)
        {
            if (id != snapshotForUpdate.Id)
            {
                _logger.LogWarning("Bad request. Location snapshot ID mismatch: {0} != {Id}", id, snapshotForUpdate.Id);
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Bad request. Model state invalid: {@ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            var changeTargets = await _locationSnapshotDataService.GetSnapshotsByIdsAsync(new[] { id });
            if(changeTargets.Count() != 1)
            {
                _logger.LogWarning("Location snapshot not found. Location snapshot ID = {0}", id);
                return NotFound();
            }

            var snapshotToUpdate = Mapper.Map<LocationSnapshotForUpdateDto, LocationSnapshot>(snapshotForUpdate);
            var updatedSnapshot = await _locationSnapshotDataService.UpdateSnapshotAsync(snapshotToUpdate);
            var result = Mapper.Map<LocationSnapshot, LocationSnapshotDto>(updatedSnapshot);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSnapshot(int id)
        {
            var changeTargets = await _locationSnapshotDataService.GetSnapshotsByIdsAsync(new[] { id });
            if (changeTargets.Count() != 1)
            {
                _logger.LogWarning("Location snapshot not found. Location snapshot ID = {0}", id);
                return NotFound();
            }

            var deletedSnapshot = await _locationSnapshotDataService.RemoveSnapshotAsync(id);
            var result = Mapper.Map<LocationSnapshot, LocationSnapshotDto>(deletedSnapshot);

            return Ok(result);
        }
    }
}