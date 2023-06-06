using LocationCapture.DAL;
using LocationCapture.Models;
using NSubstitute;
using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;

namespace LocationCapture.BL.UnitTests
{    
    public class LocationSnapshotDataServiceUnitTest
    {
        private ILocationContextFactory _factory;
        private ILocationContext _context;

        [Fact]
        public void GroupSnapshotsByCreatedDate_ShouldHandleFullRangeOfDates()
        {
            var currentDate = new DateTime(2017, 7, 2);
            var snapshots = new List<LocationSnapshot>
            {
                new LocationSnapshot{Id = 1, DateCreated = new DateTime(2017, 7, 1)},
                new LocationSnapshot{Id = 2, DateCreated = new DateTime(2017, 6, 2)},
                new LocationSnapshot{Id = 3, DateCreated = new DateTime(2017, 5, 3)},
                new LocationSnapshot{Id = 4, DateCreated = new DateTime(2017, 4, 4)},
                new LocationSnapshot{Id = 5, DateCreated = new DateTime(2017, 3, 5)},
                new LocationSnapshot{Id = 6, DateCreated = new DateTime(2017, 2, 6)},
                new LocationSnapshot{Id = 7, DateCreated = new DateTime(2017, 1, 7)},
                new LocationSnapshot{Id = 8, DateCreated = new DateTime(2016, 12, 8)},
                new LocationSnapshot{Id = 9, DateCreated = new DateTime(2016, 11, 9)},
                new LocationSnapshot{Id = 10, DateCreated = new DateTime(2016, 10, 31, 10, 59, 16)},
                new LocationSnapshot{Id = 11, DateCreated = new DateTime(2016, 9, 11)},
                new LocationSnapshot{Id = 12, DateCreated = new DateTime(2016, 8, 12)},
                new LocationSnapshot{Id = 13, DateCreated = new DateTime(2016, 7, 13)},
                new LocationSnapshot{Id = 14, DateCreated = new DateTime(2016, 6, 13)}
            };
            var expected = new List<SnapshotGroup>
            {
                new SnapshotGroup{Name = "July 2017", SnapshotIds = new List<int>{1}},
                new SnapshotGroup{Name = "June 2017", SnapshotIds = new List<int>{2}},
                new SnapshotGroup{Name = "May 2017", SnapshotIds = new List<int>{3}},
                new SnapshotGroup{Name = "April 2017", SnapshotIds = new List<int>{4}},
                new SnapshotGroup{Name = "March 2017", SnapshotIds = new List<int>{5}},
                new SnapshotGroup{Name = "February 2017", SnapshotIds = new List<int>{6}},
                new SnapshotGroup{Name = "January 2017", SnapshotIds = new List<int>{7}},
                new SnapshotGroup{Name = "December 2016", SnapshotIds = new List<int>{8}},
                new SnapshotGroup{Name = "November 2016", SnapshotIds = new List<int>{9}},
                new SnapshotGroup{Name = "October 2016", SnapshotIds = new List<int>{10}},
                new SnapshotGroup{Name = "September 2016", SnapshotIds = new List<int>{11}},
                new SnapshotGroup{Name = "August 2016", SnapshotIds = new List<int>{12}},
                new SnapshotGroup{Name = "July 2016", SnapshotIds = new List<int>{13}},
                new SnapshotGroup{Name = "June 2016", SnapshotIds = new List<int>{14}}
            };

            GroupSnapshotsTest(_ => _.GroupSnapshotsByCreatedDate(currentDate), snapshots, expected);
        }

