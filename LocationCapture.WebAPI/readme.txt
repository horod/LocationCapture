How to run the LocationCapture API on the local IIS instance:

- Install .NET Core Runtime 2.0.9 and .NET Core SDK 2.1.202,
- Create app pool called LocationCapture (no managed code),
- Create application (under Default Web Site) called LocationCapture,
- Set the app physical path to C:\inetpub\LocationCapture\wwwroot, copy all the app files there,
- Assign the LocationCapture app pool to the LocationCapture app,
- Create the virtual directory from the C:\inetpub\LocationCapture\wwwroot\Snapshot_Pictures folder,
- Grant read/write permissions on the C:\inetpub\LocationCapture\wwwroot\Snapshot_Pictures folder to the IIS APPPOOL\LocationCapture account,
- Create the SQL Server login for the IIS APPPOOL\LocationCapture and grant it read/write permissions to the LocationCapture database,
- Publish the web API from Visual Studio using the following settings:
-- Configuration: Release (Any CPU),
-- Target Framework: dotnetcoreapp2.0,
-- Deployment Mode: Framework Dependent,
-- Target Runtime: Portable
- If things still don't work, check eventvwr for error logs.