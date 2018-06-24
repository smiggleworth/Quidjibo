using System;
using System.Linq;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Amazon.Util;

namespace Quidjibo.Aws.Sqs.Utils
{
    public class AWSCredentialsUtil
    {
        public static AWSCredentials LoadFromInstance()
        {
            var store = new CredentialProfileStoreChain();

            store.TryGetAWSCredentials("default", out var awsCredentials);

            if (awsCredentials == null && string.IsNullOrEmpty(EC2InstanceMetadata.InstanceId))
            {
                var firstRole = InstanceProfileAWSCredentials.GetAvailableRoles().FirstOrDefault();
                if (firstRole != null)
                {
                    awsCredentials = new InstanceProfileAWSCredentials(firstRole);
                }
            }

            if (awsCredentials == null)
            {
                throw new ArgumentException($"{nameof(awsCredentials)} could not be loaded from the ~/.aws/credentials or the instance profile. Please pass explicitly.");
            }

            return awsCredentials;
        }
    }
}