        [Fact]
        public void GroupSnapshotsByCreatedDate_ShouldHandleIncompleteRangeOfDates()
        {
            var currentDate = new DateTime(2017, 7, 2);
            var snapshots = new List<LocationSnapshot>
            {
                new LocationSnapshot{Id = 1, DateCreated = new DateTime(2017, 7, 2)},
                new LocationSnapshot{Id = 2, DateCreated = new DateTime(2017, 7, 1)},
                new LocationSnapshot{Id = 3, DateCreated = new DateTime(2017, 3, 17)},
                new LocationSnapshot{Id = 4, DateCreated = new DateTime(2017, 3, 16)},
                new LocationSnapshot{Id = 5, DateCreated = new DateTime(2016, 11, 10)},
                new LocationSnapshot{Id = 6, DateCreated = new DateTime(2016, 11, 9)},
                new LocationSnapshot{Id = 7, DateCreated = new DateTime(2016, 5, 14)},
                new LocationSnapshot{Id = 8, DateCreated = new DateTime(2016, 3, 13)},
            };
            var expected = new List<SnapshotGroup>
            {
                new SnapshotGroup{Name = "July 2017", SnapshotIds = new List<int>{1,2}},
                new SnapshotGroup{Name = "March 2017", SnapshotIds = new List<int>{3,4}},
                new SnapshotGroup{Name = "November 2016", SnapshotIds = new List<int>{5,6}},
                new SnapshotGroup{Name = "May 2016", SnapshotIds = new List<int>{7}},
                new SnapshotGroup{Name = "March 2016", SnapshotIds = new List<int>{8}}
            };

            GroupSnapshotsTest(_ => _.GroupSnapshotsByCreatedDate(currentDate), snapshots, expected);
        }

        [Fact]
        public void GroupSnapshotsByCreatedDate_ShouldHandleAllSnapshotsFromTheSameMonth()
        {
            var currentDate = new DateTime(2017, 7, 2);
            var snapshots = new List<LocationSnapshot>
            {
                new LocationSnapshot{Id = 1, DateCreated = new DateTime(2017, 5, 2)},
                new LocationSnapshot{Id = 2, DateCreated = new DateTime(2017, 5, 1)},
                new LocationSnapshot{Id = 3, DateCreated = new DateTime(2017, 5, 17)},
                new LocationSnapshot{Id = 4, DateCreated = new DateTime(2017, 5, 16)},
            };
            var expected = new List<SnapshotGroup>
            {
                new SnapshotGroup{Name = "May 2017", SnapshotIds = new List<int>{1,2,3,4}}
            };

            GroupSnapshotsTest(_ => _.GroupSnapshotsByCreatedDate(currentDate), snapshots, expected);
        }

