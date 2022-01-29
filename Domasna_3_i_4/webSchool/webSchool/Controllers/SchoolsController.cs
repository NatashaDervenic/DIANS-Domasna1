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

        public Boolean checkRole(Roles user, string role)
        {
            if (user.Role == role) return true;
            else return false;
        }

        //1
        // GET: Schools
        public async Task<IActionResult> Index()
        {
            //loggedUser() e funckija koja go vrakja id-to na logiraniot korisnik. Dokolku nikoj ne e logiran vrakja null koj podocna se sreduva
            string loggedUserId = loggedUser();
            var user = database.loggedUserRoles.Where(x => x.UserId.Equals(loggedUserId)).ToList().FirstOrDefault();
            if (user == null) ViewBag.userRole = "User";
            else
            {
                //if (user.Role == "Admin") 
                //else ViewBag.userRole = "User";
                if (checkRole(user, "Admin")) ViewBag.userRole = "Admin";
                else ViewBag.username = "User";
            }
            return View(await database.schools.ToListAsync());
        }

        // GET: Schools/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            string loggedUserId = loggedUser();
            var user = database.loggedUserRoles.Where(x => x.UserId.Equals(loggedUserId)).ToList().FirstOrDefault();
            if (user == null) ViewBag.userRole = "User";
            else
            {
                //if (user.Role == "Admin") ViewBag.userRole = "Admin";
                //else ViewBag.userRole = "User";
                if (checkRole(user, "Admin")) ViewBag.userRole = "Admin";
                else ViewBag.username = "User";
            }

            if (id == null)
            {
                return View("error");
            }

            var school = await database.schools
                .FirstOrDefaultAsync(m => m.Id == id);
            if (school == null)
            {
                return View("error");
            }

            return View(school);
        }
        //2
        // GET: Schools/Create
        public IActionResult Create()
        {
            string loggedUserId = loggedUser();
            if (loggedUserId == null)
            {
                return View("error"); //return View("U should be logged in for this action")
            }
            var user = database.loggedUserRoles.Where(x => x.UserId.Equals(loggedUserId)).ToList().FirstOrDefault();
            if (user == null) return BadRequest();
            //if (user.Role == "Admin")
            //return View();
            if (checkRole(user, "Admin")) return View();
            else ViewBag.username = "User";
            return View("error"); //obicen korisnik e nema pravo da dodava ucilista
        }

        // POST: Schools/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,name,imageUrl,city,street,modules,latitude,longitude,contact,email,teachers,workTime,numOfStudents")] School school)
        {
            if (ModelState.IsValid)
            {
                database.Add(school);
                await database.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(school);
        }

        //3
        // GET: Schools/Edit/5

        public async Task<IActionResult> Edit(int? id)
        {
            string loggedUserId = loggedUser();
            if (loggedUserId == null)
            {
                return View("error"); //return View("U should be logged in for this action")
            }
            var user = database.loggedUserRoles.Where(x => x.UserId.Equals(loggedUserId)).ToList().FirstOrDefault();
            if (user == null) return BadRequest();
            //if (user.Role != "Admin")
            //    return View("error");
            if (!checkRole(user, "Admin"))
                return View("error");

            //if (id == null)
            //{
            //    return View("error");
            //}

            var school = await database.schools.FindAsync(id);
            if (id == null || school == null)
            {
                return View("error");
            }
            return View(school);
        }

        // POST: Schools/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,name,imageUrl,city,street,modules,latitude,longitude,contact,email,teachers,workTime,numOfStudents")] School school)
        {
            if (id != school.Id)
            {
                return View("error");
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
                        return View("error");
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
        //4
        //This activity is programmed but shoulnd't be used
        // GET: Schools/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            string loggedUserId = loggedUser();
            if (loggedUserId == null)
            {
                return View("error"); //return View("U should be logged in for this action")
            }
            var user = database.loggedUserRoles.Where(x => x.UserId.Equals(loggedUserId)).ToList().FirstOrDefault();
            if (user == null) return View("error");
            //if (user.Role != "Admin")
            //    return View("error");

            if (!checkRole(user, "Admin")) return View("error");//obicen korisnik e nema pravo da brisi ucilista
 
            //if (id == null)
            //{
            //    return View("error");
            //}

            var school = await database.schools
                .FirstOrDefaultAsync(m => m.Id == id);
            if (id == null || school == null)
            {
                return View("error");
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
        //5
        //funkcija koja cita podatoci od .csv file i zacuvuva vo baza
        public void populate()
        {
           using (var reader = new StreamReader(@"D:\FINKI\Petti Semestar\Dizajn i arhitektura na softver\Sredni ucilista\sredniUcilistaMkd.txt", System.Text.Encoding.UTF8))
            {
                var line = reader.ReadLine();
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    var values = line.Split(',');
                    School school = new School(values[1], values[2], values[3], values[4], Double.Parse(values[5]), Double.Parse(values[6]),
                        values[7], values[8], values[9], values[10], values[11], Int32.Parse(values[12]));
                    database.schools.Add(school);
                    database.SaveChanges();
                }
            } 
           
        }

        //Returns id of the logged user. If the user is not logged returns null
        public string loggedUser()
        {
            if (_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) == null) return null;
            return _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        }

        //Counter used in the view for optimization of the returned list by searching
        public static int counter = 0;

        //Returns the search page with a list filled with results
        public ActionResult searchSchoolGet(string inputField)
        {
            //searchList is variable that contains the result (from the inputField) of searching in database
            //searches for city if null returns empty list
            var searchList = database.schools.Where(x => x.city.Equals(inputField)).ToList();
            //if the list is empty then it searches by name
            if (searchList.Count == 0)
            {
                searchList = database.schools.Where(x => x.name.Equals(inputField)).ToList();
            }
            //ViewbBag.valid is variable that is used in the view to check if results are found in the database from searching
            if (searchList.Count == 0) ViewBag.valid = true;
            counter = counter + 1;
            ViewBag.counter = counter;
            //Returns the searchSchoolGet view and passes the searchList as a model
            return View(searchList);
            
        }
        //Returns the credits page with contributors of the site
        public ActionResult credits()
        {
            return View("Credits");
        }
    }
}
