﻿using Newtonsoft.Json;
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
        private string dateFormat = "MM/dd/yy";

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
                Start = initiatives.Min(i => i.StartDate).ToString(dateFormat),
                End = initiatives.Max(i => i.EndDate).ToString(dateFormat)
            });
            return Json(result);
        }

        [HttpPost]
        [Route("initiatives")]
        public async Task<ActionResult> ReturnInitiatives(string culture)
        {
            List<Initiative> initiatives = await db.Initiatives.ToListAsync();
            initiatives = culture == "fr-ca" ?
                initiatives.OrderBy(i => i.NameF).ToList() : initiatives.OrderBy(i => i.NameE).ToList();
            var initiativeBlocks = new List<object>();
            var timelines = initiatives.Select(i => i.Timeline).Distinct();
            foreach (var timeline in timelines)
            {
                var initiativeBlock = initiatives.Where(i => i.Timeline == timeline).ToList();
                initiativeBlocks.Add(new {
                    NameE = timeline,
                    NameF = timeline,
                    Data = populateJSON(initiativeBlock)
                });
            }
            return Json(initiativeBlocks);
        }

        private List<DateTime> GetDateRage(int quarter, int year)
        {
            int firstMonth = 0;
            int yearOffset = 0;
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
                    yearOffset++;
                    break;
                default:
                    break;
            }
            DateTime firstDayOfQuarter = new DateTime(year + yearOffset, firstMonth, 1);
            DateTime lastDayOfQuarter = firstDayOfQuarter.AddMonths(3).AddDays(-1);
            return new List<DateTime> { firstDayOfQuarter, lastDayOfQuarter };
        }

        [HttpPost]
        [Route("initiative-quarterly")]
        public async Task<ActionResult> ReturnQuarterlyInitiatives(int quarter, int year, string culture)
        {
            List<Initiative> initiatives = await db.Initiatives.ToListAsync();
            initiatives = culture == "fr-ca" ?
                initiatives.OrderBy(i => i.NameF).ToList() : initiatives.OrderBy(i => i.NameE).ToList();
            List<DateTime> dateRange = GetDateRage(quarter, year);
            var initiativeBlocks = new List<object>();
            var timelines = initiatives.Select(i => i.Timeline).Distinct();
            foreach (var timeline in timelines)
            {
                var initiativeBlock = initiatives.Where(i => i.Timeline == timeline).ToList();
                var quarterlyData = populateQuarterlyJSON(dateRange, initiativeBlock);
                var unusedInits = getUnusedInitiatives(dateRange, initiativeBlock);
                if (quarterlyData.Count > 0)
                {
                    initiativeBlocks.Add(new
                    {
                        NameE = timeline,
                        NameF = timeline,
                        StartDate = dateRange[0].ToString(dateFormat),
                        EndDate = dateRange[1].ToString(dateFormat),
                        StartMonth = dateRange[0].Month,
                        EndMonth = dateRange[1].Month,
                        Data = quarterlyData,
                        Unused = unusedInits
                    });
                }
            }
            return Json(initiativeBlocks);
        }

        private List<object> populateJSON(List<Initiative> initiatives)
        {
            var json = new List<object>();
            foreach (var init in initiatives)
            {
                var jsonEvents = new List<object>();
                var jsonImpacts = new List<object>();
                var events = init.Events
                    .OrderBy(e => e.PublishedEdit.DisplayDate);

                foreach (var e in events)
                {
                    jsonEvents.Add(new
                    {
                        ID = e.ID,
                        Type = e.PublishedEdit.Type.ToString(),
                        Date = e.PublishedEdit.DisplayDate.ToString(dateFormat),
                        TextE = e.PublishedEdit.TextE,
                        HoverE = e.PublishedEdit.HoverE,
                        TextF = e.PublishedEdit.TextF,
                        HoverF = e.PublishedEdit.HoverF,
                        Show = e.Show
                    });
                }
                json.Add(new
                {
                    ID = init.ID,
                    NameE = init.NameE,
                    NameF = init.NameF,
                    DescriptionE = init.DescriptionE,
                    DescriptionF = init.DescriptionF,
                    StartDate = init.StartDate.ToString(dateFormat),
                    EndDate = init.EndDate.ToString(dateFormat),
                    Events = jsonEvents
                });
            }
            return json;
        }

        private class EventInformation
        {
            public int ID { get; set; }
            public string InitiativeNameE { get; set; }
            public string InitiativeNameF { get; set; }
            public string Type { get; set; }
            public string Date { get; set; }
            public string TextE { get; set; }
            public string HoverE { get; set; }
            public string TextF { get; set; }
            public string HoverF { get; set; }
            public Dictionary<string, int> Control { get; set; }
        }

        private List<EventInformation> populateQuarterlyJSON(List<DateTime> dateRange, List<Initiative> initiatives)
        {

            var jsonEvents = new List<EventInformation>();
            foreach (var init in initiatives)
            {
                var events = init.Events
                    .Where(e => (e.PublishedEdit.DisplayDate >= dateRange[0] && e.PublishedEdit.DisplayDate <= dateRange[1]))
                    .OrderBy(e => e.PublishedEdit.DisplayDate);

                foreach (var e in events)
                {
                    var controlDictionary = new Dictionary<string, int>();
                    var branches = e.Branches.Select(b => b.ID);
                    var regions = e.Regions.Select(r => r.ID);
                    foreach (var r in regions)
                    {
                        foreach (var b in branches)
                        {
                            controlDictionary.Add(r + "," + b, -1);
                        }
                    }
                    jsonEvents.Add(new EventInformation
                    {
                        ID = e.ID,
                        InitiativeNameE = init.NameE,
                        InitiativeNameF = init.NameF,
                        Type = e.PublishedEdit.Type.ToString(),
                        Date = e.PublishedEdit.DisplayDate.ToString(dateFormat),
                        TextE = e.PublishedEdit.TextE,
                        HoverE = e.PublishedEdit.HoverE,
                        TextF = e.PublishedEdit.TextF,
                        HoverF = e.PublishedEdit.HoverF,
                        Control = controlDictionary
                    });
                }
            }
            return jsonEvents.OrderBy(e => e.Date).ToList();
        }
        
        private List<object> getUnusedInitiatives(List<DateTime> dateRange, List<Initiative> initiatives)
        {
            var unused = new List<object>();
            foreach (var init in initiatives)
            {
                var events = init.Events
                    .Where(e => (e.PublishedEdit.DisplayDate >= dateRange[0] && e.PublishedEdit.DisplayDate <= dateRange[1]))
                    .ToList();
                if (events.Count > 0)
                    continue;
                unused.Add(new {
                    NameE = init.NameE,
                    NameF = init.NameF
                });
            }
            return unused;
        }
    }

   
}