        [Fact]
        public void GroupSnapshotsByLongitude_ShouldHandleFullRangeOfLongitudes()
        {
            var snapshots = new List<LocationSnapshot>
            {
                new LocationSnapshot{Id = 1, Longitude = -178},
                new LocationSnapshot{Id = 2, Longitude = -168},
                new LocationSnapshot{Id = 3, Longitude = -158},
                new LocationSnapshot{Id = 4, Longitude = -148},
                new LocationSnapshot{Id = 5, Longitude = -138},
                new LocationSnapshot{Id = 6, Longitude = -128},
                new LocationSnapshot{Id = 7, Longitude = -118},
                new LocationSnapshot{Id = 8, Longitude = -108},
                new LocationSnapshot{Id = 9, Longitude = -98},
                new LocationSnapshot{Id = 10, Longitude = -88},
                new LocationSnapshot{Id = 11, Longitude = -78},
                new LocationSnapshot{Id = 12, Longitude = -68},
                new LocationSnapshot{Id = 13, Longitude = -58},
                new LocationSnapshot{Id = 14, Longitude = -48},
                new LocationSnapshot{Id = 15, Longitude = -38},
                new LocationSnapshot{Id = 16, Longitude = -28},
                new LocationSnapshot{Id = 17, Longitude = -18},
                new LocationSnapshot{Id = 18, Longitude = -8},
                new LocationSnapshot{Id = 19, Longitude = 8},
                new LocationSnapshot{Id = 20, Longitude = 18},
                new LocationSnapshot{Id = 21, Longitude = 28},
                new LocationSnapshot{Id = 22, Longitude = 38},
                new LocationSnapshot{Id = 23, Longitude = 48},
                new LocationSnapshot{Id = 24, Longitude = 58},
                new LocationSnapshot{Id = 25, Longitude = 68},
                new LocationSnapshot{Id = 26, Longitude = 78},
                new LocationSnapshot{Id = 27, Longitude = 88},
                new LocationSnapshot{Id = 28, Longitude = 98},
                new LocationSnapshot{Id = 29, Longitude = 108},
                new LocationSnapshot{Id = 30, Longitude = 118},
                new LocationSnapshot{Id = 31, Longitude = 128},
                new LocationSnapshot{Id = 32, Longitude = 138},
                new LocationSnapshot{Id = 33, Longitude = 148},
                new LocationSnapshot{Id = 34, Longitude = 158},
                new LocationSnapshot{Id = 35, Longitude = 168},
                new LocationSnapshot{Id = 36, Longitude = 178}
            };
            var expected = new List<SnapshotGroup>
            {
                new SnapshotGroup{Name = "180° W - 170° W", SnapshotIds = new List<int>{1}},
                new SnapshotGroup{Name = "170° W - 160° W", SnapshotIds = new List<int>{2}},
                new SnapshotGroup{Name = "160° W - 150° W", SnapshotIds = new List<int>{3}},
                new SnapshotGroup{Name = "150° W - 140° W", SnapshotIds = new List<int>{4}},
                new SnapshotGroup{Name = "140° W - 130° W", SnapshotIds = new List<int>{5}},
                new SnapshotGroup{Name = "130° W - 120° W", SnapshotIds = new List<int>{6}},
                new SnapshotGroup{Name = "120° W - 110° W", SnapshotIds = new List<int>{7}},
                new SnapshotGroup{Name = "110° W - 100° W", SnapshotIds = new List<int>{8}},
                new SnapshotGroup{Name = "100° W - 90° W", SnapshotIds = new List<int>{9}},
                new SnapshotGroup{Name = "90° W - 80° W", SnapshotIds = new List<int>{10}},
                new SnapshotGroup{Name = "80° W - 70° W", SnapshotIds = new List<int>{11}},
                new SnapshotGroup{Name = "70° W - 60° W", SnapshotIds = new List<int>{12}},
                new SnapshotGroup{Name = "60° W - 50° W", SnapshotIds = new List<int>{13}},
                new SnapshotGroup{Name = "50° W - 40° W", SnapshotIds = new List<int>{14}},
                new SnapshotGroup{Name = "40° W - 30° W", SnapshotIds = new List<int>{15}},
                new SnapshotGroup{Name = "30° W - 20° W", SnapshotIds = new List<int>{16}},
                new SnapshotGroup{Name = "20° W - 10° W", SnapshotIds = new List<int>{17}},
                new SnapshotGroup{Name = "10° W - -0° W", SnapshotIds = new List<int>{18}},
                new SnapshotGroup{Name = "0° E - 10° E", SnapshotIds = new List<int>{19}},
                new SnapshotGroup{Name = "10° E - 20° E", SnapshotIds = new List<int>{20}},
                new SnapshotGroup{Name = "20° E - 30° E", SnapshotIds = new List<int>{21}},
                new SnapshotGroup{Name = "30° E - 40° E", SnapshotIds = new List<int>{22}},
                new SnapshotGroup{Name = "40° E - 50° E", SnapshotIds = new List<int>{23}},
                new SnapshotGroup{Name = "50° E - 60° E", SnapshotIds = new List<int>{24}},
                new SnapshotGroup{Name = "60° E - 70° E", SnapshotIds = new List<int>{25}},
                new SnapshotGroup{Name = "70° E - 80° E", SnapshotIds = new List<int>{26}},
                new SnapshotGroup{Name = "80° E - 90° E", SnapshotIds = new List<int>{27}},
                new SnapshotGroup{Name = "90° E - 100° E", SnapshotIds = new List<int>{28}},
                new SnapshotGroup{Name = "100° E - 110° E", SnapshotIds = new List<int>{29}},
                new SnapshotGroup{Name = "110° E - 120° E", SnapshotIds = new List<int>{30}},
                new SnapshotGroup{Name = "120° E - 130° E", SnapshotIds = new List<int>{31}},
                new SnapshotGroup{Name = "130° E - 140° E", SnapshotIds = new List<int>{32}},
                new SnapshotGroup{Name = "140° E - 150° E", SnapshotIds = new List<int>{33}},
                new SnapshotGroup{Name = "150° E - 160° E", SnapshotIds = new List<int>{34}},
                new SnapshotGroup{Name = "160° E - 170° E", SnapshotIds = new List<int>{35}},
                new SnapshotGroup{Name = "170° E - 180° E", SnapshotIds = new List<int>{36}}
            };

            GroupSnapshotsTest(_ => _.GroupSnapshotsByLongitude(), snapshots, expected);
        }

