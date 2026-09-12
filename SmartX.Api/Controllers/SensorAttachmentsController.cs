using Microsoft.AspNetCore.Mvc;
using SmartX.Api.Services;
using SmartX.Shared.Models;

namespace SmartX.Api.Controllers
{
    [ApiController]
    [Route("api/sensors/{deviceId}/attachments")]
    public class SensorAttachmentsController : ControllerBase
    {
        private readonly SmartXDataStore _dataStore;
        private readonly EncryptedFileStorageService _fileStorage;

        public SensorAttachmentsController(
            SmartXDataStore dataStore,
            EncryptedFileStorageService fileStorage)
        {
            _dataStore = dataStore;
            _fileStorage = fileStorage;
        }

        [HttpPost]
        [RequestSizeLimit(50_000_000)]
        public async Task<IActionResult> Upload(
            string deviceId,
            IFormFile file,
            [FromForm] string attachmentType)
        {
            if (string.IsNullOrWhiteSpace(deviceId))
            {
                return BadRequest(new
                {
                    message = "Device ID is required."
                });
            }

            if (file == null || file.Length == 0)
            {
                return BadRequest(new
                {
                    message = "A file is required."
                });
            }

            if (string.IsNullOrWhiteSpace(attachmentType))
            {
                return BadRequest(new
                {
                    message = "Attachment type is required."
                });
            }

            Sensor? sensor = _dataStore.Sensors
                .FirstOrDefault(s =>
                    s.UniqueIdentifier.Equals(
                        deviceId,
                        StringComparison.OrdinalIgnoreCase));

            if (sensor == null)
            {
                return NotFound(new
                {
                    message = $"Sensor '{deviceId}' was not found."
                });
            }

            string extension = Path.GetExtension(file.FileName);

            string storedFileName =
                $"{Guid.NewGuid():N}{extension}.encrypted";

            await using Stream fileStream = file.OpenReadStream();

            await _fileStorage.SaveEncryptedAsync(
                fileStream,
                storedFileName);

            SensorAttachment attachment = new SensorAttachment
            {
                Id = _dataStore.GetNextAttachmentId(),
                DeviceId = sensor.UniqueIdentifier,
                FileName = Path.GetFileName(file.FileName),
                ContentType = file.ContentType,
                FileSize = file.Length,
                AttachmentType = attachmentType,
                StoredFileName = storedFileName,
                UploadedAt = DateTime.UtcNow
            };

            _dataStore.AddAttachment(attachment);

            return Ok(attachment);
        }
    }
}