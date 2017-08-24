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

        // GET: Job/Create
        public ActionResult Create(string start,string end)
        {
            byte tempID = (byte)db.JobTemplates.Count();
            if (ModelState.IsValid)
            {
                Job newJob = new Job
                {
                    JobID = (short)db.Job.Count(),
                    StartingHour = TimeSpan.Parse(start.Split('T')[1]),
                    EndingHour = TimeSpan.Parse(end.Split('T')[1]),
                    JobDate = DateTime.Parse(start.Split('T')[0]),
                    TemplateID = tempID
                };

                JobTemplates template = new JobTemplates
                {
                    TemplateID = tempID,
                    Wall = false
                };
                db.Job.Add(newJob);
                db.JobTemplates.Add(template);
                db.SaveChanges();
                TempData["message"] = "Posao je uspješno dodan!";
                return RedirectToAction("Index", "Job");
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
        public ActionResult Edit(int id)
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
            return View();
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
