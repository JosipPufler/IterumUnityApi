using Amazon.S3.Transfer;
using Amazon.S3;
using Amazon;
using Amazon.S3.Model;
using System.Xml.Linq;
using System.IO;
using IterumApi.DTOs;

namespace IterumApi.Services
{
    public static class BucketService
    {
        private static readonly string bucketName = "iterum-bucket";
        private static readonly string accessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID");
        private static readonly string secretKey = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY");
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.EUNorth1;

        private static readonly IAmazonS3 s3Client = new AmazonS3Client(accessKey, secretKey, bucketRegion);

        public static async Task UploadFileAsync(string filePath, string keyName)
        {
            var fileTransferUtility = new TransferUtility(s3Client);

            await fileTransferUtility.UploadAsync(filePath, bucketName, keyName);
            Console.WriteLine("Upload completed.");
        }

        public static async Task UploadStreamAsync(Stream fileStream, string keyName, string contentType = "application/octet-stream")
        {
            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = fileStream,
                Key = keyName,
                BucketName = bucketName,
                ContentType = contentType,
                CannedACL = S3CannedACL.Private
            };

            var fileTransferUtility = new TransferUtility(s3Client);
            await fileTransferUtility.UploadAsync(uploadRequest);
        }

        public static async Task<JournalDto?> DownloadJournalAsync(string keyName, string username)
        {
            string path = $"{username}/journal/{keyName}.txt";
            return await DownloadJournalAsync(path); 
        }

        public static async Task<JournalDto?> DownloadJournalAsync(string path)
        {
            try
            {
                var response = await s3Client.GetObjectAsync(bucketName, path);
                var memoryStream = new MemoryStream();

                await response.ResponseStream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                using var reader = new StreamReader(memoryStream);
                var content = await reader.ReadToEndAsync();

                return new JournalDto() { Name = Path.GetFileNameWithoutExtension(path), LastModified = response.LastModified, Content = content };
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine($"S3 Error: {e.Message}");
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine($"General Error: {e.Message}");
                return null;
            }
        }

        public static async Task<Stream> DownloadStreamAsync(string keyName)
        {
            var response = await s3Client.GetObjectAsync(bucketName, keyName);
            var memoryStream = new MemoryStream();
            await response.ResponseStream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;
            return memoryStream;
        }

        public static async Task<bool> DeleteFileAsync(string keyName)
        {
            try
            {
                var deleteObjectRequest = new DeleteObjectRequest
                {
                    BucketName = bucketName,
                    Key = keyName
                };

                await s3Client.DeleteObjectAsync(deleteObjectRequest);
                return true;
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine($"S3 Delete Error: {e.Message}");
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine($"General Delete Error: {e.Message}");
                return false;
            }
        }

        public static async Task<List<string>> ListFilesWithExtensionsAsync(string userFolderPrefix, string[] extensions)
        {
            var matchedKeys = new List<string>();

            try
            {
                var request = new ListObjectsV2Request
                {
                    BucketName = bucketName,
                    Prefix = userFolderPrefix.EndsWith("/") ? userFolderPrefix : userFolderPrefix + "/"
                };

                ListObjectsV2Response response;
                do
                {
                    response = await s3Client.ListObjectsV2Async(request);

                    if (response.S3Objects == null)
                    {
                        return matchedKeys;
                    }

                    foreach (var obj in response.S3Objects)
                    {
                        foreach (var ext in extensions)
                        {
                            if (obj.Key.EndsWith(ext, StringComparison.OrdinalIgnoreCase))
                            {
                                matchedKeys.Add(obj.Key);
                                break; // Match found, skip checking other extensions
                            }
                        }
                    }

                    request.ContinuationToken = response.NextContinuationToken;
                } while (response.IsTruncated != null && response.IsTruncated == true);
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine($"S3 Error: {e.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"General Error: {e.Message}");
            }

            return matchedKeys;
        }
    }
}
