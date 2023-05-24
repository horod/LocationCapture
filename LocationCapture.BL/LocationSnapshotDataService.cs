using System.Linq;
using System.Collections.Generic;
using LocationCapture.Models;
using LocationCapture.DAL;
using System;
using System.Globalization;
using LocationCapture.Extensions;
using LocationCapture.Enums;
using System.Threading.Tasks;

namespace LocationCapture.BL
{
    public class LocationSnapshotDataService : ILocationSnapshotDataService
    {
        private readonly ILocationContextFactory _dataContextFactory;

        public LocationSnapshotDataService(ILocationContextFactory dataContextFactory)
        {
            _dataContextFactory = dataContextFactory;
        }

        public IEnumerable<SnapshotGroup> GroupSnapshotsByCreatedDate(DateTime currentDate)
        {
            var lastDayOfCurrentMonth = currentDate.ExtractLastDayOfMonth();

            using (var context = _dataContextFactory.Create())
            {
                var snapshotCount = context.LocationSnapshots.Count();

                if (snapshotCount == 0)
                {
                    return new List<SnapshotGroup>();
                }

                var oldestSnapshotDate = context.LocationSnapshots.Min(x => x.DateCreated);
                var newestSnapshotDate = context.LocationSnapshots.Max(x => x.DateCreated);
                var monthsApart = (newestSnapshotDate.Year - oldestSnapshotDate.Year) * 12 + newestSnapshotDate.Month - oldestSnapshotDate.Month;

                var headersWithDates = Enumerable.Range(0, monthsApart + 1)
                    .Select(_ => lastDayOfCurrentMonth.AddMonths(-1 * _).ExtractLastDayOfMonth())
                    .Select(_ => new HeaderWithDate {
                        Header = $"{_.ToString("MMMM", CultureInfo.InvariantCulture)} {_.Year}",
                        Date = _});

                var headersWithIds = headersWithDates.SelectMany(hwd => context.LocationSnapshots
                    .Where(ls => ls.DateCreated <= hwd.Date.ExtractEndOfDay() && ls.DateCreated >= hwd.Date.ExtractFirstDayOfMonth())
                    .Select(ls => new HeaderWithId { Header = hwd.Header, Id = ls.Id }));

                var snapshotGroups = headersWithIds.GroupBy(_ => _.Header)
                    .Select(_ => new SnapshotGroup
                    {
                        Name = _.Key,
                        SnapshotIds = _.Select(__ => __.Id).ToList()
                    });
                
                return snapshotGroups.ToList();
            }
        }

        public IEnumerable<SnapshotGroup> GroupSnapshotsByLongitude()
        {
            var criteria = new GroupByGeoCoordinateCriteria
            {
                NumberOfRanges = 36,
                MinValue = -180,
                Offset = 10,
                DisplaySuffixNegative = $"° {LongitudeDirection.W}",
                DisplaySuffixPositive = $"° {LongitudeDirection.E}",
                CoordinateFieldRetriever = ls => ls.Longitude
            };
            return GroupSnapshotsByGeoCoordinate(criteria);
        }

        public IEnumerable<SnapshotGroup> GroupSnapshotsByLatitude()
        {
            var criteria = new GroupByGeoCoordinateCriteria
            {
                NumberOfRanges = 18,
                MinValue = -90,
                Offset = 10,
                DisplaySuffixNegative =$"° {LatitudeDirection.S}",
                DisplaySuffixPositive = $"° {LatitudeDirection.N}",
                CoordinateFieldRetriever = ls => ls.Latitude
            };
            return GroupSnapshotsByGeoCoordinate(criteria);
        }

        public IEnumerable<SnapshotGroup> GroupSnapshotsByAltitude()
        {
            var criteria = new GroupByGeoCoordinateCriteria
            {
                NumberOfRanges = 10,
                MinValue = -1000,
                Offset = 1000,
                DisplaySuffixNegative = $" {AltitudeDirection.MBSL}",
                DisplaySuffixPositive = $" {AltitudeDirection.MASL}",
                CoordinateFieldRetriever = ls => ls.Altitude
            };
            return GroupSnapshotsByGeoCoordinate(criteria);
        }

