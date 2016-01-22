﻿using Microsoft.AspNet.Identity;
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
        }

        protected void Application_PostAuthenticateRequest(object sender, EventArgs args)
        {
            var user = HttpContext.Current.User.Identity;
            GenericPrincipal principal = new GenericPrincipal(user, Utils.GetUserRoles(user.Name));
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
