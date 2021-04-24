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
            List<string> activitynames = _context.Activities.Select(p => p.name).ToList();

            ViewBag.statedict = dict;
            ViewBag.activitynames = activitynames;

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
            ViewBag.activitynames = an;
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
                    Park npk = new Park()
                    {
                        ID = DateTime.Now.ToString("yyyyMMddHHmmssffff"),
                        fullName = pk.fullName,
                        parkCode = pk.parkCode,
                        description = pk.description,
                        url = pk.url
                    };
                    _context.Parks.Add(npk);
                    if (pk.activitynames != null)
                    {
                        foreach (string str in pk.activitynames)
                        {
                            Activity a = _context.Activities.Where(p => p.name == str).FirstOrDefault();
                            _context.ParkActivities.Add(new ParkActivity()
                            {
                                park = npk,
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
                                park = npk,
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
            ViewBag.activitynames = an;
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
            Park updtPark = _context.Parks.Where(p => p.ID == id).FirstOrDefault();
            List<string> park_acct = _context.ParkActivities.Where(p => p.park == updtPark).Select(p => p.activity.name).ToList();

            AddNewPark updtnewpk = new AddNewPark()
            {
                fullName = updtPark.fullName,
                description = updtPark.description,
                activitynames = park_acct
            };

            List<string> activitynames = await _context.Activities.Select(p => p.name).ToListAsync();

            ViewBag.activitynames = activitynames;

            return View(updtnewpk);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("url,fullName,parkCode,description,statenames,activitynames")] AddNewPark updatedpk)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Park updtpark = _context.Parks
                        .Include(p => p.activities)
                        .Where(p => p.ID == id)
                        .FirstOrDefault();

                    updtpark.description = updatedpk.description;
                    updtpark.activities.Clear();

                    foreach (string actname in updatedpk.activitynames)
                    {
                        Activity a = _context.Activities.Where(a => a.name == actname).FirstOrDefault();
                        ParkActivity pa = new ParkActivity()
                        {
                            park = updtpark,
                            activity = a
                        };
                        updtpark.activities.Add(pa);
                    }
                    _context.Update(updtpark);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Error Occured");
            }
            List<string> activitynames = await _context.Activities.Select(p => p.name).ToListAsync();

            ViewBag.activitynames = activitynames;
            return View(updatedpk);
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
//CRUD - Delete Ends

//Chart JS Starts 
 
// Chart JS Ends
    }
}