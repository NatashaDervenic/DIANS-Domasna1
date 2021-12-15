using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using webSchool.Data;
using webSchool.Models;
using System.Security.Claims;

namespace webSchool.Controllers
{
    public class SchoolsController : Controller
    {

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly ApplicationDbContext database;

        public SchoolsController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            database = context;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: Schools
        public async Task<IActionResult> Index()
        {
            //populate();
            string loggedUserId = admin_permissions();
            var user = database.loggedUserRoles.Where(x => x.UserId.Equals(loggedUserId)).ToList().FirstOrDefault();
            if (user == null) ViewBag.userRole = "User";
            else
            {
                if (user.Role == "Admin") ViewBag.userRole = "Admin";
                else ViewBag.userRole = "User";
            }
            return View(await database.schools.ToListAsync());
        }

        // GET: Schools/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            string loggedUserId = admin_permissions();
            var user = database.loggedUserRoles.Where(x => x.UserId.Equals(loggedUserId)).ToList().FirstOrDefault();
            if (user == null) ViewBag.userRole = "User";
            else
            {
                if (user.Role == "Admin") ViewBag.userRole = "Admin";
                else ViewBag.userRole = "User";
            }

            if (id == null)
            {
                return NotFound();
            }

            var school = await database.schools
                .FirstOrDefaultAsync(m => m.Id == id);
            if (school == null)
            {
                return NotFound();
            }

            return View(school);
        }

        // GET: Schools/Create
        public IActionResult Create()
        {
            string loggedUserId = admin_permissions();
            if (loggedUserId == null)
            {
                return BadRequest(); //return View("U should be logged in for this action")
            }
            var user = database.loggedUserRoles.Where(x => x.UserId.Equals(loggedUserId)).ToList().FirstOrDefault();
            if (user == null) return BadRequest();
            if (user.Role == "Admin")
            return View();
            return BadRequest(); //obicen korisnik e nema pravo da dodava ucilista
        }

        // POST: Schools/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,name,imageUrl,city,street,modules")] School school)
        {
            if (ModelState.IsValid)
            {
                database.Add(school);
                await database.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(school);
        }

        // GET: Schools/Edit/5

        public async Task<IActionResult> Edit(int? id)
        {
            string loggedUserId = admin_permissions();
            if (loggedUserId == null)
            {
                return View("error"); //return View("U should be logged in for this action")
            }
            var user = database.loggedUserRoles.Where(x => x.UserId.Equals(loggedUserId)).ToList().FirstOrDefault();
            if (user == null) return BadRequest();
            if (user.Role == "Admin")
                return View();
            return BadRequest(); //obicen korisnik e nema pravo da izmenuva info za ucilista

            if (id == null)
            {
                return NotFound();
            }

            var school = await database.schools.FindAsync(id);
            if (school == null)
            {
                return NotFound();
            }
            return View(school);
        }

        // POST: Schools/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,name,imageUrl,city,street,modules")] School school)
        {
            if (id != school.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    database.Update(school);
                    await database.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SchoolExists(school.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(school);
        }
        //This activity is programmed but shoulnd't be used
        // GET: Schools/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            string loggedUserId = admin_permissions();
            if (loggedUserId == null)
            {
                return BadRequest(); //return View("U should be logged in for this action")
            }
            var user = database.loggedUserRoles.Where(x => x.UserId.Equals(loggedUserId)).ToList().FirstOrDefault();
            if (user == null) return BadRequest();
            if (user.Role == "Admin")
                return View();
            return BadRequest(); //obicen korisnik e nema pravo da brisi ucilista

            if (id == null)
            {
                return NotFound();
            }

            var school = await database.schools
                .FirstOrDefaultAsync(m => m.Id == id);
            if (school == null)
            {
                return NotFound();
            }

            return View(school);
        }

        // POST: Schools/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var school = await database.schools.FindAsync(id);
            database.schools.Remove(school);
            await database.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SchoolExists(int id)
        {
            return database.schools.Any(e => e.Id == id);
        }
        //funkcija koja cita podatoci od .csv file i zacuvuva vo baza
        public void populate()
        {
           using (var reader = new StreamReader(@"C:\Users\Bojan\Documents\FINKI\tretaGodina\zimskiSemestar\DIANS\lab1\sredniUcilistaMkd.txt", System.Text.Encoding.UTF8))
            {
                var line = reader.ReadLine();
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    var values = line.Split(',');
                    School school = new School(values[1], values[2], values[3], values[4], Double.Parse(values[5]), Double.Parse(values[6]),
                        values[7], values[8], values[9], values[10], values[11], Int32.Parse(values[12]));
                    database.schools.Add(school);
                    //TryUpdateModelAsync(school);
                    database.SaveChanges();
                }
            } 
           
        }

        public string admin_permissions()
        {
            if (_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) == null) return null;
            return _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            //tuka logiraj ili return napraj na pocetna strana vo zavisnost od to dali e null etc.
            
        }

        public static int counter = 0;
        public ActionResult searchSchoolGet(string q)
        {
            var result = database.schools.Where(x => x.city.Equals(q)).ToList();
            if (result.Count == 0)
            {
                result = database.schools.Where(x => x.name.Equals(q)).ToList();
            }
            if (result.Count == 0) ViewBag.valid = true;
            counter = counter + 1;
            ViewBag.counter = counter;
            //ViewBag.user = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return View(result);
            
        }
    }
}
