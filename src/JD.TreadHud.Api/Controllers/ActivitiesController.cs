using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;
using JD.TreadHud.Domain.Managers;

namespace JD.TreadHud.Api.Controllers
{
    [Route("api/[controller]")]
    public class ActivitiesController : Controller
    {
        private readonly ILogger<ActivitiesController> _logger;
        private readonly IActivityManager _activityManager;

        public ActivitiesController(ILogger<ActivitiesController> logger, IActivityManager activityManager)
        {
            _logger = logger;
            _activityManager = activityManager;
        }

        // GET: api/activity
        [HttpGet]
        public IEnumerable<string> Get()
        {
            JD.TreadHud.Domain.Models.Activity newActivity = new JD.TreadHud.Domain.Models.Activity();
            newActivity.Id = Guid.NewGuid();
            newActivity.StartDate = DateTime.Now.AddMinutes(-20);
            newActivity.EndDate = DateTime.Now;
            _activityManager.AddActivity(newActivity);

            var activities = _activityManager.GetAllActivities();
            if (activities != null && activities.Count > 0)
            {
                return activities.Select(a => a.Id.ToString());
            }
            return new string[] { "value1", "value2" };
        }

        // GET api/activity/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/activity
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/activity/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/activity/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
