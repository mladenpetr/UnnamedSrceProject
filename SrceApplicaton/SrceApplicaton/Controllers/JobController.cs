using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SrceApplicaton.Models;
using System.Data.Entity;
using System.Web.Script.Serialization;
using System.Web.Services;


namespace SrceApplicaton.Controllers
{
    [MyAuthorization]
    public class JobController : Controller
    {
        private SrceAppDatabase1Entities db = new SrceAppDatabase1Entities();
        // GET: Job
        public ActionResult Index()
        {
            return View();
        }

        // GET: Job/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        public short checkId(List<short> list)
        {
            short id = 0;
        
            while(list.Contains(id))
            {
                id++;
            }

            return id;
        }

        // GET: Job/Create
        public ActionResult Create(string start, string end, string view)
        {
            if (ModelState.IsValid)
            {

                List<short> jobIds = new List<short>(db.Job.Count()); // stvaranje liste sa svim JobId u bazi
                foreach (Job job in db.Job)
                {
                    jobIds.Add(job.JobID);
                }

                TimeSpan startTime;
                TimeSpan endTime;
                if (String.Equals(view, "month")){
                    startTime = new TimeSpan(16, 00, 00);
                    endTime = new TimeSpan(18, 00, 00);
                }
                else
                {
                    startTime = TimeSpan.Parse(start.Split('T')[1]);
                    endTime = TimeSpan.Parse(end.Split('T')[1]);
                }

                Job newJob = new Job
                {
                    JobID = checkId(jobIds),
                    StartingHour = startTime,
                    EndingHour = endTime,
                    JobDate = DateTime.Parse(start.Split('T')[0]),
                    JobState = 0,
                    TechnicianNumber = 2
                };
                db.Job.Add(newJob);
                db.SaveChanges();
                TempData["message"] = "Posao je uspješno dodan!";
                return View("Index");
            }
            return View();
        }

        // POST: Job/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Job/Edit/5
        public ActionResult Edit(short id)
        {
            return View(db.Job.Find(id));
        }

        // POST: Job/Edit/5
        [HttpPost]
        public ActionResult Edit(Job job)
        {
            if (ModelState.IsValid)
            {
                var jb = db.Job.Find(job.JobID);
                jb.Title = job.Title;
                jb.StartingHour = job.StartingHour;
                jb.EndingHour = job.EndingHour;
                jb.JobNotes = job.JobNotes;
                jb.JobDate = job.JobDate;
                if (job.JobTemplates != null)
                {
                    jb.JobTemplates.Hall = job.JobTemplates.Hall;
                    jb.JobTemplates.Chairs = job.JobTemplates.Chairs;
                    jb.JobTemplates.ChairLayout = job.JobTemplates.ChairLayout;
                    jb.JobTemplates.Tables = job.JobTemplates.Tables;
                    jb.JobTemplates.TablesLayout = job.JobTemplates.TablesLayout;
                    jb.JobTemplates.ExtraNotes = job.JobTemplates.ExtraNotes;
                    jb.JobTemplates.Wall = job.JobTemplates.Wall;
                }
                db.SaveChanges();
                TempData["message"] = "Uspješno ste promijenili podatke o poslu!";
                return View(db.Job.Find(job.JobID));
            }
            return View(job);
        }