        [Fact]
        public void GroupSnapshotsByLongitude_ShouldHandleIncompleteRangeOfLongitudes()
        {
            var snapshots = new List<LocationSnapshot>
            {
                new LocationSnapshot{Id = 1, Longitude = -178},
                new LocationSnapshot{Id = 2, Longitude = -171},
                new LocationSnapshot{Id = 3, Longitude = -23},
                new LocationSnapshot{Id = 4, Longitude = -21},
                new LocationSnapshot{Id = 5, Longitude = 47},
                new LocationSnapshot{Id = 6, Longitude = 43},
                new LocationSnapshot{Id = 7, Longitude = 158},
                new LocationSnapshot{Id = 8, Longitude = 159}
            };
            var expected = new List<SnapshotGroup>
            {
                new SnapshotGroup{Name = "180° W - 170° W", SnapshotIds = new List<int>{1,2}},
                new SnapshotGroup{Name = "30° W - 20° W", SnapshotIds = new List<int>{3,4}},
                new SnapshotGroup{Name = "40° E - 50° E", SnapshotIds = new List<int>{5,6}},
                new SnapshotGroup{Name = "150° E - 160° E", SnapshotIds = new List<int>{7,8}},
            };

            GroupSnapshotsTest(_ => _.GroupSnapshotsByLongitude(), snapshots, expected);
        }

