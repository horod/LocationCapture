using LocationCapture.DAL;
using LocationCapture.Models;
using NSubstitute;
using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;

namespace LocationCapture.BL.UnitTests
{
    public class LocationDataServiceUnitTests
    {
        private ILocationContextFactory _factory;
        private ILocationContext _context;

        [Fact]
        public async void GetAllLocations_ShouldReadFromDbContext()
        {
            // Arrange
            SetUp();
            var locationsInDb = new List<Location>
            {
                new Location{Id = 1},
                new Location{Id = 2},
                new Location{Id = 3}
            };
            _context.Locations.Returns(locationsInDb);

            // Act
            var sit = new LocationDataService(_factory);
            var results = (await sit.GetAllLocationsAsync()).ToList();

            // Assert
            Assert.Equal(locationsInDb[0].Id, results[0].Id);
            Assert.Equal(locationsInDb[1].Id, results[1].Id);
            Assert.Equal(locationsInDb[2].Id, results[2].Id);
        }

        [Fact]
        public async void AddLocation_ShouldAddToDbContext()
        {
            // Arrange
            SetUp();
            var toAdd = new Location { Id = 2 };
            var payload = new Location
            {
                Id = 2,
                Name = "NewLocName"
            };

            // Act
            var sit = new LocationDataService(_factory);
            await sit.AddLocationAsync(toAdd);

            // Assert
            _context.Received().Add(toAdd);
        }

        [Fact]
        public async void RenameLocation_ShouldSaveDbContextChanges()
        {
            // Arrange
            SetUp();
            var payload = new Location
            {
                Id = 2,
                Name = "NewLocName"
            };
            var locationInDb = new Location { Id = 2 };
            _context.Locations.Returns(new List<Location> { locationInDb });

            // Act
            var sit = new LocationDataService(_factory);
            var result = await sit.RenameLocationAsync(payload.Id, payload.Name);

            // Assert
            _context.Received().SaveChanges();
            Assert.Equal(payload.Name, result.Name);
        }

        [Fact]
        public async void RemoveLocation_ShouldRemoveFromDbContext()
        {
            // Arrange
            SetUp();
            var locationInDb = new Location { Id = 2 };
            _context.Locations.Returns(new List<Location> { locationInDb });

            // Act
            var sit = new LocationDataService(_factory);
            await sit.RemoveLocationAsync(2);

            // Assert
            _context.Received().Remove(locationInDb);
        }

        private void SetUp()
        {
            _factory = Substitute.For<ILocationContextFactory>();
            _context = Substitute.For<ILocationContext>();
            _factory.Create().Returns(_context);
        }
    }
}
