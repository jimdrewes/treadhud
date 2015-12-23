using System;
using System.Collections.Generic;
using JD.TreadHud.Domain.Models;

namespace JD.TreadHud.Domain.DataAccess
{
	public interface IActivityDao
	{
        IList<Activity> GetAllActivities();
        bool AddActivity(Activity activity);
    }
}