        [Fact]
        public void GroupSnapshotsByLatitude_ShouldHandleFullRangeOfLatitudes()
        {
            var snapshots = new List<LocationSnapshot>
            {
                new LocationSnapshot{Id = 1, Latitude = -88},
                new LocationSnapshot{Id = 2, Latitude = -78},
                new LocationSnapshot{Id = 3, Latitude = -68},
                new LocationSnapshot{Id = 4, Latitude = -58},
                new LocationSnapshot{Id = 5, Latitude = -48},
                new LocationSnapshot{Id = 6, Latitude = -38},
                new LocationSnapshot{Id = 7, Latitude = -28},
                new LocationSnapshot{Id = 8, Latitude = -18},
                new LocationSnapshot{Id = 9, Latitude = -8},
                new LocationSnapshot{Id = 10, Latitude = 8},
                new LocationSnapshot{Id = 11, Latitude = 18},
                new LocationSnapshot{Id = 12, Latitude = 28},
                new LocationSnapshot{Id = 13, Latitude = 38},
                new LocationSnapshot{Id = 14, Latitude = 48},
                new LocationSnapshot{Id = 15, Latitude = 58},
                new LocationSnapshot{Id = 16, Latitude = 68},
                new LocationSnapshot{Id = 17, Latitude = 78},
                new LocationSnapshot{Id = 18, Latitude = 88}
            };
            var expected = new List<SnapshotGroup>
            {
                new SnapshotGroup{Name = "90° S - 80° S", SnapshotIds = new List<int>{1}},
                new SnapshotGroup{Name = "80° S - 70° S", SnapshotIds = new List<int>{2}},
                new SnapshotGroup{Name = "70° S - 60° S", SnapshotIds = new List<int>{3}},
                new SnapshotGroup{Name = "60° S - 50° S", SnapshotIds = new List<int>{4}},
                new SnapshotGroup{Name = "50° S - 40° S", SnapshotIds = new List<int>{5}},
                new SnapshotGroup{Name = "40° S - 30° S", SnapshotIds = new List<int>{6}},
                new SnapshotGroup{Name = "30° S - 20° S", SnapshotIds = new List<int>{7}},
                new SnapshotGroup{Name = "20° S - 10° S", SnapshotIds = new List<int>{8}},
                new SnapshotGroup{Name = "10° S - -0° S", SnapshotIds = new List<int>{9}},
                new SnapshotGroup{Name = "0° N - 10° N", SnapshotIds = new List<int>{10}},
                new SnapshotGroup{Name = "10° N - 20° N", SnapshotIds = new List<int>{11}},
                new SnapshotGroup{Name = "20° N - 30° N", SnapshotIds = new List<int>{12}},
                new SnapshotGroup{Name = "30° N - 40° N", SnapshotIds = new List<int>{13}},
                new SnapshotGroup{Name = "40° N - 50° N", SnapshotIds = new List<int>{14}},
                new SnapshotGroup{Name = "50° N - 60° N", SnapshotIds = new List<int>{15}},
                new SnapshotGroup{Name = "60° N - 70° N", SnapshotIds = new List<int>{16}},
                new SnapshotGroup{Name = "70° N - 80° N", SnapshotIds = new List<int>{17}},
                new SnapshotGroup{Name = "80° N - 90° N", SnapshotIds = new List<int>{18}}
            };

            GroupSnapshotsTest(_ => _.GroupSnapshotsByLatitude(), snapshots, expected);
        }

        [Fact]
        public void GroupSnapshotsByLatitude_ShouldHandleIncompleteRangeOfLatitudes()
        {
            var snapshots = new List<LocationSnapshot>
            {
                new LocationSnapshot{Id = 1, Latitude = -88},
                new LocationSnapshot{Id = 2, Latitude = -81},
                new LocationSnapshot{Id = 3, Latitude = -18},
                new LocationSnapshot{Id = 4, Latitude = -19},
                new LocationSnapshot{Id = 5, Latitude = 34},
                new LocationSnapshot{Id = 6, Latitude = 37},
                new LocationSnapshot{Id = 7, Latitude = 66},
                new LocationSnapshot{Id = 8, Latitude = 65}
            };
            var expected = new List<SnapshotGroup>
            {
                new SnapshotGroup{Name = "90° S - 80° S", SnapshotIds = new List<int>{1,2}},
                new SnapshotGroup{Name = "20° S - 10° S", SnapshotIds = new List<int>{3,4}},
                new SnapshotGroup{Name = "30° N - 40° N", SnapshotIds = new List<int>{5,6}},
                new SnapshotGroup{Name = "60° N - 70° N", SnapshotIds = new List<int>{7,8}}
            };

            GroupSnapshotsTest(_ => _.GroupSnapshotsByLatitude(), snapshots, expected);
        }

