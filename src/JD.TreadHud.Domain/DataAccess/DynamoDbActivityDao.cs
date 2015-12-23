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
                _logger.LogInformation("Getting all activities via DynamoDB.");

                DateTime timeout = DateTime.Now.AddMilliseconds(TIMEOUT_IN_MS);
                var search = _context.ScanAsync<Activity>(new List<ScanCondition>());

                // Essentially this causes the operation to complete syncronously.  Need to re-factor to
                // handle true async saving.
                while (!search.IsDone && DateTime.Now < timeout) { }

                if (!search.IsDone)
                {
                    _logger.LogError("Timed out while searching activity list.");
                    //return new List<Activity>();
                }

                _logger.LogInformation("Finished activity search.  Pulling results down.");
                
                // This just has it pull everything.  Needs refactoring to pull in batches.
                var task = search.GetRemainingAsync();

                timeout = DateTime.Now.AddMilliseconds(TIMEOUT_IN_MS);
                while (!task.IsCompleted && DateTime.Now < timeout) { }

                if (task.Status != TaskStatus.RanToCompletion)
                {
                    _logger.LogError("Failed to retrieve activity list - status:  {0}, exception: {1}", task.Status.ToString(), task.Exception == null ? "" : task.Exception.InnerException.Message);
                    return new List<Activity>();
                }

                _logger.LogInformation("Finished pulling down results.");

                return task.Result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while pulling activity list from DynamoDB: {0}", ex.Message);
            }
            
            return new List<Activity>();
        }
    }
}