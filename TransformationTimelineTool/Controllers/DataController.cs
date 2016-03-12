using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TransformationTimelineTool.DAL;
using TransformationTimelineTool.Helpers;
using TransformationTimelineTool.Models;

namespace TransformationTimelineTool.Controllers
{
    [RoutePrefix("data")]
    public class DataController : Controller
    {
        private TimelineContext db = new TimelineContext();

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

        [HttpPost]
        [Route("regions")]
        public async Task<ActionResult> ReturnRegions()
        {
            List<Region> regions = await db.Regions.ToListAsync();
            regions = regions.OrderBy(r => r.Name).ToList();
            List<object> result = new List<object>();
            foreach (var r in regions)
            {
                result.Add(new
                {
                    ID = r.ID,
                    NameShort = r.NameShort,
                    NameE = r.NameE,
                    NameF = r.NameF
                });
            }
            return Json(result);
        }

        [HttpPost]
        [Route("branches")]
        public async Task<ActionResult> ReturnBranches()
        {
            List<Branch> branches = await db.Branches.ToListAsync();
            branches = branches.OrderBy(b => b.Name).ToList();
            List<object> result = new List<object>();
            foreach (var b in branches)
            {
                result.Add(new
                {
                    ID = b.ID,
                    NameShort = b.NameShort,
                    NameE = b.NameE,
                    NameF = b.NameF
                });
            }
            return Json(result);
        }

        [HttpPost]
        [Route("quarters")]
        public async Task<ActionResult> ReturnQuarters()
        {
            List<Initiative> initiatives = await db.Initiatives.ToListAsync();
            List<object> result = new List<object>();
            result.Add(new
            {
                Start = initiatives.Min(i => i.StartDate).ToShortDateString(),
                End = initiatives.Max(i => i.EndDate).ToShortDateString()
            });
            return Json(result);
        }

        [HttpPost]
        [Route("initiatives")]
        public async Task<ActionResult> ReturnInitiatives()
        {
            List<Initiative> initiatives = await db.Initiatives.ToListAsync();
            List<object> result = new List<object>();
            foreach (Initiative i in initiatives)
            {
                result.Add(new
                {
                    ID = i.ID,
                    StartDate = i.StartDate,
                    EndDate = i.EndDate,
                    NameE = i.NameE,
                    NameF = i.NameF,
                    DescE = i.DescriptionE,
                    DescF = i.DescriptionF,
                    Timeline = i.Timeline
                });
            }
            return Json(result);
        }

        [HttpPost]
        [Route("initiative")]
        public async Task<ActionResult> ReturnInitiative(int Id)
        {
            var jsonInitiatives = new List<object>();
            List<Initiative> initiatives = await db.Initiatives.Where(i => (i.ID == Id)).ToListAsync();
            foreach (var init in initiatives)
            {
                List<object> jsonEvents = new List<object>();
                List<object> jsonImpacts = new List<object>();
                var events = init.Events.OrderBy(e => e.PublishedEdit.DisplayDate);

                foreach (var e in events)
                {
                    jsonEvents.Add(new
                    {
                        ID = e.ID,
                        Type = e.PublishedEdit.Type.ToString(),
                        Date = e.PublishedEdit.DisplayDate.ToShortDateString(),
                        Branches = e.Branches.Select(b => b.ID),
                        Regions = e.Regions.Select(r => r.ID),
                        TextE = e.PublishedEdit.TextE,
                        HoverE = e.PublishedEdit.HoverE,
                        TextF = e.PublishedEdit.TextF,
                        HoverF = e.PublishedEdit.HoverF,
                        Show = e.Show
                    });
                }
                foreach (var impact in init.Impacts)
                {
                    jsonImpacts.Add(new
                    {
                        Level = impact.Level,
                        Branches = impact.Branches.Select(b => b.ID),
                        Regions = impact.Regions.Select(r => r.ID)
                    });
                }

                jsonInitiatives.Add(new
                {
                    ID = init.ID,
                    NameE = init.NameE,
                    NameF = init.NameF,
                    DescriptionE = init.DescriptionE,
                    DescriptionF = init.DescriptionF,
                    StartDate = init.StartDate.ToShortDateString(),
                    EndDate = init.EndDate.ToShortDateString(),
                    Impacts = jsonImpacts,
                    Events = jsonEvents,
                    Timeline = init.Timeline
                });
            }
            return Json(jsonInitiatives);
        }

        private List<DateTime> GetDateRage(int quarter, int year)
        {
            int firstMonth = 0;
            switch (quarter)
            {
                case 1:
                    firstMonth = 4;
                    break;
                case 2:
                    firstMonth = 7;
                    break;
                case 3:
                    firstMonth = 10;
                    break;
                case 4:
                    firstMonth = 1;
                    break;
                default:
                    break;
            }
            DateTime firstDayOfQuarter = new DateTime(year, firstMonth, 1);
            DateTime lastDayOfQuarter = firstDayOfQuarter.AddMonths(3).AddDays(-1);
            return new List<DateTime> { firstDayOfQuarter, lastDayOfQuarter };
        }

        [HttpPost]
        [Route("initiative-quarterly")]
        public async Task<ActionResult> ReturnQuarterlyInitiatives(int quarter, int year, string culture)
        {
            var jsonInitiatives = new List<object>();
            List<Initiative> initiatives = await db.Initiatives.ToListAsync();
            if (culture == "fr-ca")
            {
                initiatives = initiatives.OrderBy(i => i.NameF).ToList();
            } else
            {
                initiatives = initiatives.OrderBy(i => i.NameE).ToList();
            }
            List<DateTime> dateRange = GetDateRage(quarter, year);
            foreach (var init in initiatives)
            {
                var jsonEvents = new List<object>();
                var jsonImpacts = new List<object>();
                var events = init.Events
                    .Where(e => (e.PublishedEdit.DisplayDate >= dateRange[0] && e.PublishedEdit.DisplayDate <= dateRange[1]))
                    .OrderBy(e => e.PublishedEdit.DisplayDate);

                foreach (var e in events)
                {
                    jsonEvents.Add(new
                    {
                        ID = e.ID,
                        Type = e.PublishedEdit.Type.ToString(),
                        Date = e.PublishedEdit.DisplayDate.ToShortDateString(),
                        Branches = e.Branches.Select(b => b.ID),
                        Regions = e.Regions.Select(r => r.ID),
                        TextE = e.PublishedEdit.TextE,
                        HoverE = e.PublishedEdit.HoverE,
                        TextF = e.PublishedEdit.TextF,
                        HoverF = e.PublishedEdit.HoverF,
                        Show = e.Show
                    });
                }
                foreach (var impact in init.Impacts)
                {
                    jsonImpacts.Add(new
                    {
                        Level = impact.Level,
                        Branches = impact.Branches.Select(b => b.ID),
                        Regions = impact.Regions.Select(r => r.ID)
                    });
                }

                jsonInitiatives.Add(new
                {
                    ID = init.ID,
                    NameE = init.NameE,
                    NameF = init.NameF,
                    DescriptionE = init.DescriptionE,
                    DescriptionF = init.DescriptionF,
                    StartDate = init.StartDate.ToShortDateString(),
                    EndDate = init.EndDate.ToShortDateString(),
                    Impacts = jsonImpacts,
                    Events = jsonEvents,
                    Timeline = init.Timeline
                });
            }
            return Json(jsonInitiatives);
        }
    }
}