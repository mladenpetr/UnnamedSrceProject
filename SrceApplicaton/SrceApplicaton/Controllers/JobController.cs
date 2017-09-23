using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SrceApplicaton.Models;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Text;
using System.IO;
using System.Collections;

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
                        TechnicianNumber = 1
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
            var db = new SrceAppDatabase1Entities();
            var job = db.Job.Find(id);
            return View(job);
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
                        job.StartingHour = TimeSpan.Parse(start.Split('T')[1]);
                        job.EndingHour = TimeSpan.Parse(end.Split('T')[1]);
                        job.JobDate = DateTime.Parse(end.Split('T')[0]);
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
					job.Title += ", " + user.Name + " " + user.LastName;
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
	
	
				if (job == null)
				{
					new EntryPointNotFoundException();
				}
	
	
				if (ModelState.IsValid)
				{
					db.Technician.Find(user.TechnicianID).Job.Remove(job);
					int numTech = db.Job.Find(id).Technician.Count();
					if(numTech != 0)
					{   
						
						if (numTech == 1) //Ako ostane jedan tehničar event oboja u boju tehničara i prikazano je ime tehničara
						{   
							var tech = db.Job.Find(id).Technician.First();
							job.Color = tech.Color;
							job.Title = tech.Name + " " + tech.LastName;
						}
						else
						{   //Ukoliko ostane više tehničara miče se iz stringa title user-a koji se trenutno odjavio
							List<string> title = job.Title.Split(new string[] { ", " }, StringSplitOptions.None).ToList();
							int indexSubstring = title.IndexOf(user.Name + " " + user.LastName);
							if(indexSubstring == 0)
							{
							job.Title =  job.Title.Replace(user.Name + " " + user.LastName + ", ", "");
							}
							else
							{
							job.Title = job.Title.Replace(", " + user.Name + " " + user.LastName, "");
							}
						}
					}
					else
					{
						job.Color = null; //Ako nema prijavljenih tehničara onda se vraća defaultni event
						job.Title = null;
						job.JobNotes = null;
					}
	
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

        //Metoda koja raspodjeljuje tehničarima poslova
        //Kriterij pri dodjeljivanju posla je broj odrađenih sati ovaj mjesec zbrojen sa brojem potencijalnih radnih sati
        //Potencijalni radni sati označavaju vrijednost u slučaju da tehničar radi svaki posao za koji se prijavio u tekućem tjednu.
        //cilj je ravnomjerno raspodjeliti sate tehničarima tako da na kraju mjeseca imaju relativno slične plaće
        public string AssignTechnicians()
        {
            using (var db = new SrceAppDatabase1Entities())
            {
                var jobs = db.Job.Where(m => m.JobState == 0).ToList();
                if (jobs.Count == 0)
                {
                    return "Svi trenutni poslovi su raspodjeljeni!";
                }
                var allTechnicians = CalculatePotentialHours(db.Technician.Where(m=>m.AccessLevel=="Technician").ToList(), jobs);
                foreach (var job in jobs)
                {
                    var technicians = job.Technician.ToList();
                    var orderedTechnicians = GetSignedTechnicians(allTechnicians, technicians);
                    for (int i = technicians.Count - 1; i >= job.TechnicianNumber; i--)
                    {
                        var user = orderedTechnicians[i];
                        List<string> title = job.Title.Split(new string[] { ", " }, StringSplitOptions.None).ToList();
                        int indexSubstring = title.IndexOf(user.Key.Name + " " + user.Key.LastName);
                        if (indexSubstring == 0)
                        {
                            job.Title = job.Title.Replace(user.Key.Name + " " + user.Key.LastName + ", ", "");
                        }
                        else
                        {
                            job.Title = job.Title.Replace(", " + user.Key.Name + " " + user.Key.LastName, "");
                        }
                        job.Technician.Remove(user.Key);
                    }
                    if (job.TechnicianNumber == 1)
                    {
                        job.Color = job.Technician.First().Color;
                    }
                    job.JobState = 1;
                    UpdatePotentialHours(allTechnicians,orderedTechnicians,job);
                }
                db.SaveChanges();
            }
            return "Tehničari su uspješno raspodjeljeni!";
        }

        //Metoda koja ažurira potencijalni broj radnih sati od tehničara.
        //Svakom tehničaru koji nije odabran za posao job smanjuje se potencijalni broj radnih sati u skladu s trajanjem posla.
        private void UpdatePotentialHours(List<KeyValuePair<Technician, byte>> allTechnicians, List<KeyValuePair<Technician, byte>>  ordered,Job job)
        {
            foreach (var pair in ordered)
            {
                if (!job.Technician.Contains(pair.Key))
                {
                    allTechnicians[allTechnicians.IndexOf(pair)] = new KeyValuePair<Technician,byte>(pair.Key, (byte)(pair.Value - Byte.Parse(GetHours(job))));
                }
            }
            return;
        }

        //Metoda koja vraća Listu uređenih parova - Tehničar, potencijalni sati - popunjenu sa svim tehničarima koji rade na trenutnom poslu.
        //technicians parametar sadrži tehničare zapisane za trenutni posao.
        private List<KeyValuePair<Technician,byte>> GetSignedTechnicians(List<KeyValuePair<Technician, byte>> allTechnicians, List<Technician> technicians)
        {
            List<KeyValuePair<Technician, byte>> signedTechnicians = new List<KeyValuePair<Technician, byte>>();
            foreach (var tech in allTechnicians)
            {
                if (technicians.Contains(tech.Key))
                {
                    signedTechnicians.Add(tech);
                }
            }
            return signedTechnicians.OrderBy(m => m.Value).ToList();
        }

        //Metoda koja računa broj potencijalnih sati za svakog tehničara te dobivene podatke sprema u dictionary.
        //Na kraju se dictionary sortira te kao lista ključ-vrijednost parova vraća.
        private List<KeyValuePair<Technician,byte>> CalculatePotentialHours(List<Technician> technicians,List<Job> jobs)
        {
            Dictionary<Technician, byte> techPotentialHours = new Dictionary<Technician, byte>();
            foreach (Technician tech in technicians)
            {
                byte potentialHours = 0;
                List<Job> techJobs = jobs.Where(m => m.Technician.Contains(tech)).ToList();
                foreach (Job job in techJobs)
                {
                    potentialHours += Byte.Parse(GetHours(job));
                }
                potentialHours += (byte)tech.WorkHours;
                techPotentialHours.Add(tech, potentialHours);
            }
            return techPotentialHours.ToList();
        }

        //Metoda koja ažurira broj sati posla tehničaru koji ga radi.
        //Parametri koje funkcija prima su instance posla i tehničara koji je dodjeljen tom poslu.
        private void UpdateHours(Technician user, Job job)
        {
            byte hours = 0;
            var diff = job.EndingHour.Subtract(job.StartingHour);
            hours = (byte)diff.Hours;
            if (diff.Minutes != 0)
            {
                hours++;
            }
            user.WorkHours += hours;
            return;
        }

        //Metoda koja generira izvještaj za tekući mjesec
        //Uzimaju se svi poslovi koji su u stanju 1 odn. nisu još obračunati.
        //Metoda prolazi kroz poslove te u novu txt datoteku zapisuje radne sate
        //te ukupni broj radnih sati za tekući mjesec.
        //Po završetku, stanja obrađenih poslova se ažuriraju na 2 (zaključano stanje).
        //Tehničarima se resetiraju podaci o odrađenim satima te tako započinje praćenje rada u 
        //sljedećem mjesecu.
        public string CreateReport()
        {
            string data;
            using (var db = new SrceAppDatabase1Entities()) {
                var jobs = db.Job.Where(m => m.JobState == 2);
                List<Technician> technicians = db.Technician.Where(m=>m.AccessLevel=="Technician").ToList();

                //Lista koja je indirektno povezana sa listom tehničara a sadrži
                //listu prijavljenih poslova od tehničara u svakom njenom elementu.
                List<List<Job>> technicianJobs = new List<List<Job>>();

                //Povezivanje liste tehničara sa listom lista poslova koje su odradili.
                foreach (var tech in technicians)
                {
                    technicianJobs.Add(tech.Job.Where(m=>m.JobState==1).ToList());
                }

                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string month = DateTime.Now.Month < 10 ? "0" + DateTime.Now.Month : DateTime.Now.Month.ToString();
                string date = month + "-" + DateTime.Now.Year;
                string filePath = desktopPath + "\\Izvještaj_"+date+".txt";
                if (!System.IO.File.Exists(filePath))
                {
                    if (jobs.Count() == 0)
                    {
                        data = "Ovaj mjesec ne postoji niti jedan odrađeni posao.";
                        return data;
                    }
                    using (StreamWriter sw = new StreamWriter(filePath))
                    {
                        foreach (var tech in technicians)
                        {
                            sw.Write("\t"+tech.LastName);
                        }
                        sw.WriteLine();
                        var maxCount = 0;
                        foreach (var job in technicianJobs)
                        {
                            if (maxCount < job.Count)
                            {
                                maxCount = job.Count;
                            }
                        }
                        for (int i = 0; i < maxCount; i++)
                        {
                            foreach (var tech in technicianJobs)
                            {
                                if (i < tech.Count)
                                {
                                    sw.Write("\t" + GetHours(tech[i]));
                                } else
                                {
                                    sw.Write("\t" + " ");
                                }
                            }
                            sw.WriteLine();
                        }
                        sw.Write("Ukupno: ");
                        int j = 0;
                        foreach (var tech in technicians)
                        {
                            if (j != 0)
                            {
                                sw.Write("\t");
                            }
                            sw.Write(tech.WorkHours);
                            j++;
                        }
                    }
                    foreach (var job in jobs)
                    {
                        job.JobState = 2;
                    }
                    foreach (var tech in technicians)
                    {
                        UpdateSalary(tech);
                    }
                    db.SaveChanges();
                    data = "Izvještaj je generiran i dostupan na radnoj površini!";
                } else
                {
                    data = "Izvještaj za ovaj mjesec je već napravljen i nalazi se na radnoj površini.";
                }
            }
            return data;
        }

        private string GetHours(Job job)
        {
            byte hours;
            var diff = job.EndingHour.Subtract(job.StartingHour);
            hours = (byte)diff.Hours;
            if (diff.Minutes != 0)
            {
                hours++;
            }
            return hours.ToString();
        }

        private void UpdateSalary(Technician technician)
        {
            int HOURLY_RATE = 25;
            technician.LastMonthSalary = technician.ThisMonthSalary;
            technician.ThisMonthSalary = (byte)((int)technician.WorkHours * HOURLY_RATE);
            technician.ThisYearSalary += technician.ThisMonthSalary;
            technician.WorkHours = 0;
            return;
        }

    }
}
