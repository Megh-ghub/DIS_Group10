using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DIS_Group10.Models;
using DIS_Group10.Data;
using Activity = DIS_Group10.Models.Activity;

namespace DIS_Group10.Controllers
{
    public class HomeController : Controller
    {

        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Aboutus()
        {
            return View();
        }

// Explore Park Data Start 
        public async Task<IActionResult> ExplorePark(string statename, string activityname, string parkname)
        {
            statename = (statename == null) ? "" : statename;
            activityname = (activityname == null) ? "" : activityname;
            parkname = (parkname == null) ? "" : parkname;

            List<Park> plist = new List<Park>();
            if (statename == "" && activityname == "" && parkname == "")
            {
                plist = await _context.Parks.Include(p => p.states).ToListAsync();
            }
            else
            {
                plist = await _context.Parks
                            .Include(p => p.activities)
                            .Include(p => p.states)
                            .Where(p => p.activities.Any(s => s.activity.name.Contains(activityname)))
                            .Where(p => p.states.Any(s => s.state.ID.Contains(statename)))
                            .Where(p => p.fullName.Contains(parkname))
                            .ToListAsync();
            }

            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (State i in _context.States)
            {
                dict.Add(i.ID, i.name);
            }
            List<string> anames = _context.Activities.Select(p => p.name).ToList();

            ViewBag.statedict = dict;
            ViewBag.anames = anames;

            return View(plist);
        }
// Explore Park Data Ends

        public IActionResult Model()
        {
            return View();
        }

        public IActionResult GetInTouch()
        {
            return View();
        }

//CRUD - Create Starts 
        public async Task<IActionResult> Create()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (State i in _context.States)
            {
                dict.Add(i.ID, i.name);
            }
            List<string> an = await _context.Activities.Select(p => p.name).ToListAsync();
            ViewBag.anames = an;
            ViewBag.statedict = dict;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("url,fullName,parkCode,description,statenames,activitynames")] AddNewPark pk)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Park newpark = new Park()
                    {
                        ID = DateTime.Now.ToString("yyyyMMddHHmmssffff"),
                        fullName = pk.fullName,
                        parkCode = pk.parkCode,
                        description = pk.description,
                        url = pk.url
                    };
                    _context.Parks.Add(newpark);
                    if (pk.activitynames != null)
                    {
                        foreach (string str in pk.activitynames)
                        {
                            Activity a = _context.Activities.Where(p => p.name == str).FirstOrDefault();
                            _context.ParkActivities.Add(new ParkActivity()
                            {
                                park = newpark,
                                activity = a
                            });
                        }
                    }
                    if (pk.statenames != null)
                    {
                        foreach (string str in pk.statenames)
                        {
                            State s = _context.States.Where(p => p.ID == str).FirstOrDefault();
                            _context.StateParks.Add(new StatePark()
                            {
                                park = newpark,
                                state = s
                            });
                        }
                    }
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Error Occured");
            }

            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (State i in _context.States)
            {
                dict.Add(i.ID, i.name);
            }
            List<string> an = await _context.Activities.Select(p => p.name).ToListAsync();
            ViewBag.anames = an;
            ViewBag.statedict = dict;
            return View(pk);
        }
//CRUD - Create Ends

//CRUD - Read Starts 
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var p = await _context.Parks
                .Include(s => s.activities)
                    .ThenInclude(e => e.activity)
                .Include(s => s.states)
                    .ThenInclude(e => e.state)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID.Equals(id));

            if (p == null)
            {
                return NotFound();
            }
            ViewData["Title"] = "Details: " + p.parkCode;
            return View(p);
        }
//CRUD - Read Ends

