private async Task<bool> UploadFileAsync(string targetPath, string localFilePath)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;

                var s3Client = new AmazonS3Client(Helper.masterSetting.AccessKey, Helper.masterSetting.SecretKey, new AmazonS3Config
                {
                    ServiceURL = $"https://{Helper.masterSetting.SpacesEndpoint}"
                });

                var transferUtility = new TransferUtility(s3Client);

                using (var fileStream = new FileStream(localFilePath, FileMode.Open))
                {
                    var request = new TransferUtilityUploadRequest
                    {
                        BucketName = Helper.masterSetting.BucketName,
                        Key = targetPath,
                        InputStream = fileStream,
                        CannedACL = S3CannedACL.PublicRead, // Adjust the ACL based on your requirements
                        PartSize = 5 * 1024 * 1024, 
                        DisablePayloadSigning = true// Set a custom part size for multipart upload, if desired

                    };
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
                    // Subscribe to the event to track progress
                    request.UploadProgressEvent += new EventHandler<UploadProgressArgs>(progressChanged);

                    await transferUtility.UploadAsync(request);
                }
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }

        }
