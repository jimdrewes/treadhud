using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using JD.TreadHud.Domain.Models;
using JD.TreadHud.Domain.DataAccess;

namespace JD.TreadHud.Domain.Managers
{
    public class ActivityManager : IActivityManager
    {
        private readonly ILogger<ActivityManager> _logger;
        private readonly IActivityDao _activityDao;

        public ActivityManager(ILogger<ActivityManager> logger, IActivityDao activityDao)
        {
            _logger = logger;
            _activityDao = activityDao;
        }
		
        public bool AddActivity(Activity activity)
        {
            _logger.LogInformation("Adding activity {@Activity}", activity);
            return _activityDao.AddActivity(activity);
        }

        public Activity GetActivity(Guid id)
        {
            _logger.LogInformation("Getting activity with ID {0}", id.ToString());
            return null;
        }

        public IList<Activity> GetAllActivities()
        {
            _logger.LogInformation("Getting all activities.");
            return _activityDao.GetAllActivities();
        }
    }
}