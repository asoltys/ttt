using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TransformationTimelineTool.Controllers
{
    [RoutePrefix("data")]
    public class DataController : Controller
    {

        [Route("initiatives-eng")]
        public ActionResult ReturnMockDataEng()
        {
            var path = "~/Scripts/MockData/data-eng.json";
            return File(path, "application/json");
        }

        [Route("initiatives-fra")]
        public ActionResult ReturnMockDataFra()
        {
            var path = "~/Scripts/MockData/data-fra.json";
            return File(path, "application/json");
        }

        [Route("regions")]
        public ActionResult ReturnRegions()
        {
            var path = "~/Scripts/MockData/regions.json";
            return File(path, "application/json");
        }

        [Route("branches")]
        public ActionResult ReturnBranches()
        {
            var path = "~/Scripts/MockData/branches.json";
            return File(path, "application/json");
        }
    }
}