        [Fact]
        public void GroupSnapshotsByLatitude_ShouldHandleFullRangeOfAltitudes()
        {
            var snapshots = new List<LocationSnapshot>
            {
                new LocationSnapshot{Id = 1, Altitude = -419},
                new LocationSnapshot{Id = 2, Altitude = 719},
                new LocationSnapshot{Id = 3, Altitude = 1719},
                new LocationSnapshot{Id = 4, Altitude = 2719},
                new LocationSnapshot{Id = 5, Altitude = 3719},
                new LocationSnapshot{Id = 6, Altitude = 4719},
                new LocationSnapshot{Id = 7, Altitude = 5719},
                new LocationSnapshot{Id = 8, Altitude = 6719},
                new LocationSnapshot{Id = 9, Altitude = 7719},
                new LocationSnapshot{Id = 10, Altitude = 8719}
            };
            var expected = new List<SnapshotGroup>
            {
                new SnapshotGroup{Name = "1000 MBSL - -0 MBSL", SnapshotIds = new List<int>{1}},
                new SnapshotGroup{Name = "0 MASL - 1000 MASL", SnapshotIds = new List<int>{2}},
                new SnapshotGroup{Name = "1000 MASL - 2000 MASL", SnapshotIds = new List<int>{3}},
                new SnapshotGroup{Name = "2000 MASL - 3000 MASL", SnapshotIds = new List<int>{4}},
                new SnapshotGroup{Name = "3000 MASL - 4000 MASL", SnapshotIds = new List<int>{5}},
                new SnapshotGroup{Name = "4000 MASL - 5000 MASL", SnapshotIds = new List<int>{6}},
                new SnapshotGroup{Name = "5000 MASL - 6000 MASL", SnapshotIds = new List<int>{7}},
                new SnapshotGroup{Name = "6000 MASL - 7000 MASL", SnapshotIds = new List<int>{8}},
                new SnapshotGroup{Name = "7000 MASL - 8000 MASL", SnapshotIds = new List<int>{9}},
                new SnapshotGroup{Name = "8000 MASL - 9000 MASL", SnapshotIds = new List<int>{10}}
            };

            GroupSnapshotsTest(_ => _.GroupSnapshotsByAltitude(), snapshots, expected);
        }

        [Fact]
        public void GroupSnapshotsByLatitude_ShouldHandleIncompleteRangeOfAltitudes()
        {
            var snapshots = new List<LocationSnapshot>
            {
                new LocationSnapshot{Id = 1, Altitude = -500},
                new LocationSnapshot{Id = 2, Altitude = -600},
                new LocationSnapshot{Id = 3, Altitude = 2500},
                new LocationSnapshot{Id = 4, Altitude = 2900},
                new LocationSnapshot{Id = 5, Altitude = 6150},
                new LocationSnapshot{Id = 6, Altitude = 6900},
                new LocationSnapshot{Id = 7, Altitude = 8500},
                new LocationSnapshot{Id = 8, Altitude = 8848}
            };
            var expected = new List<SnapshotGroup>
            {
                new SnapshotGroup{Name = "1000 MBSL - -0 MBSL", SnapshotIds = new List<int>{1,2}},
                new SnapshotGroup{Name = "2000 MASL - 3000 MASL", SnapshotIds = new List<int>{3,4}},
                new SnapshotGroup{Name = "6000 MASL - 7000 MASL", SnapshotIds = new List<int>{5,6}},
                new SnapshotGroup{Name = "8000 MASL - 9000 MASL", SnapshotIds = new List<int>{7,8}}
            };

            GroupSnapshotsTest(_ => _.GroupSnapshotsByAltitude(), snapshots, expected);
        }

        private void GroupSnapshotsTest(Func<LocationSnapshotDataService, IEnumerable<SnapshotGroup>> testLogic, List<LocationSnapshot> snapshots, List<SnapshotGroup> expected)
        {
            SetUp();
            _context.LocationSnapshots.Returns(snapshots);

            var service = new LocationSnapshotDataService(_factory);
            var actual = testLogic(service).ToList();

            for (var i = 0; i < actual.Count; i++)
            {
                Assert.Equal(expected[i].Name, actual[i].Name);
                Assert.Equal(expected[i].SnapshotIds, actual[i].SnapshotIds);
            }
        }