//CRUD - Update Starts 
        public async Task<IActionResult> Edit(string id)
        {
            Park parkToUpdate = _context.Parks.Where(p => p.ID == id).FirstOrDefault();
            List<string> park_a = _context.ParkActivities.Where(p => p.park == parkToUpdate).Select(p => p.activity.name).ToList();
            List<string> park_s = _context.StateParks.Where(p => p.park == parkToUpdate).Select(p => p.state.ID).ToList();

            AddNewPark cp_edit = new AddNewPark()
            {
                ID = parkToUpdate.ID,
                fullName = parkToUpdate.fullName,
                parkCode = parkToUpdate.parkCode,
                url = parkToUpdate.url,
                description = parkToUpdate.description,
                activitynames = park_a,
                statenames = park_s
            };

            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (State i in _context.States)
            {
                dict.Add(i.ID, i.name);
            }
            List<string> anames = await _context.Activities.Select(p => p.name).ToListAsync();

            ViewBag.statedict = dict;
            ViewBag.anames = anames;

            return View(cp_edit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("url,fullName,parkCode,description,statenames,activitynames")] AddNewPark modifiedp)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Park ptobeupdated = _context.Parks
                        .Include(p => p.activities)
                        .Include(p => p.states)
                        .Where(p => p.ID == id)
                        .FirstOrDefault();

                    ptobeupdated.url = modifiedp.url;
                    ptobeupdated.fullName = modifiedp.fullName;
                    ptobeupdated.parkCode = modifiedp.parkCode;
                    ptobeupdated.description = modifiedp.description;

                    ptobeupdated.activities.Clear();

                    foreach (string aname in modifiedp.activitynames)
                    {
                        Activity a = _context.Activities.Where(a => a.name == aname).FirstOrDefault();
                        ParkActivity pa = new ParkActivity()
                        {
                            park = ptobeupdated,
                            activity = a
                        };
                        ptobeupdated.activities.Add(pa);
                    }

                    ptobeupdated.states.Clear();

                    foreach (string sname in modifiedp.statenames)
                    {
                        State s = _context.States.Where(s => s.ID == sname).FirstOrDefault();
                        StatePark sp = new StatePark()
                        {
                            park = ptobeupdated,
                            state = s
                        };
                        ptobeupdated.states.Add(sp);
                    }
                    _context.Update(ptobeupdated);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Error Occured");
            }
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (State i in _context.States)
            {
                dict.Add(i.ID, i.name);
            }
            List<string> anames = await _context.Activities.Select(p => p.name).ToListAsync();

            ViewBag.statedict = dict;
            ViewBag.anames = anames;
            return View(modifiedp);
        }
//CRUD - Update Ends

//CRUD - Delete Starts 
        public async Task<IActionResult> Delete(string id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var p = await _context.Parks
                .Include(s => s.activities)
                    .ThenInclude(e => e.activity)
                .Include(s => s.states)
                    .ThenInclude(e => e.state)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID.Equals(id));

            if (p == null)
            {
                return NotFound();
            }
            ViewData["Title"] = "Delete: " + p.parkCode;

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Error Occured";
            }

            return View(p);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            Park deletepark = await _context.Parks
                .Include(p => p.activities)
                .Include(p => p.states)
                .Where(p => p.ID == id)
                .FirstOrDefaultAsync();

            if (deletepark == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Parks.Remove(deletepark);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }
//CRUD - Delete Starts

        //Chart JS Starts 
        public ActionResult Chart()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (State i in _context.States)
            {

            }
                //    {
                //        dict.Add(i.ID, i.name);
                //    }

                return View();
        }
        //public IActionResult Chart()
        //{
        //    Dictionary<string, string> dict = new Dictionary<string, string>();
        //    foreach (Activity i in _context.Activities)
        //    {
        //        dict.Add(i.ID, i.name);
        //    }
        //    ViewBag.adict = dict;
        //    return View();
        //}

        [HttpPost]
        public JsonResult chart(string id)
        {
            List<object> charttable = new List<object>();
            List<string> statelist = _context.States.Select(s => s.ID).ToList();
            List<int> pcount = new List<int>();
            string aname = _context.Activities.Where(a => a.ID == id).Select(a => a.name).FirstOrDefault();
            foreach (string s in statelist)
            {
                int parkcount = 0;
                if (id == "all")
                {
                    parkcount = _context.StateParks
                    .Where(p => p.state.ID == s)
                    .Select(p => p.park)
                    .Count();
                }
                else
                {
                    parkcount = _context.StateParks
                                .Where(p => p.state.ID == s)
                                .Select(p => p.park)
                                .Where(p => p.activities.Any(s => s.activity.ID == id))
                                .Count();
                }
                pcount.Add(parkcount);
            }
            charttable.Add(statelist);
            charttable.Add(pcount);
            charttable.Add(aname);
            return Json(charttable);
        }
        //chart js ends

    }
}
