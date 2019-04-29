using LocationCapture.Enums;

namespace LocationCapture.BL
{
    public interface IDataServiceFactory
    {
        ILocationDataService CreateLocationDataService(DataSourceType dataStorageType);
        ILocationSnapshotDataService CreateLocationSnapshotDataService(DataSourceType dataStorageType);
        IPictureService CreatePictureService(DataSourceType dataStorageType);
    }
}
