using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SrceApplicaton.Models;
using System.Data.Entity;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Text;

namespace SrceApplicaton.Controllers
{
    [MyAuthorization]
    public class JobController : Controller
    {
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
                using (var db = new SrceAppDatabase1Entities())
                {
                    List<short> jobIds = new List<short>(db.Job.Count()); // stvaranje liste sa svim JobId u bazi
                    foreach (Job job in db.Job)
                    {
                        jobIds.Add(job.JobID);
                    }

                    TimeSpan startTime;
                    TimeSpan endTime;
                    if (String.Equals(view, "month"))
                    {
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
            using (var db = new SrceAppDatabase1Entities())
            {
                return View(db.Job.Find(id));
            }
        }

        // POST: Job/Edit/5
        [HttpPost]
        public ActionResult Edit(Job job)
        {
            if (ModelState.IsValid)
            {
                using (var db = new SrceAppDatabase1Entities())
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
            }
            return View(job);
        }

        // GET: Job/Delete/5
        public ActionResult Delete(int id)
        {
            if (ModelState.IsValid)
            {
                using (var db = new SrceAppDatabase1Entities())
                {
                    var job = db.Job.Find((short)id);
                    if (job != null)
                    {

                        if (ModelState.IsValid)
                        {
                            //Izbaci posao iz liste poslova kod svakog tehnicara koji je prijavljen za taj posao
                            foreach (Technician tech in db.Job.Find((short)id).Technician)
                            {
                                tech.Job.Remove(job);
                            }
                            db.Job.Remove(job);
                            db.SaveChanges();
                        }
                    }
                }
                return RedirectToAction("Index");  
            }
            return HttpNotFound("Greška kod brisanja!");
        }

        // GET: Job/ResizeEvent
        public ActionResult ResizeEvent(int id, string end)
        {
            using (var db = new SrceAppDatabase1Entities())
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
            }
            return RedirectToAction("Index");
        }

        // GET: Job/DropEvent
        public ActionResult DropEvent(int id, string start, string end)
        {
            using (var db = new SrceAppDatabase1Entities())
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
            }
            return RedirectToAction("Index");
            
        }

        // Dohvaćanje svih poslova iz baze...
        public string GetEvents()
        {
            using (var db = new SrceAppDatabase1Entities())
            {
                var eventList = db.Job.Select(e => new
                {
                    id = e.JobID,
                    date = e.JobDate,
                    start = e.StartingHour,
                    end = e.EndingHour,
                    title = e.Title,
                    color = e.Color,
                    jobTemplate = e.TemplateID,
                    jobState = e.JobState
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
                        templateInfo = job.jobTemplate.Equals(null) ? null : TemplateInfo((byte)job.jobTemplate),
                        jobState = job.jobState
                    });
                }

                JavaScriptSerializer ser = new JavaScriptSerializer();
                return ser.Serialize(list);
            }
        }

        //Formatiranje datuma u prikaz koji fullcalendar prepoznaje
        //Povratna vrijednost je string koji sadrži pravilan format prikaza podatka.
        public string FormData(DateTime date, TimeSpan time)
        {
            string month = date.Month < 10 ? "0" + date.Month.ToString() : date.Month.ToString();
            string day = date.Day < 10 ? "0" + date.Day.ToString() : date.Day.ToString();
            var tempDate = date.Year + "-" + month + "-" + day;

            return tempDate + "T" + time;
        }

        //Metoda koja se poziva pri prijavi tehničara na određeni posao
        public ActionResult CheckIn(int id)
        {
            Technician user = Session["user"] as Technician;
            using (var db = new SrceAppDatabase1Entities())
            {
                Job job = db.Job.Find((short)id);
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
                if (ModelState.IsValid)
                {
                    db.Technician.Find(user.TechnicianID).Job.Add(job);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }

        //Metoda koja se poziva kada tehničar odjavljuje posao
        public ActionResult CheckOut(int id)
        {
            Technician user = Session["user"] as Technician;
            using (var db = new SrceAppDatabase1Entities())
            {
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
            }
            return RedirectToAction("Index");
        }

        //Metoda koja uzima sve poslove na koje je korisnik prijavljen te ih vraća u
        //Javascript funkciju koja ju je pozvala.
        [WebMethod]
        public string GetUserEvents(int id)
        {
            using (var db = new SrceAppDatabase1Entities())
            {
                var user = Session["user"] as Technician;
                var userJobs = db.Technician.Find(user.TechnicianID).Job;
                JavaScriptSerializer ser = new JavaScriptSerializer();
                var result = ser.Serialize(userJobs.Select(d => d.JobID).ToList());
                return result;
            }
        }
		
		 // GET: Job/GetUsers
        public string GetUsers()
        {
            using (var db = new SrceAppDatabase1Entities())
            {
                var eventList = db.Job.Select(e => new
                {
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

        //Metoda koja generira tekst rekonfiguracije na osnovi podataka iz baze
        //Ulazni parametar je ID rekonfiguracije
        //Povratna vrijednost je generirani string koji služi za jednostavniji prikaz rekonfiguracije klijentu.
        public string TemplateInfo(byte templateID)
        {
            using (var db = new SrceAppDatabase1Entities())
            {
                JobTemplates model = db.JobTemplates.Find(templateID);
                bool chairs = false;
                StringBuilder sb = new StringBuilder();
                sb.Append("U dvorani " + model.Hall + " potrebno je");
                if (model.Chairs != null)
                {
                    chairs = true;
                    sb.Append(" složiti " + model.Chairs + " stolica");
                    if (model.ChairLayout != null)
                    {
                        sb.Append(" u formaciju " + model.ChairLayout + ".");
                    }
                    sb.AppendLine();
                }
                if (model.Tables != null)
                {
                    if (chairs)
                    {
                        sb.Append("Uz to potrebno je složiti ");
                    }
                    else
                    {
                        sb.Append("složiti ");
                    }
                    sb.Append(model.Tables + " stolova");
                    if (model.TablesLayout != null)
                    {
                        sb.Append(" u formaciju " + model.TablesLayout);
                    }
                    sb.AppendLine();
                }
                if (model.ExtraNotes != null)
                {
                    sb.Append("Dodatne napomene: ");
                    sb.Append(model.ExtraNotes);
                    sb.AppendLine();
                }
                sb.AppendLine();
                sb.Append("Dvoranu ");
                string str = model.Wall ? "treba pregraditi." : "nije potrebno pregraditi.";
                sb.Append(str);
                return sb.ToString();
            }
        }
    }
}