        private IEnumerable<SnapshotGroup> GroupSnapshotsByGeoCoordinate(GroupByGeoCoordinateCriteria criteria)
        {
            using (var context = _dataContextFactory.Create())
            {
                var headersWithGeoCoordinates = Enumerable.Range(0, criteria.NumberOfRanges)
                    .Select(_ => new HeaderWithGeoCoordinate
                    {
                        GeoCoordLowerBound = criteria.MinValue + _ * criteria.Offset,
                        GeoCoordUpperBound = criteria.MinValue + _ * criteria.Offset + criteria.Offset,
                        IsPositive = (criteria.MinValue + _ * criteria.Offset >= 0)

                    })
                    .Select(_ => new HeaderWithGeoCoordinate
                    {
                        Header = !_.IsPositive
                            ? $"{_.GeoCoordLowerBound * -1}{criteria.DisplaySuffixNegative} - {_.GeoCoordUpperBound * -1}{criteria.DisplaySuffixNegative}"
                            : $"{_.GeoCoordLowerBound}{criteria.DisplaySuffixPositive} - {_.GeoCoordUpperBound}{criteria.DisplaySuffixPositive}",
                        GeoCoordLowerBound = _.GeoCoordLowerBound,
                        GeoCoordUpperBound = _.GeoCoordUpperBound
                    })
                    .ToList();

                var headersWithIds = headersWithGeoCoordinates.SelectMany(hwl => context.LocationSnapshots
                    .Where(ls => criteria.CoordinateFieldRetriever(ls) >= hwl.GeoCoordLowerBound && criteria.CoordinateFieldRetriever(ls) < hwl.GeoCoordUpperBound)
                    .Select(ls => new HeaderWithId { Header = hwl.Header, Id = ls.Id }));

                var snapshotGroups = headersWithIds.GroupBy(_ => _.Header)
                    .Select(_ => new SnapshotGroup
                    {
                        Name = _.Key,
                        SnapshotIds = _.Select(__ => __.Id).ToList()
                    })
                    .ToList();

                return snapshotGroups;
            }
        }

        public IEnumerable<LocationSnapshot> GetSnapshotsByLocationId(int locationId)
        {
            using (var context = _dataContextFactory.Create())
            {
                var snapshots = context.LocationSnapshots
                    .Where(_ => _.LocationId == locationId)
                    .ToList();

                return snapshots;
            }
        }

        public IEnumerable<LocationSnapshot> GetSnapshotsByIds(IEnumerable<int> snapshotIds)
        {
            using (var context = _dataContextFactory.Create())
            {
                var snapshots = context.LocationSnapshots
                    .Where(_ => snapshotIds.Contains(_.Id))
                    .ToList();

                return snapshots;
            }
        }

        private LocationSnapshot AddSnapshot(LocationSnapshot snapshotToAdd)
        {
            using (var context = _dataContextFactory.Create())
            {
                return context.Add(snapshotToAdd);
            }
        }

        private LocationSnapshot UpdateSnapshot(LocationSnapshot snapshot)
        {
            using (var context = _dataContextFactory.Create())
            {
                var snapshotToUpdate = context.LocationSnapshots
                    .First(_ => _.Id == snapshot.Id);

                snapshotToUpdate.Longitude = snapshot.Longitude;
                snapshotToUpdate.Latitude = snapshot.Latitude;
                snapshotToUpdate.Altitude = snapshot.Altitude;
                snapshotToUpdate.Thumbnail = !string.IsNullOrEmpty(snapshot.Thumbnail) ? snapshot.Thumbnail : snapshotToUpdate.Thumbnail;

                context.SaveChanges();

                return snapshotToUpdate;
            }
        }

        private LocationSnapshot RemoveSnapshot(int snapshotId)
        {
            using (var context = _dataContextFactory.Create())
            {
                var snapshotToRemove = context.LocationSnapshots
                    .First(_ => _.Id == snapshotId);

                return context.Remove(snapshotToRemove);
            }
        }

