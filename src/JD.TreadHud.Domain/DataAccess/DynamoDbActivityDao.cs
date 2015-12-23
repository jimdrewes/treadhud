using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using JD.TreadHud.Domain.Models;
using Amazon;
using Amazon.Runtime;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

namespace JD.TreadHud.Domain.DataAccess
{
    public class DynamoDbActivityDao : IActivityDao
    {
        private const int TIMEOUT_IN_MS = 5000;
        private const string ACCESS_KEY_ENV_NAME = "AWS_ACCESS_KEY";
        private const string SECRET_KEY_ENV_NAME = "AWS_SECRET_KEY";
        private static AmazonDynamoDBClient _client;
        private static DynamoDBContext _context;
        private readonly ILogger<DynamoDbActivityDao> _logger;
        
        public DynamoDbActivityDao(ILogger<DynamoDbActivityDao> logger)
        {
            _logger = logger;
            _logger.LogInformation("Using DynamoDB as Activity Data Repository.");

            string accessKey = Environment.GetEnvironmentVariable(ACCESS_KEY_ENV_NAME);
            string secretKey = Environment.GetEnvironmentVariable(SECRET_KEY_ENV_NAME);
            
            RegionEndpoint endpoint = RegionEndpoint.USEast1;
            AWSCredentials credentials = new BasicAWSCredentials(accessKey, secretKey);

            _client = new AmazonDynamoDBClient(credentials, endpoint);
            _context = new DynamoDBContext(_client);
            
            _logger.LogInformation("Finished connecting to Amazon Client.");
        }
        
        public bool AddActivity(Activity activity)
        {
            try
            {
                _logger.LogInformation("Saving Activity {0} via DynamoDB", activity.ActivityId);
                
                DateTime timeout = DateTime.Now.AddMilliseconds(TIMEOUT_IN_MS);
                var task = _context.SaveAsync(activity);
                
                // Essentially this causes the operation to complete syncronously.  Need to re-factor to
                // handle true async saving.
                while (!task.IsCompleted && DateTime.Now < timeout) { }

                if (task.Status != TaskStatus.RanToCompletion)
                {
                    _logger.LogError("Failed to Save Activity {0} - status:  {1}, exception: {2}", activity.ActivityId, task.Status.ToString(), task.Exception == null ? "" : task.Exception.InnerException.Message);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to Save Activity {0} - Exception: {1}", activity.ActivityId, ex.Message);
                return false;
            }

            return true;
        }

        public IList<Activity> GetAllActivities()
        {
            try
            {
                _logger.LogInformation("Getting all activities.");
            }
            catch (Exception ex)
            {
                _logger.LogError("***** ERROR ADDING ACTIVITY *****");
                _logger.LogError(ex.Message);
            }
            
            return null;
        }
    }
}