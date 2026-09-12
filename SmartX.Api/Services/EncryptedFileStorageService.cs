using System.Security.Cryptography;

namespace SmartX.Api.Services
{
    public class EncryptedFileStorageService
    {
        private readonly string _storagePath;
        private readonly byte[] _encryptionKey;

        public EncryptedFileStorageService(IWebHostEnvironment environment)
        {
            _storagePath = Path.Combine(environment.ContentRootPath, "UploadedFiles");

            Directory.CreateDirectory(_storagePath);

            string? configuredKey =
                Environment.GetEnvironmentVariable("SMARTX_FILE_ENCRYPTION_KEY");

            if (string.IsNullOrWhiteSpace(configuredKey))
            {
                _encryptionKey = RandomNumberGenerator.GetBytes(32);
            }
            else
            {
                _encryptionKey = Convert.FromBase64String(configuredKey);
            }

            if (_encryptionKey.Length != 32)
            {
                throw new InvalidOperationException(
                    "SMARTX_FILE_ENCRYPTION_KEY must contain a 32-byte AES key.");
            }
        }

        public async Task<string> SaveEncryptedAsync(
            Stream sourceStream,
            string storedFileName)
        {
            string filePath = Path.Combine(_storagePath, storedFileName);

            byte[] iv = RandomNumberGenerator.GetBytes(16);

            await using FileStream fileStream = new FileStream(
                filePath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                81920,
                useAsync: true);

            await fileStream.WriteAsync(iv);

            using Aes aes = Aes.Create();

            aes.Key = _encryptionKey;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            await using CryptoStream cryptoStream =
                new CryptoStream(
                    fileStream,
                    aes.CreateEncryptor(),
                    CryptoStreamMode.Write,
                    leaveOpen: false);

            await sourceStream.CopyToAsync(cryptoStream);

            await cryptoStream.FlushAsync();

            return filePath;
        }
    }
}