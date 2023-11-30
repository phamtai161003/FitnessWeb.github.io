using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FitnessProject.Models;

namespace FitnessProject.Areas.Admin.Controllers
{
    public class ClientController : Controller
    {
        private readonly FitnessWebDbEntities _db;

        public ClientController()
        {
            _db = new FitnessWebDbEntities();
        }

        public ActionResult Client()
        {
            try
            {
                List<Client> clients = _db.Clients.ToList();
                return View(clients);
            }
            catch (Exception ex)
            {
                // Handle the exception (e.g., log, display an error message)
                return View("Error");
            }
        }

        public ActionResult Create()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                // Handle the exception (e.g., log, display an error message)
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Client client)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _db.Clients.Add(client);
                    _db.SaveChanges();
                    return RedirectToAction("Client");
                }
                return View(client);
            }
            catch (Exception ex)
            {
                // Handle the exception (e.g., log, display an error message)
                return View("Error");
            }
        }

        public ActionResult Edit(int id)
        {
            try
            {
                Client client = _db.Clients.Find(id);
                if (client == null)
                {
                    return HttpNotFound();
                }
                return View(client);
            }
            catch (Exception ex)
            {
                // Handle the exception (e.g., log, display an error message)
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Client client, HttpPostedFileBase images)
        {
            try
            {
                if (images != null && images.ContentLength > 0)
                {
                    string rootFolder = Server.MapPath("/Data");
                    string pathImages = rootFolder + "/" + images.FileName;
                    images.SaveAs(pathImages);
                    client.anh = Url.Content("/Data/" + images.FileName);
                }

                if (ModelState.IsValid)
                {
                    _db.Entry(client).State = EntityState.Modified;
                    _db.SaveChanges();
                    return RedirectToAction("Client");
                }
                return View(client);
            }
            catch (Exception ex)
            {
                // Handle the exception (e.g., log, display an error message)
                return View("Error");
            }
        }

        public ActionResult Delete(int id)
        {
            try
            {
                Client client = _db.Clients.Find(id);
                if (client == null)
                {
                    return HttpNotFound();
                }
                return View(client);
            }
            catch (Exception ex)
            {
                // Handle the exception (e.g., log, display an error message)
                return View("Error");
            }
        }

        [HttpPost]
        [ActionName("DeleteConfirmed")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Client client = _db.Clients.Find(id);
                _db.Clients.Remove(client);
                _db.SaveChanges();
                return RedirectToAction("Client");
            }
            catch (Exception ex)
            {
                // Handle the exception (e.g., log, display an error message)
                return View("Error");
            }
        }

        public ActionResult Search(string searchString)
        {
            try
            {
                List<Client> clients = _db.Clients.Where(c => c.Fullname.Contains(searchString) || c.email.Contains(searchString) || c.phone.Contains(searchString)).ToList();
                return View("Client", clients);
            }
            catch (Exception ex)
            {
                // Handle the exception (e.g., log, display an error message)
                return View("Error");
            }
        }
    }
}
