using Amazon.S3;
using Amazon.S3.Model;

namespace MvcCoreAWSS3.Services
{
    public class ServiceStorageS3
    {
        private string BucketName;

        private IAmazonS3 ClientS3;

        public ServiceStorageS3(IConfiguration configuration,
            IAmazonS3 clientS3)
        {
            this.ClientS3 = clientS3;
            this.BucketName = configuration.GetValue<string>("AWS:BucketName");
        }

        public async Task<bool> UploadFileAsync
            (string filename, Stream stream)
        {
            PutObjectRequest request = new PutObjectRequest
            {
                BucketName = this.BucketName,
                Key=filename,
                InputStream = stream
            };
            PutObjectResponse response = await this.ClientS3.PutObjectAsync
                (request);
            if(response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<bool> DeleteFileAsync
            (string filename)
        {
            DeleteObjectResponse response = await 
                this.ClientS3.DeleteObjectAsync(this.BucketName, filename);
            if(response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<List<string>> GetVersionsFileAsync()
        {
            ListVersionsResponse response = await 
                this.ClientS3.ListVersionsAsync(this.BucketName);

            List<string> keyFiles = response.Versions
                .Select(x => x.Key).ToList();
            return keyFiles;
        }

        public async Task<Stream> GetFileAsync(string fileName)
        {
            GetObjectResponse response = await this.ClientS3.GetObjectAsync
                (this.BucketName, fileName);
            return response.ResponseStream;
        }
    }
}
