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

        [Route("initiative-eng")]
        public ActionResult ReturnMockDataEng()
        {
            var path = "~/Scripts/MockData/data-eng.json";
            return File(path, "application/json");
        }

        [Route("initiative-fra")]
        public ActionResult ReturnMockDataFra()
        {
            var path = "~/Scripts/MockData/data-fra.json";
            return File(path, "application/json");
        }
    }
}