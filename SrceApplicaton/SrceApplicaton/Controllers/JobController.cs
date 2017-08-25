using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SrceApplicaton.Models;

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
            while (true)
            {
                if (!list.Contains(id))
                {
                    break;
                }
                id++;
            }

            return id;
        }

        // GET: Job/Create
        public ActionResult Create(string start,string end)
        {
            if (ModelState.IsValid)
            {

                List<short> jobIds = new List<short>(db.Job.Count()); // stvaranje liste sa svim JobId u bazi
                foreach (Job job in db.Job)
                {
                    jobIds.Add(job.JobID);
                }

                Job newJob = new Job
                {
                    JobID = checkId(jobIds),
                    StartingHour = TimeSpan.Parse(start.Split('T')[1]),
                    EndingHour = TimeSpan.Parse(end.Split('T')[1]),
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
    }
}
