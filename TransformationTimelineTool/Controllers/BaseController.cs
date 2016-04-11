using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TransformationTimelineTool.Helpers;

namespace TransformationTimelineTool.Controllers
{
    public class BaseController : Controller
    {
        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            string cultureName = RouteData.Values["culture"] as string;
            if(Request.QueryString["lang"] == "fra")
            {
                cultureName = "fr";
            }
            else
            {
                cultureName = "en";
            }

            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureName);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

            return base.BeginExecuteCore(callback, state);
        }

        public void ManageCache()
        {
            Task updateCache = Task.Run(() =>
            {
                CacheLayer.EmptyTimelineCache();
                new DataController().ReturnAllInitiatives("en");
                new DataController().ReturnAllInitiatives("fr");
            });
        }

        //protected override ViewResult View(string viewName, string masterName, object model)
        //{
        //    viewName += "-fr";
        //    return base.View(viewName, masterName, model);
        //}
    }
}