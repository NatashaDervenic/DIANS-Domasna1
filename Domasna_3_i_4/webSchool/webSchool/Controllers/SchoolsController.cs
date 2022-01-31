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

        //Function that returns true if users role is the role that we put as an argument by calling the function, otherwise returns false
        public Boolean checkRole(Roles user, string role)
        {
            if (user.Role == role) return true;
            else return false;
        }

        //The initial method for calling the main (Index) page
        public async Task<IActionResult> Index()
        {
            //loggedUserId is variable type string that contains logged user's id. If there is no logged user loggedUserId will be null
            string loggedUserId = loggedUser();
            //In the database we search for user providing it's id. If there is no result it will return null, otherwise
            //in the user variable we will have the actual user object
            var user = database.loggedUserRoles.Where(x => x.UserId.Equals(loggedUserId)).ToList().FirstOrDefault();

            if (user == null)
                ViewBag.userRole = "User"; //If the user with given credentials is not logged we store "user" as a string in the
                                           //ViewBag.userRole so we can access it in the view and handle it since we havent logged admin
            else
            {
                if (checkRole(user, "Admin"))
                    ViewBag.userRole = "Admin";//If the user with given credentials is logged in as admin we store "admin" as a string in the
                                               //ViewBag.userRole so we can access it in the view and handle it
                else ViewBag.username = "User"; //if we have logged in user but its not admin we can treat it as a usual user with basic
                                                //permissions. For this type of user we are not showing the edit and delete buttons (since is
                                                //not admin)
            }
            return View(await database.schools.ToListAsync()); //after success we return the Index view providing list
                                                               //with all the schools as a model
        }

        //Method that will show the details view about some school that is selected on the page (found by its id)
        public async Task<IActionResult> Details(int? id)
        {
            //loggedUserId is variable type string that contains logged user's id. If there is no logged user loggedUserId will be null
            string loggedUserId = loggedUser();
            //In the database we search for user providing it's id. If there is no result it will return null, otherwise
            //in the user variable we will have the actual user object
            var user = database.loggedUserRoles.Where(x => x.UserId.Equals(loggedUserId)).ToList().FirstOrDefault();
            if (user == null)
                ViewBag.userRole = "User";//If the user with given credentials is not logged we store "user" as a string in the
                                          //ViewBag.userRole so we can access it in the view and handle it since we havent logged admin
            else
            {
                if (checkRole(user, "Admin"))
                    ViewBag.userRole = "Admin";//If the user with given credentials is logged in as admin we store "admin" as a string in the
                                               //ViewBag.userRole so we can access it in the view and handle it
                else ViewBag.username = "User";//if we have logged in user but its not admin we can treat it as a usual user with basic
                                               //permissions. For this type of user we are not showing the edit and delete buttons (since is
                                               //not admin)
            }

            var school = await database.schools
                .FirstOrDefaultAsync(m => m.Id == id);
            if (id == null || school == null)
            {
                return View("error"); //if there is no school with that unique id in the database we are showing error page
            }

            return View(school);//if the request came up this far means there is no danger of not logged in user, not admin
                                //or even hacker so we can show the page where the school's properties can be edited or deleted by the admin
        }
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
        //Returns Edit page where the model's properties can be edited. This is get method so after this call the Edit page
        //should be shown with filled fields properly with the model's info. Id is the parameter that follows the request of the
        //school that should be found in the database and edited
        public async Task<IActionResult> Edit(int? id)
        {
            //loggedUserId is variable type string that contains logged user's id. If there is no logged user loggedUserId will be null
            string loggedUserId = loggedUser();
            if (loggedUserId == null)
                return View("error"); //User is not logged in so (neither is admin) so we are redirecting the error view 
            var user = database.loggedUserRoles.Where(x => x.UserId.Equals(loggedUserId)).ToList().FirstOrDefault();
            if (user == null)
                return BadRequest(); //there is no such user with those parameters so (client error) -> showing blank page. No access 
            if (!checkRole(user, "Admin"))
                return View("error"); //The edit page should be available if the user is logged and has the permissions to be admin.
                                      //Here we check if user is not admin we should't let him edit the school's informations.
                                      //That's why we are displaying error view with possibility to go back

            var school = await database.schools.FindAsync(id); //in the variable school we store the school that we found in the database
                                                               //seaching by it's id (id is the path variable in the request)
            if (id == null || school == null)
            {
                return View("error"); //if there is no school with that unique id in the database we are showing error page
            }
            return View(school); //if the request came up this far means there is no danger of not logged in user, not admin
                                 //or even hacker so we can show the page where the school's properties can be edited 
        }
        //This is the method that is called after filling the edit form. This is post method after pressing the 'save' button
        //on the edit page. We are recieving the edited school id and we are binding the info that is filled in the fields
        //as School object
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,name,imageUrl,city,street,modules,latitude,longitude,contact,email,teachers,workTime,numOfStudents")] School school)
        {
            //if there is no such school with the same id as the id that we are sending as a parameter we are showing the error page
            if (id != school.Id)
                return View("error");
            if (ModelState.IsValid) //ModelState.IsValid is true if the form is filled with proper info and contains no error
            {
                try
                {
                    database.Update(school); //update on the database with the new info about the school
                    await database.SaveChangesAsync(); //forced save changes on the database
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SchoolExists(school.Id)) //if searching in the database returns null there is no school that should be edited
                                                  //in that case we are showing the error page
                    {
                        return View("error");
                    }
                    else
                    {
                        throw; //throwing DbUpdateConcurrencyException exception if the school that is found with the id sent as a
                               //parameter cannot be updated
                    }
                }
                return RedirectToAction(nameof(Index)); //success. Redirecting to Index page
            }
            return View(school); //returns the same view with the info in the school object that is sent as a model. This is error and
                                 //we are displaying the error to the clients side on the exact place where he missed the data input 
        }
        //This activity is programmed but shoulnd't be used
        public async Task<IActionResult> Delete(int? id)
        {
            // checking if the user is logged in
            string loggedUserId = loggedUser();
            if (loggedUserId == null)
            {
                //There is no logged user -> the school should't be deleted
                return View("error"); 
            }
            //searching the school in the database
            var user = database.loggedUserRoles.Where(x => x.UserId.Equals(loggedUserId)).ToList().FirstOrDefault();
            if (user == null) return View("error");
            // checking if the user is not an Admin, returns the error view 
            // standard user is not allowed to make changes (delete school) in the database
            if (!checkRole(user, "Admin")) return View("error");
            // it searches the school with the given parameter id in the database
            // and the result is stored in the variable school
            var school = await database.schools.FirstOrDefaultAsync(m => m.Id == id);
            if (id == null || school == null)
            {
                return View("error");//no such school -> showing error page
            }
            // returns the Delete view and passes the school as a model
            return View(school);
        }

        // Delete confirm post method
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
            return RedirectToAction(nameof(Index)); //Returns Index page after a successfull delete  
        }
        //Returns true if a school with the given id is found
        private bool SchoolExists(int id)
        {
            return database.schools.Any(e => e.Id == id);
        }

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
