using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SrceApplicaton.Models;
using System.Web.Script.Serialization;
using System.Web.Services;


namespace SrceApplicaton.Controllers
{
    public class JobController : Controller
    {
        private SrceAppDatabase1Entities db = new SrceAppDatabase1Entities();
        // GET: Job
        public ActionResult Index()
        {
            return View(db.Job.ToList());
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
                };
                db.Job.Add(newJob);
                db.SaveChanges();
                TempData["message"] = "Posao je uspješno dodan!";
                return RedirectToAction("Index");
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
            return View();
        }

        // POST: Job/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
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
                end = e.EndingHour
            }).ToList();

            var list = new List<object>();

            foreach (var job in eventList)
            {
               list.Add(new
                {
                    id = job.id,
                    start = FormData(job.date, job.start),
                    end = FormData(job.date, job.end)
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
    }
}
