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

        //1 Mihaela
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
                if (checkRole(user, "Admin")) 
                    ViewBag.userRole = "Admin";
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
        //2 Martina
        // GET: Schools/Create
        //A method for checking if the user is logged in and if the user is admin to return the Create View
        public IActionResult Create()
        {
            //loggedUser() is a function that returns the id of the logged user
            string loggedUserId = loggedUser();
            //If loggedUserId is null it returns Error View ("You should be logged in for this action")
            if (loggedUserId == null)
            {
                return View("error"); 
            }
            //user is a variable that takes the role that the particular user has
            var user = database.loggedUserRoles.Where(x => x.UserId.Equals(loggedUserId)).ToList().FirstOrDefault();
            //If the user variable is null it returns BadRequest()
            if (user == null) return BadRequest();
            //If the user's role is Admin the user is allowed to access the Create View which consists a fill in form
            if (checkRole(user, "Admin")) return View();
            //If the user's role is not Admin, the user is not allowed to access the Create View which leads to an Error View
            //A user that is not an Admin is not allowed to add more schools to the database
            else ViewBag.username = "User";
            return View("error");
        }

        // POST: Schools/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        //This is a method that is called after filling the create form. It's a post method that is called after pressing the 'save' button on the Create page
        //We are recieving an object that contains all the filled in inputs and we are binding the info that is filled in the inputs as a School object
        public async Task<IActionResult> Create([Bind("Id,name,imageUrl,city,street,modules,latitude,longitude,contact,email,teachers,workTime,numOfStudents")] School school)
        {
            //If we binded the object into the Model successfully we can add the object into the database
            if (ModelState.IsValid)
            {
                //Adding the object to the database
                database.Add(school);
                //Saving the newly added changes
                await database.SaveChangesAsync();
                //Returns the Index page
                return RedirectToAction(nameof(Index));
            }
            //Returns the same page but it is not valid, it shows an error
            return View(school);
        }

        //3 Bojan
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
        //4 Sofija
        //This activity is programmed but shoulnd't be used
        // GET: Schools/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            // checking if the user is logged in
            string loggedUserId = loggedUser();
            if (loggedUserId == null)
            {
                //return View("You should be logged in for this action")
                return View("error"); 
            }
            var user = database.loggedUserRoles.Where(x => x.UserId.Equals(loggedUserId)).ToList().FirstOrDefault();
            if (user == null) return View("error");
            

            // checking if the user is not an Admin, returns the error view 
            // standard user is not allowed to make changes (delete school) in the database
            if (!checkRole(user, "Admin")) return View("error");
 
            // it searches the school with the given parameter id in the database
            // and the result is stored in the variable school
            var school = await database.schools
                .FirstOrDefaultAsync(m => m.Id == id);
            if (id == null || school == null)
            {
                return View("error");
            }
            // returns the Delete view and passes the school as a model
            return View(school);
        }
        
        // POST: Schools/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //school is variable that contains the result of searching by id in the database
            var school = await database.schools.FindAsync(id);
            // it removes the school from the database 
            database.schools.Remove(school);    
            // saves all changes made in this context to the database
            await database.SaveChangesAsync();   
            return RedirectToAction(nameof(Index));    
        }

        private bool SchoolExists(int id)
        {
            return database.schools.Any(e => e.Id == id);
        }
        //5 Natasa
        //Function that reads the data from the .csv file and saves it in the base
        public void populate()
        {
           //Access to the .csv file (in this case, the .txt file)
           using (var reader = new StreamReader(@"D:\FINKI\Petti Semestar\Dizajn i arhitektura na softver\Sredni ucilista\sredniUcilistaMkd.txt", System.Text.Encoding.UTF8))
            {
                var line = reader.ReadLine();//Reads the data in the file
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    var values = line.Split(',');//It splits the string into a list
                    School school = new School(values[1], values[2], values[3], values[4], Double.Parse(values[5]), Double.Parse(values[6]),
                        values[7], values[8], values[9], values[10], values[11], Int32.Parse(values[12]));//We convert the string representation of a number in a specified style and format
                    database.schools.Add(school);//We add a new item to the list
                    database.SaveChanges();//We save the changes
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
