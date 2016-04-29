using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using TransformationTimelineTool.DAL;
using TransformationTimelineTool.Models;
using TransformationTimelineTool.Helpers;
using System.Configuration;
using System.Data.SqlClient;
using Quartz;
using Quartz.Impl;

namespace TransformationTimelineTool
{
    public class MvcApplication : System.Web.HttpApplication
    {

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //SqlDependency.Start(ConfigurationManager.ConnectionStrings["TimelineContext"].ConnectionString);

            // Construct a scheduler factory & get a scheduler
            ISchedulerFactory ScheduleFactory = new StdSchedulerFactory();
            IScheduler Schedule = ScheduleFactory.GetScheduler();
            Schedule.Start();

            // Define the job and tie it to ChangeNotify class
            IJobDetail Job = JobBuilder.Create<ChangeNotify>()
                .WithIdentity("ChangeNotification", "Group")
                .Build();

            // Trigger the job to run now for testing
            /*
            ITrigger Trigger = TriggerBuilder.Create()
                .WithIdentity("ChangeNotificationTrigger", "Group")
                .Build();
                */
            ITrigger Trigger = TriggerBuilder.Create()
                .WithIdentity("ChangeNotificationTrigger", "Group")
                .StartNow()
                .WithSchedule(CronScheduleBuilder.WeeklyOnDayAndHourAndMinute(DayOfWeek.Monday, 3,0))
                .Build();
            Schedule.ScheduleJob(Job, Trigger);
        }

        protected void Application_PostAuthenticateRequest(object sender, EventArgs args)
        {
            var user = HttpContext.Current.User.Identity;
            GenericPrincipal principal = new GenericPrincipal(user, Utils.GetUserRoles().ToArray());
            Thread.CurrentPrincipal = HttpContext.Current.User = principal;
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            if (Response.StatusCode == 401)
            {
                Response.ClearContent();
                Response.WriteFile("~/Static/NotAuthorized.html");
                Response.ContentType = "text/html";
            }
        }
    }
}
