using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using JD.TreadHud.Domain.Models;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using System;

namespace JD.TreadHud.Domain.DataAccess
{
    public class DynamoDbActivityDao : IActivityDao
    {
        private static AmazonDynamoDBClient _client;
        private static DynamoDBContext _context;
        private readonly ILogger<DynamoDbActivityDao> _logger;
        
        public DynamoDbActivityDao(ILogger<DynamoDbActivityDao> logger)
        {
            _logger = logger;
            Amazon.RegionEndpoint endpoint = Amazon.RegionEndpoint.USEast1;
            _client = new AmazonDynamoDBClient("AKIAIAHMHGG3DLERCRFQ", "2UbdKdnsz3wvGrQ84AvR5DyKXGOHNgP/xWKc0UN8", endpoint);
            _context = new DynamoDBContext(_client);
            _logger.LogInformation("Finished connecting to Amazon Client.");
        }
        
        public bool AddActivity(Activity activity)
        {
            return true;
        }

        public IList<Activity> GetAllActivities()
        {
            try
            {
                _logger.LogInformation("Adding activity...");
                Guid newId = Guid.NewGuid();
                Activity activity = new Activity
                {
                    Id = newId,
                    StartDate = DateTime.Now.AddMinutes(-20),
                    EndDate = DateTime.Now,
                    ActivityId = newId.ToString()
                };

                var task = _context.SaveAsync(activity);
                while (!task.IsCompleted) { }

                _logger.LogInformation("info - status:  {0}, exception: {1}", task.Status.ToString(), task.Exception == null ? "" : task.Exception.InnerException.Message);
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