using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;

namespace RankingVideojuegos.Services
{
    public enum S3Folder
    {
        Perfil,
        Videojuegos
    }

    public class S3Service
    {
        private readonly string? _bucketName;
        private readonly string? _accessKey;
        private readonly string? _secretKey;
        private readonly RegionEndpoint? _region;

        public S3Service(IConfiguration configuration)
        {
            _bucketName = configuration["AWS:BucketName"];
            _accessKey = configuration["AWS:AccessKey"];
            _secretKey = configuration["AWS:SecretKey"];
            _region = RegionEndpoint.GetBySystemName(configuration["AWS:Region"]);
        }

        public async Task<string> UploadToS3Async(IFormFile file, S3Folder folder)
        {
            string prefix = folder switch
            {
                S3Folder.Perfil => "perfil",
                S3Folder.Videojuegos => "videojuegos",
                _ => ""
            };

            return await UploadToS3Async(file, prefix);
        }

        public async Task<string> UploadToS3Async(IFormFile file, string prefix)
        {
            using var client = new AmazonS3Client(_accessKey, _secretKey, _region);

            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            if (!string.IsNullOrEmpty(prefix))
            {
                uniqueFileName = $"{prefix.TrimEnd('/')}/{uniqueFileName}";
            }

            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = file.OpenReadStream(),
                Key = uniqueFileName,
                BucketName = _bucketName,
                ContentType = file.ContentType,
                //CannedACL = S3CannedACL.PublicRead
            };

            var transferUtility = new TransferUtility(client);
            await transferUtility.UploadAsync(uploadRequest);

            return $"https://{_bucketName}.s3.amazonaws.com/{uniqueFileName}";
        }

        public async Task DeleteFromS3Async(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl)) return;

            var key = imageUrl.Replace($"https://{_bucketName}.s3.amazonaws.com/", "");
            using var client = new AmazonS3Client(_accessKey, _secretKey, _region);

            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = key
            };

            await client.DeleteObjectAsync(deleteRequest);
        }

        public async Task<List<string>> GetAllFileUrlsAsync(string? prefix = null)
        {
            using var client = new AmazonS3Client(_accessKey, _secretKey, _region);

            var request = new ListObjectsV2Request
            {
                BucketName = _bucketName,
                Prefix = prefix
            };

            var response = await client.ListObjectsV2Async(request);

            var urls = response.S3Objects.Select(obj =>
                $"https://{_bucketName}.s3.amazonaws.com/{obj.Key}"
            ).ToList();

            return urls;
        }

        public string GetPublicFileUrl(string fileName)
        {
            return $"https://{_bucketName}.s3.amazonaws.com/{fileName}";
        }
    }
}
