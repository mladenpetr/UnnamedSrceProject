using System.Web.Mvc;
using SrceApplicaton.Models;
using System.Net;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Web;
using System.Security.Principal;
using System.Web.Security;
using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Policy;
using System.Security.Cryptography;
using System.Text;

namespace SrceApplicaton.Controllers
{
    [MyAuthorization]
    public class AccountController : Controller
    {
        private SrceAppDatabase1Entities db = new SrceAppDatabase1Entities();

        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                List<short> techIds = new List<short>(db.Technician.Count());
                foreach (Technician tech in db.Technician)
                {
                    techIds.Add(tech.TechnicianID);
                }
                byte[] hashedPassword = GetHashedPassword(model.Password, model.Username);
                string storedPassword = Convert.ToBase64String(hashedPassword);
                Technician newUser = new Technician
                {
                    TechnicianID = new JobController().checkId(techIds),
                    Name = model.Name,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    DateOfBirth = model.DateOfBirth,
                    DateHired = model.DateHired,
                    Color = model.Color,
                    email = model.email,
                    Username = model.Username,
                    Password = storedPassword,
                    AccessLevel = "Technician",
                    WorkHours = 0,
                    ThisMonthSalary = 0,
                    LastMonthSalary = 0,
                    ThisYearSalary = 0
                    
                };
                db.Technician.Add(newUser);
                db.SaveChanges();
                TempData["message"] = "Uspješno ste registrirali račun. Možete se ulogirati sa kreiranim podacima";
                return RedirectToAction("Index", "Home");
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        private byte[] GetHashedPassword(string password, string username)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(username);
            while (sb.Length < 8)
            {
                sb.Append(username.Last());
            }
            byte[] salt = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 1000);
            return pbkdf2.GetBytes(15);

        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var user = AttemptLogIn(model.Username, model.Password);
            if (user != null)
            {
                Session["user"] = user;
                TempData["message"] = "Prijava je uspješna!";
                return RedirectToAction("Index", "Job");
            }
            TempData["message"] = "Upisano je krivo korisničko ime ili lozinka";
            return View();
        }

        private Technician AttemptLogIn(string username, string password)
        {
            var hashedPassword = Convert.ToBase64String(GetHashedPassword(password, username));
            var user = db.Technician.Where(u => u.Username == username && 
                u.Password == hashedPassword).FirstOrDefault();
            return user;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logoff()
        {
            Session["user"] = null;
            return RedirectToAction("Index", "Home");
        }

        public ActionResult AccountDetails()
        {
            var user = (Session["user"] as Technician);
            var currentUser = db.Technician.Find(user.TechnicianID);
            return View(currentUser);
        }

        public ActionResult Edit()
        {
            var user = (Session["user"] as Technician);
            var currentUser = db.Technician.Find(user.TechnicianID);
            
            return View(currentUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Technician user)
        {
            if (ModelState.IsValid)
            {
                var usr = db.Technician.Find(user.TechnicianID);
                usr.Name = user.Name;
                usr.LastName = user.LastName;
                usr.PhoneNumber = user.PhoneNumber;
                usr.DateOfBirth = user.DateOfBirth;
                usr.email = user.email;
                db.SaveChanges();
                TempData["message"] = "Uspješno ste promijenili osobne podatke!";
                return View(db.Technician.Find(user.TechnicianID));
            }
            return View(user);
        }
    }
}