        private async Task<IEnumerable<SnapshotGroup>> GroupSnapshotsByCreatedDateAsync(DateTime currentDate)
        {
            var result = await Task.Factory.StartNew(() => GroupSnapshotsByCreatedDate(currentDate));
            return result;
        }

        private async Task<IEnumerable<SnapshotGroup>> GroupSnapshotsByLongitudeAsync()
        {
            var result = await Task.Factory.StartNew(() => GroupSnapshotsByLongitude());
            return result;
        }

        private async Task<IEnumerable<SnapshotGroup>> GroupSnapshotsByLatitudeAsync()
        {
            var result = await Task.Factory.StartNew(() => GroupSnapshotsByLatitude());
            return result;
        }

        private async Task<IEnumerable<SnapshotGroup>> GroupSnapshotsByAltitudeAsync()
        {
            var result = await Task.Factory.StartNew(() => GroupSnapshotsByAltitude());
            return result;
        }

        public async Task<IEnumerable<LocationSnapshot>> GetSnapshotsByLocationIdAsync(int locationId)
        {
            var result = await Task.Factory.StartNew(() => GetSnapshotsByLocationId(locationId));
            return result;
        }

        public async Task<IEnumerable<LocationSnapshot>> GetSnapshotsByIdsAsync(IEnumerable<int> snapshotIds)
        {
            var result = await Task.Factory.StartNew(() => GetSnapshotsByIds(snapshotIds));
            return result;
        }

        public async Task<LocationSnapshot> AddSnapshotAsync(LocationSnapshot snapshotToAdd)
        {
            var result = await Task.Factory.StartNew(() => AddSnapshot(snapshotToAdd));
            return result;
        }

        public async Task<LocationSnapshot> UpdateSnapshotAsync(LocationSnapshot snapshot)
        {
            var result = await Task.Factory.StartNew(() => UpdateSnapshot(snapshot));
            return result;
        }

        public async Task<LocationSnapshot> RemoveSnapshotAsync(int snapshotId)
        {
            var result = await Task.Factory.StartNew(() => RemoveSnapshot(snapshotId));
            return result;
        }

        public Func<Task<IEnumerable<SnapshotGroup>>> ChooseGroupByOperation(GroupByCriteria groupBy)
        {
            switch (groupBy)
            {
                case GroupByCriteria.CreatedDate:
                    return new Func<Task<IEnumerable<SnapshotGroup>>>(() => GroupSnapshotsByCreatedDateAsync(DateTime.Now));
                case GroupByCriteria.Longitude:
                    return new Func<Task<IEnumerable<SnapshotGroup>>>(() => GroupSnapshotsByLongitudeAsync());
                case GroupByCriteria.Latitude:
                    return new Func<Task<IEnumerable<SnapshotGroup>>>(() => GroupSnapshotsByLatitudeAsync());
                case GroupByCriteria.Altitude:
                    return new Func<Task<IEnumerable<SnapshotGroup>>>(() => GroupSnapshotsByAltitudeAsync());
                default:
                    throw new ArgumentOutOfRangeException("Could not recognize the specified group by criteria.");
            }
        }

        // not using anonymous types in LINQ queries, as they have poor performance on .NET Native
        private struct HeaderWithDate
        {
            public string Header { get; set; }
            public DateTime Date { get; set; }
        }

        private struct HeaderWithGeoCoordinate
        {
            public string Header { get; set; }
            public double GeoCoordLowerBound { get; set; }
            public double GeoCoordUpperBound { get; set; }
            public bool IsPositive { get; set; }
        }

        private struct GroupByGeoCoordinateCriteria
        {
            public int NumberOfRanges { get; set; }
            public int MinValue { get; set; }
            public int Offset { get; set; }
            public string DisplaySuffixPositive { get; set; }
            public string DisplaySuffixNegative { get; set; }
            public Func<LocationSnapshot, double> CoordinateFieldRetriever { get; set; }
        }

        private struct HeaderWithId
        {
            public string Header { get; set; }
            public int Id { get; set; }
        }
    }
}