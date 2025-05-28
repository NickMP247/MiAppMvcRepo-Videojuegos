using Amazon;

namespace RankingVideojuegos.Services
{
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
    }
}