        [Fact]
        public void GetSnapshots_ByLocationId()
        {
            var snapshots = new List<LocationSnapshot>
            {
                new LocationSnapshot{Id = 1, LocationId = 1},
                new LocationSnapshot{Id = 2, LocationId = 1},
                new LocationSnapshot{Id = 3, LocationId = 2},
                new LocationSnapshot{Id = 4, LocationId = 2}
            };
            var expected = new List<LocationSnapshot>
            {
                new LocationSnapshot{Id = 3, LocationId = 2},
                new LocationSnapshot{Id = 4, LocationId = 2}
            };

            GetSnapshotsTest(_ => _.GetSnapshotsByLocationId(2), snapshots, expected);
        }

        [Fact]
        public void GetSnapshots_ByIds()
        {
            var snapshots = new List<LocationSnapshot>
            {
                new LocationSnapshot{Id = 1, LocationId = 1},
                new LocationSnapshot{Id = 2, LocationId = 1},
                new LocationSnapshot{Id = 3, LocationId = 2},
                new LocationSnapshot{Id = 4, LocationId = 2}
            };
            var expected = new List<LocationSnapshot>
            {
                new LocationSnapshot{Id = 2, LocationId = 1},
                new LocationSnapshot{Id = 3, LocationId = 2}
            };

            GetSnapshotsTest(_ => _.GetSnapshotsByIds(new List<int> { 2, 3 }), snapshots, expected);
        }

        private void GetSnapshotsTest(Func<LocationSnapshotDataService, IEnumerable<LocationSnapshot>> testLogic, List<LocationSnapshot> snapshots, List<LocationSnapshot> expected)
        {
            SetUp();
            _context.LocationSnapshots.Returns(snapshots);

            var service = new LocationSnapshotDataService(_factory);
            var actual = testLogic(service).ToList();

            for (var i = 0; i < actual.Count; i++)
            {
                Assert.Equal(expected[i].Id, actual[i].Id);
            }
        }

        [Fact]
        public async void AddSnapshot_ShouldAddToDbContext()
        {
            // Arrange
            SetUp();
            var toAdd = new LocationSnapshot { Id = 2, LocationId = 1 };

            // Act
            var sit = new LocationSnapshotDataService(_factory);
            await sit.AddSnapshotAsync(toAdd);

            // Assert
            _context.Received().Add(toAdd);
        }

        [Fact]
        public async void UpdateSnapshot_ShouldSaveDbContextChanges()
        {
            // Arrange
            SetUp();
            var payload = new LocationSnapshot
            {
                Id = 2,
                LocationId = 1,
                Longitude = 19,
                Latitude = 51,
                Altitude = 200
            };
            var snapshotInDb = new LocationSnapshot { Id = 2 };
            _context.LocationSnapshots.Returns(new List<LocationSnapshot> { snapshotInDb });

            // Act
            var sit = new LocationSnapshotDataService(_factory);
            var result = await sit.UpdateSnapshotAsync(payload);

            // Assert
            _context.Received().SaveChanges();
            Assert.Equal(payload.Longitude, result.Longitude);
            Assert.Equal(payload.Latitude, result.Latitude);
            Assert.Equal(payload.Altitude, result.Altitude);
        }

        [Fact]
        public async void RemoveSnapshot_ShouldRemoveFromDbContext()
        {
            // Arrange
            SetUp();
            var snapshotInDb = new LocationSnapshot { Id = 2 };
            _context.LocationSnapshots.Returns(new List<LocationSnapshot> { snapshotInDb });

            // Act
            var sit = new LocationSnapshotDataService(_factory);
            await sit.RemoveSnapshotAsync(2);

            // Assert
            _context.Received().Remove(snapshotInDb);
        }

        private void SetUp()
        {
            _factory = Substitute.For<ILocationContextFactory>();
            _context = Substitute.For<ILocationContext>();
            _factory.Create().Returns(_context);
        }
    }
}
