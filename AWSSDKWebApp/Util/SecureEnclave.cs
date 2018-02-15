using System.IO;
using System.Net;
using System.Threading.Tasks;
using Amazon.KeyManagementService;
using Amazon.KeyManagementService.Model;
using Amazon.S3.Encryption;
using Amazon.S3.Model;

namespace AWSSDKWebApp.Util
{
    public class SecureEnclave : ISecureEnclave
    {
        public async Task<string> ReadValue(string key)
        {
            using (var kmsClient = new AmazonKeyManagementServiceClient())
            {
                string kmsKeyID = null;
                var response = await kmsClient.CreateKeyAsync(new CreateKeyRequest());
                 kmsKeyID = response.KeyMetadata.KeyId;

                var keyMetadata = response.KeyMetadata; 
                var bucketName = "<your S3 bucket name>";
                var objectKey = key;

                var kmsEncryptionMaterials = new EncryptionMaterials(key);
                var config = new AmazonS3CryptoConfiguration()
                {
                    StorageMode = CryptoStorageMode.ObjectMetadata
                };

                using (var s3Client = new AmazonS3EncryptionClient(config, kmsEncryptionMaterials))
                {
                    var getRequest = new GetObjectRequest
                    {
                        BucketName = bucketName,
                        Key = objectKey
                    };

                    using (var getResponse = await s3Client.GetObjectAsync(getRequest))
                    using (var stream = getResponse.ResponseStream)
                    using (var reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }

        public async Task<bool> WriteValue(string key, string value)
        {
            using (var kmsClient = new AmazonKeyManagementServiceClient())
            {
                var response = await kmsClient.CreateKeyAsync(new CreateKeyRequest());
                string kmsKeyID = response.KeyMetadata.KeyId;

                var keyMetadata = response.KeyMetadata; 
                var bucketName = "<your S3 bucket name>";
                var objectKey = key;

                var kmsEncryptionMaterials = new EncryptionMaterials(kmsKeyID);
                var config = new AmazonS3CryptoConfiguration()
                {
                    StorageMode = CryptoStorageMode.ObjectMetadata
                };

                using (var s3Client = new AmazonS3EncryptionClient(config, kmsEncryptionMaterials))
                {
                    var putRequest = new PutObjectRequest
                    {
                        BucketName = bucketName,
                        Key = objectKey,
                        ContentBody = value
                    };
                    var obj = await s3Client.PutObjectAsync(putRequest);
                    if (obj.HttpStatusCode == HttpStatusCode.OK)
                        return true;

                }
            }
            return false;
        }
    }
}