        // GET: Job/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                var job = db.Job.Find((short)id);
                if (job != null)
                {
					
                    if (ModelState.IsValid)
                    {
						//Izbaci posao iz liste poslova kod svakog tehnicara koji je prijavljen za taj posao
                        foreach(Technician tech in db.Job.Find((short)id).Technician)
                        {
                            tech.Job.Remove(job);
                        }
                        db.Job.Remove(job);
                        db.SaveChanges();
                    }
                }
                return RedirectToAction("Index");
            } catch
            {
                return HttpNotFound("Greška kod brisanja!");
            }
        }

        // POST: Job/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Job/ResizeEvent
        public ActionResult ResizeEvent(int id, string end)
        {
            try
            {
                var job = db.Job.Find(id);
                if (job != null)
                {
                    if (ModelState.IsValid)
                    {
                        DateTime newDate = DateTime.Parse(end.Split('T')[0]);

                        if (newDate.Equals(job.JobDate))
                        {
                            db.Job.Find(id).EndingHour = TimeSpan.Parse(end.Split('T')[1]);
                            db.SaveChanges();
                        }

                    }
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Job/DropEvent
        public ActionResult DropEvent(int id, string start, string end)
        {
            try
            {
                var job = db.Job.Find(id);
                if (job != null)
                {
                    if (ModelState.IsValid)
                    {
                        db.Job.Find(id).StartingHour = TimeSpan.Parse(start.Split('T')[1]);
                        db.Job.Find(id).EndingHour = TimeSpan.Parse(end.Split('T')[1]);
                        db.Job.Find(id).JobDate = DateTime.Parse(end.Split('T')[0]);
                        db.SaveChanges();
                    }
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Job/GetEvents
        public string GetEvents()
        {
            var eventList = db.Job.Select(e => new {
                id = e.JobID,
                date = e.JobDate,
                start = e.StartingHour,
                end = e.EndingHour,
                title = e.Title,
                color = e.Color,
                jobNotes = e.JobNotes
            }).ToList();

            var list = new List<object>();

            foreach (var job in eventList)
            {
               list.Add(new
                {
                    id = job.id,
                    start = FormData(job.date, job.start),
                    end = FormData(job.date, job.end),
                    title = job.title,
                    color = job.color,
                    notes = job.jobNotes
                });
            }

            JavaScriptSerializer ser = new JavaScriptSerializer();

            return ser.Serialize(list);
        }


        public string FormData(DateTime date, TimeSpan time)
        {
            string month = date.Month < 10 ? "0" + date.Month.ToString() : date.Month.ToString();
            string day = date.Day < 10 ? "0" + date.Day.ToString() : date.Day.ToString();
            var tempDate = date.Year + "-" + month + "-" + day;

            return tempDate + "T" + time;
        }

        public ActionResult CheckIn(int id)
        {
            Technician user = Session["user"] as Technician;

            Job job = db.Job.Find((short)id);

            string newNotes = "rekoniguracija 50stolova, pozvat odrzavanje ak treba pomoc";
            job.JobNotes = newNotes;
            if (job.Title == null)
            {
                job.Title = user.Name + " " + user.LastName;
                job.Color = user.Color;
            }
            else
            {
                job.Title += user.Name + " " + user.LastName + ", ";
                job.Color = "#9f00ff";
            }
            
            if (job == null)
            {
                new EntryPointNotFoundException();
            }
            job.JobNotes += user.Name + " " + user.LastName + "\n";
            if (ModelState.IsValid)
            {
                db.Technician.Find(user.TechnicianID).Job.Add(job);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public ActionResult CheckOut(int id)
        {
            Technician user = Session["user"] as Technician;
            Job job = db.Job.Find((short)id);
            job.Color = null;
            job.Title = null;
            job.JobNotes = null;
            if (job == null)
            {
                new EntryPointNotFoundException();
            }

            if (ModelState.IsValid)
            {
                db.Technician.Find(user.TechnicianID).Job.Remove(job);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        //Metoda koja uzima sve poslove na koje je korisnik prijavljen te ih vraća u
        //Javascript funkciju koja ju je pozvala.
        [WebMethod]
        public string GetUserEvents(int id)
        {
            var user = Session["user"] as Technician;
            var userJobs = db.Technician.Find(user.TechnicianID).Job;
            JavaScriptSerializer ser = new JavaScriptSerializer();
            var result = ser.Serialize(userJobs.Select(d => d.JobID).ToList());
            return result;
        }
		
		 // GET: Job/GetUsers
        public string GetUsers()
        {
            var eventList = db.Job.Select(e => new {
                id = e.JobID,
                date = e.JobDate,
                start = e.StartingHour,
                end = e.EndingHour,
                title = e.Title,
                color = e.Color,
                jobNotes = e.JobNotes
            }).ToList();
            var usersList = db.Technician.Select(e => new
            {
                name = e.Name,
                lastName = e.LastName,
                hours = e.WorkHours,
                level = e.AccessLevel
            }).ToList();

            JavaScriptSerializer ser = new JavaScriptSerializer();

            return ser.Serialize(usersList);
        }
    }
}
