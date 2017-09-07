using SrceApplicaton.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SrceApplicaton.Controllers
{
    [MyAuthorization]
    public class TemplateController : Controller
    {
        SrceAppDatabase1Entities db = new SrceAppDatabase1Entities();
        // GET: Template
        public ActionResult Index(int id)
        {
            List<TemplateViewModel> tempModels = new List<TemplateViewModel>();
            foreach (JobTemplates temp in db.JobTemplates)
            {
                tempModels.Add(new TemplateViewModel()
                {
                    jobID = id,
                    ChairLayout = temp.ChairLayout,
                    Chairs = temp.Chairs,
                    Tables = temp.Tables,
                    TablesLayout = temp.TablesLayout,
                    Hall = temp.Hall,
                    Wall = temp.Wall,
                    ExtraNotes = temp.ExtraNotes,
                    templateID=temp.TemplateID
                });
            }
            ComplexTemplateModel model = new ComplexTemplateModel();
            model.jobID = id;
            model.viewList = tempModels;
            return View(model);
        }

        private byte checkId(List<byte> list)
        {
            byte id = 0;
            while (list.Contains(id))
            {
                id++;
            }

            return id;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ComplexTemplateModel model,int jobID)
        {
            if (ModelState.IsValid)
            {
                var newTemplate = new JobTemplates()
                {
                    TemplateID = checkId(db.JobTemplates.Select(m => m.TemplateID).ToList()),
                    Chairs = model.view.Chairs,
                    ChairLayout = model.view.ChairLayout,
                    Tables = model.view.Tables,
                    TablesLayout = model.view.TablesLayout,
                    Hall = model.view.Hall,
                    ExtraNotes = model.view.ExtraNotes,
                    Wall = model.view.Wall,
                };
                db.JobTemplates.Add(newTemplate);
                db.Job.Find(jobID).TemplateID = model.view.templateID;
                db.SaveChanges();
                TempData["message"] = "Rekonfiguracija uspješno dodana!";
                return RedirectToAction("Index", "Job", null);
            }

            return View();
        }

        public ActionResult Add (byte templateID, int jobID)
        {
            if (ModelState.IsValid)
            {
                db.Job.Find(jobID).TemplateID = templateID;
                db.SaveChanges();
                return RedirectToAction("Index", "Job", null);
            }
            return View();
        }

        public ActionResult Delete (byte templateID, int jobID)
        {
            if (ModelState.IsValid)
            {
                JobTemplates temp = db.JobTemplates.Find(templateID);
                db.Job.Find(jobID).TemplateID = null;
                db.JobTemplates.Remove(temp);
                db.SaveChanges();
                return RedirectToAction("Index", "Job", null);
            }
            return View();
        }

        public ActionResult Edit (byte templateID)
        {
            JobTemplates model = db.JobTemplates.Find(templateID);
            bool chairs = false;
            StringBuilder sb = new StringBuilder();
            sb.Append("U dvorani " + model.Hall + " potrebno je");
            if (model.Chairs!=null)
            {
                chairs = true;
                sb.Append(" složiti "+model.Chairs + " stolica");
                if (model.ChairLayout != null)
                {
                    sb.Append(" u formaciju " + model.ChairLayout+".");
                }
                sb.AppendLine();
            }
            if (model.Tables != null)
            {
                if (chairs)
                {
                    sb.Append("Uz to potrebno je složiti ");
                } else
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
            string str = model.Wall ? "treba pregraditi." : "nije potrebno pregraditi." ;
            sb.Append(str);
            TempData["message"] = sb.ToString();
            return View(model);
        }
    }
}