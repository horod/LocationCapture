using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Collections;
using LocationCapture.WebAPI.Helpers;
using LocationCapture.WebAPI.Services;
using System.Dynamic;
using LocationCapture.BL;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace LocationCapture.WebAPI.Controllers
{
    [Route("api/snapshot-pictures")]
    public class SnapshotPicturesController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IFileSystemService _fileSystemService;
        private readonly IImageService _imageService;
        private readonly ILocationSnapshotDataService _locationSnapshotDataService;
        private readonly ILogger<SnapshotPicturesController> _logger;

        public SnapshotPicturesController(IHostingEnvironment hostingEnvironment,
            IFileSystemService fileSystemService,
            IImageService imageService,
            ILocationSnapshotDataService locationSnapshotDataService,
            ILogger<SnapshotPicturesController> logger)
        {
            _hostingEnvironment = hostingEnvironment;
            _fileSystemService = fileSystemService;
            _imageService = imageService;
            _locationSnapshotDataService = locationSnapshotDataService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetMiniatures(
            [ModelBinder(BinderType = typeof(IntArrayModelBinder), Name = "snapshotsIds")]IEnumerable<int> snapshotsIds)
        {
            if(snapshotsIds == null)
            {
                _logger.LogWarning("Bad request. Unable to deserialize snapshots IDs.");
                return BadRequest();
            }

            var results = new ArrayList();
            IEnumerable<string> pictureFileNames = (await _locationSnapshotDataService.GetSnapshotsByIdsAsync(snapshotsIds))
                .Select(_ => _.PictureFileName);

            foreach (var pictureFileName in pictureFileNames)
            {
                var base64String = await GenerateBase64EncodedThumbnail(pictureFileName);
                if (string.IsNullOrEmpty(base64String)) return NotFound();
                var snapshotMiniature = new
                {
                    PictureFileName = pictureFileName,
                    Thumbnail = base64String
                };
                results.Add(snapshotMiniature);
            }

            return Ok(results);
        }

        [HttpGet("{snapshotId:int}")]
        public async Task<IActionResult> GetMiniature(int snapshotId)
        {
            var snapshot = (await _locationSnapshotDataService.GetSnapshotsByIdsAsync(new int[] { snapshotId }))
                .FirstOrDefault();

            if (snapshot == null) return NotFound();

            var base64String = await GenerateBase64EncodedThumbnail(snapshot.PictureFileName);

            if (string.IsNullOrEmpty(base64String)) return NotFound();

            var snapshotMiniature = new
            {
                snapshot.PictureFileName,
                Thumbnail = base64String
            };

            return Ok(snapshotMiniature);
        }

        private async Task<string> GenerateBase64EncodedThumbnail(string pictureFileName)
        {
            var picturePath = GetPictureFilePath(pictureFileName);
            if (PictureNotFound(picturePath)) return string.Empty;

            var thumbnailBytes = await _imageService.GetThumbnailAsync(picturePath);
            var base64String = Convert.ToBase64String(thumbnailBytes);

            return base64String;
        }

        [HttpGet("{pictureFileName}")]
        public async Task<IActionResult> GetPicture(string pictureFileName, bool isMiniature = false)
        {
            var picturePath = GetPictureFilePath(pictureFileName);
            if (PictureNotFound(picturePath)) return NotFound();

            byte[] data;
            if(isMiniature)
            {
                data = await _imageService.GetThumbnailAsync(picturePath);
            }
            else
            {
                data = await _fileSystemService.ReadAllBytesAsync(picturePath);
            }
            var result = Convert.ToBase64String(data);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddPicture([FromBody]ExpandoObject payload)
        {
            dynamic pictureFileDescriptor = payload;
            var pictureFile = pictureFileDescriptor.pictureFile;
            var pictureFileName = pictureFileDescriptor.pictureFileName;

            if (pictureFileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                _logger.LogWarning("Bad request. Snapshot picture name contains invalid characters: {0}", pictureFileName as string);
                return BadRequest();
            }

            var pictureBytes = new byte[0];
            try
            {
                pictureBytes = Convert.FromBase64String(pictureFile);
            }
            catch(FormatException ex)
            {
                _logger.LogWarning("Bad request. Unable to decode picture from Base64 string. Details: {Ex}", ex);
                return BadRequest();
            }

            var picturePath = GetPictureFilePath(pictureFileName);

            if (_fileSystemService.FileExisits(picturePath))
            {
                _logger.LogWarning("Conflict. Picture file with the given name already exists: {0}", picturePath as string);
                return StatusCode(409, "File already exists.");
            }

            await _fileSystemService.WriteAllBytesAsync(picturePath, pictureBytes);

            return Ok();
        }

        [HttpDelete("{pictureFileName}")]
        public IActionResult DeletePicture(string pictureFileName)
        {
            var picturePath = GetPictureFilePath(pictureFileName);
            if (PictureNotFound(picturePath)) return NotFound();

            _fileSystemService.DeleteFile(picturePath);

            return Ok();
        }

        private string GetPictureFilePath(string pictureFileName) => 
            Path.Combine(_hostingEnvironment.ContentRootPath, "Snapshot_Pictures", pictureFileName);

        private bool PictureNotFound(string picturePath)
        {
            if (!_fileSystemService.FileExisits(picturePath))
            {
                _logger.LogWarning("Snapshot picture not found. Picture file path: {0}", picturePath);
                return true;
            }

            return false;
        }
    }
}