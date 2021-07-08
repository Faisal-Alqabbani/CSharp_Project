using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using C_Exam.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace C_Exam.Controllers
{
    public class HomeController : Controller
    {
       private MyContext _context;
       public HomeController(MyContext context)
       {
           _context = context;
       }
       [HttpGet("")]
        public IActionResult Index()
        {
            if(HttpContext.Session.GetInt32("uid") == null){
                return View("Index");
            }else{
                return RedirectToAction("Hobby");
            }
          
        }
        [HttpPost("create")]
        public IActionResult Create(User user){
            if(ModelState.IsValid)
            {
                if(_context.Users.Any(u=> u.Username == user.Username)){
                    ModelState.AddModelError("Username", "This Email already Exists!");
                    return View("Index", user);
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                user.Password = Hasher.HashPassword(user, user.Password);
                _context.Add(user);
                _context.SaveChanges();
                HttpContext.Session.SetInt32("uid", user.UserId);
                return RedirectToAction("Hobby");
            }
            else
            {
                return View("Index");
            }
        }
        [HttpPost("loginform")]
        public IActionResult LoginForm(UserLogin LogUser){
            var userInDb = _context.Users.FirstOrDefault(u => u.Username == LogUser.UsernameLogin);
            if(ModelState.IsValid){
if(userInDb == null)
            {
                ModelState.AddModelError("UsernameLogin", "Email or password is invalid!");
                return View("Index", LogUser);
            }
            else
            {
                var hasher = new PasswordHasher<UserLogin>();
                var result = hasher.VerifyHashedPassword(LogUser,userInDb.Password,LogUser.PasswordLogin);
                if(result == 0){
                    ModelState.AddModelError("UsernameLogin", "Email or Password is Invalid");
                    return View("Index", LogUser);
                }
                HttpContext.Session.SetInt32("uid", userInDb.UserId);
                
            }
            return RedirectToAction("Hobby");
            }else{
                return View("Index");
            }
            
        }
        [HttpGet("Hobby")]
        public IActionResult Hobby(){
             if(HttpContext.Session.GetInt32("uid") == null){
                 return RedirectToAction("Index");
             }else{
                    ViewBag.AllHobbies = _context.Hobbies
                    .Include(u => u.Users)
                    .ThenInclude(us => us.User)
                    .OrderByDescending(a => a.Users.Count)
                    .ToList();
                    ViewBag.ProficiencyExpert = _context.Hobbies
                    .Include(h => h.Users)
                    .Where(a => a.Users.Max(u => u.Proficiency == "Expert" ))
                    .ToList();
                    ViewBag.ProficiencyIntermediate = _context.Hobbies
                    .Include(h => h.Users)
                    .Where(a => a.Users.Max(u => u.Proficiency == "Intermediate" ))
                    .ToList();

                    ViewBag.ProficiencyNovice = _context.Hobbies
                    .Include(h => h.Users)
                    .Where(a => a.Users.Max(u => u.Proficiency == "Novice" ))
                    .ToList();

                    
                    
                    return View();
             }
           
        }
        // Create New Hobby
        [HttpGet("new")]
        public IActionResult NewHobby()
        {
             if(HttpContext.Session.GetInt32("uid") == null){
                 return RedirectToAction("Index");
             }else{
                   return View();
             }
          
        }
        [HttpPost("CreateHobbyForm")]
        public IActionResult CreateHobbyForm(Hobby NewHobby)
        {
            if(ModelState.IsValid){
                if(_context.Hobbies.Any(u=> u.Name == NewHobby.Name)){
                   ModelState.AddModelError("Name","Name already Exsists"); 
                   return View("NewHobby");
                }else
                {
                _context.Add(NewHobby);
                _context.SaveChanges();
                return RedirectToAction("Hobby");  
                }
               
            }else{
                return View("NewHobby");
            }
        }

        // Get One Hobby
        [HttpGet("/hobby/{id}")]
        public IActionResult GetOneHobby(int id)
        {
             if(HttpContext.Session.GetInt32("uid") == null){
                 return RedirectToAction("Index");
             }else{
            Hobby hobby = _context.Hobbies
            .Include(u => u.Users)
            .ThenInclude(us => us.User)
            .FirstOrDefault(h => h.HobbyId == id);
            ViewBag.LoggedUser = _context.Users.FirstOrDefault(u => u.UserId == (int)HttpContext.Session.GetInt32("uid"));
            return View("OneHobby", hobby);
             }
            
        }
        // Edit Hobby
        [HttpGet("/edit/{id}")]
        public IActionResult Edit(int id)
        {
             if(HttpContext.Session.GetInt32("uid") == null)
             {
                 return RedirectToAction("Index");  
             }else
             {
            Hobby editHobby = _context.Hobbies.FirstOrDefault(u => u.HobbyId == id);
            return View("Edit", editHobby);
             }
          
        }
        [HttpPost("editform/{id}")]
        public IActionResult Editform(Hobby editHobby, int id)
        {
            Hobby OldHobby = _context.Hobbies.FirstOrDefault(u => u.HobbyId == id);
            if(ModelState.IsValid)
            {
                OldHobby.Name = editHobby.Name;
                OldHobby.Description = editHobby.Description;
                OldHobby.UpdatedAt = DateTime.Now;
                _context.SaveChanges();
                return RedirectToAction("Hobby");
            }
            else
            {
                return View("Edit", OldHobby);
            }
        }
        // Add to Association
        // Add to Hobbies
        [HttpPost("AddToHobby/{id}")]
        public IActionResult AddToHobby(int id, string Proficiency)
        {
            
            Association Aso = new Association();
            Aso.UserId = (int)HttpContext.Session.GetInt32("uid");
            Aso.HobbyId = id;
            Aso.Proficiency = Proficiency;
            _context.Associations.Add(Aso);
            _context.SaveChanges();
            return RedirectToAction("GetOneHobby", new{id=id});
        }
    }
}
