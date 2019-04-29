using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using LocationCapture.BL;
using LocationCapture.Enums;
using LocationCapture.Models;
using LocationCapture.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LocationCapture.WebAPI.Controllers
{
    [Route("api/snapshot-groups")]
    public class SnapshotGroupsController : Controller
    {
        private readonly ILocationSnapshotDataService _locationSnapshotDataService;
        private readonly ILogger<SnapshotGroupsController> _logger;

        public SnapshotGroupsController(ILocationSnapshotDataService locationSnapshotDataService,
            ILogger<SnapshotGroupsController> logger)
        {
            _locationSnapshotDataService = locationSnapshotDataService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GroupSnapshots(GroupByCriteria groupBy)
        {
            if(groupBy == GroupByCriteria.None)
            {
                _logger.LogWarning("Bad request. Invalid group by criteria.");
                return BadRequest();
            }

            var dataFetchOperation = _locationSnapshotDataService.ChooseGroupByOperation(groupBy);
            var snapshotGroups = await dataFetchOperation();
            var result = Mapper.Map<IEnumerable<SnapshotGroup>, IEnumerable<SnapshotGroupDto>>(snapshotGroups);

            return Ok(result);
        }
    }
}
