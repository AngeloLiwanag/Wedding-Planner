using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WeddingPlanner.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace WeddingPlanner.Controllers
{
    public class HomeController : Controller
    {

        private MyContext dbContext;

        public HomeController(MyContext context)
        {
            dbContext = context;
        }

        // ---- Register & Login Page ----
        public IActionResult Index()
        {
            return View();
        }

        // ---- Create a User ----
        [HttpPost("CreateUser")]
        public IActionResult CreateUser(User CreateUser)
        {
            if(ModelState.IsValid)
            {
                if(dbContext.Users.Any(u => u.Email == CreateUser.Email))
                {
                    ModelState.AddModelError("Email", "Email is already in use!");
                    return View("Index");
                }
                PasswordHasher<User> hasher = new PasswordHasher<User>();
                CreateUser.Password = hasher.HashPassword(CreateUser, CreateUser.Password);
                dbContext.Add(CreateUser);
                dbContext.SaveChanges();
                HttpContext.Session.SetInt32("SessionId", CreateUser.UserId); 
                return RedirectToAction("Dashboard");
            }else{
                return View("Index");
            }
        }

        // ---- Log a User ----
        [HttpPost("LogUser")]
        public IActionResult LogUser(LoggedUser LogUser)
        {
            if(ModelState.IsValid)
            {
                var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == LogUser.LogEmail);
                if(userInDb == null)
                {
                    ModelState.AddModelError("LogEmail", "Invalid Email or Password");
                    return View("Index");
                }
                var hasher = new PasswordHasher<LoggedUser>();
                var result = hasher.VerifyHashedPassword(LogUser, userInDb.Password, LogUser.LogPassword);
                if(result == 0)
                {
                    ModelState.AddModelError("LogEmail", "Invalid Email or Password");
                    return View("Index");
                }
                HttpContext.Session.SetInt32("SessionId", userInDb.UserId);
                return RedirectToAction("Dashboard");
            }else{
                return View("Login");
            }
        }

        // ---- Dashboard Page ----
        [HttpGet("Dashboard")]
        public IActionResult Dashboard()
        {
            int? SessionId = HttpContext.Session.GetInt32("SessionId");
            if(SessionId == null)
            {
                return View("Index");
            }else{     
                var ValidUser = dbContext.Users
                    .FirstOrDefault(u => u.UserId == SessionId);     
                var AllWeddings = dbContext.Weddings
                    .Include(wedding => wedding.Creator)
                    .Include(wedding => wedding.PeopleList)
                    .ThenInclude(rsvp => rsvp.User)
                    .ToList();
                @ViewBag.User = ValidUser;
                return View("Dashboard", AllWeddings);
            }
        }

        // ---- NewWedding Page ----
        [HttpGet("NewWedding")]
        public IActionResult NewWedding()
        {
            int? SessionId = HttpContext.Session.GetInt32("SessionId");
            if(SessionId == null)
            {
                return View("Index");
            }else{
                return View("NewWedding");
            }
        }

        // ---- Create Wedding ----
        [HttpPost("CreateWedding")]
        public IActionResult CreateWedding(Wedding wed)
        {
            int? SessionId = HttpContext.Session.GetInt32("SessionId");
            if(SessionId == null)
            {
                return View("Index");
            }else{
                if(ModelState.IsValid){
                    wed.UserId = (int)SessionId;
                    dbContext.Weddings.Add(wed);
                    dbContext.SaveChanges();
                    return RedirectToAction("Dashboard");
                }else{
                    if(wed.Date < DateTime.Now)
                    {                  
                        ModelState.AddModelError("Date", "You can not have a wedding in the past.");
                        return View("NewWedding");
                    }
                    return View("NewWedding");
                }
            }
        }

        // ---- Wedding Info Page ----
        [HttpGet("Wedding/Info/{id}")]
        public IActionResult WeddingInfo(int id)
        {
            int? SessionId = HttpContext.Session.GetInt32("SessionId");
            if(SessionId == null)
            {
                return View("Index");
            }else{
                var SpecificWedding = dbContext.Weddings
                    .Include(wedding => wedding.PeopleList)
                    .ThenInclude(rsvp=> rsvp.User)
                    .FirstOrDefault(wedding => wedding.WeddingId == id);
                return View("WeddingInfo", SpecificWedding);
            }
        }

        // ---- Delete ----
        [HttpGet("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            int? SessionId = HttpContext.Session.GetInt32("SessionId");
            if(SessionId == null)
            {
                return View("Index");
            }else{
                var SpecificWedding = dbContext.Weddings
                    .Include(wedding => wedding.PeopleList)
                    .ThenInclude(rsvp=> rsvp.User)
                    .FirstOrDefault(wedding => wedding.WeddingId == id);
                dbContext.Weddings.Remove(SpecificWedding);
                dbContext.SaveChanges();
                return RedirectToAction("Dashboard");
            }

        }

        // ---- Un-RSVP ----
        [HttpGet("UnRSVP/{id}")]
        public IActionResult UnRSVP(int id)
        {
            int? SessionId = HttpContext.Session.GetInt32("SessionId");
                if(SessionId == null)
                {
                    return View("Index");
                }else{ 
                    var rsvp = dbContext.RSVPs.FirstOrDefault(wedding => wedding.WeddingId == id);
                    dbContext.RSVPs.Remove(rsvp);
                    dbContext.SaveChanges();
                    return RedirectToAction("Dashboard");
                }
        }

        // ---- RSVP ----
        [HttpGet("RSVP/{id}")]
        public IActionResult RSVP(int id)
        {
            int? SessionId = HttpContext.Session.GetInt32("SessionId");
            if(SessionId == null)
            {
                return View("Index");
            }else{ 
                var rsvp = new RSVP((int)SessionId, id);
                dbContext.RSVPs.Add(rsvp);
                dbContext.SaveChanges();
                return RedirectToAction("Dashboard");
            }
        }

        // ---- Log Out ----
        [HttpGet("LogOut")]
        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return View("Index");
        }
    }
}
