using AutoMapper;
using LocationCapture.BL;
using LocationCapture.Enums;
using LocationCapture.Models;
using LocationCapture.WebAPI.Controllers;
using LocationCapture.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace LocationCapture.WebAPI.UnitTests
{
    public class SnapshotGroupsControllerUnitTests
    {
        private void SetUp()
        {
            Mapper.Reset();
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<SnapshotGroup, SnapshotGroupDto>();
            });
        }

        [Fact]
        public async void GroupSnapshots_ShouldReturnOk()
        {
            WebApiTestsHelper.Lock();

            // Arrange
            SetUp();
            var snapshotGroups = new List<SnapshotGroup>
            {
                new SnapshotGroup
                {
                    Name = "December 2018",
                    SnapshotIds = new List<int>{1,2,3}
                },
                new SnapshotGroup
                {
                    Name = "January 2019",
                    SnapshotIds = new List<int>{4,5,6}
                }
            };
            Task<IEnumerable<SnapshotGroup>> dataFetchOperation()
            {
                var tcs = new TaskCompletionSource<IEnumerable<SnapshotGroup>>();
                tcs.SetResult(snapshotGroups);
                return tcs.Task;
            };
            var snapshotDataService = Substitute.For<ILocationSnapshotDataService>();
            snapshotDataService.ChooseGroupByOperation(Arg.Any<GroupByCriteria>())
                .Returns(dataFetchOperation);

            // Act
            var sit = new SnapshotGroupsController(snapshotDataService, Substitute.For<ILogger<SnapshotGroupsController>>());
            var result = await sit.GroupSnapshots(GroupByCriteria.CreatedDate);

            // Assert
            var dtos = WebApiTestsHelper.ExtractGenericCollectionFromActionResult<OkObjectResult, IEnumerable<SnapshotGroupDto>>(result)
                .ToList();
            for (var i = 0; i < dtos.Count; i++)
            {
                Assert.Equal(snapshotGroups[i].Name, dtos[i].Name);
                Assert.Equal(snapshotGroups[i].SnapshotIds, dtos[i].SnapshotIds);
            }

            WebApiTestsHelper.Unlock();
        }

        [Fact]
        public async void GroupSnapshots_ShouldReturnBadRequest()
        {
            // Act
            var sit = new SnapshotGroupsController(null, Substitute.For<ILogger<SnapshotGroupsController>>());
            var result = await sit.GroupSnapshots(GroupByCriteria.None);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }
    }
}
