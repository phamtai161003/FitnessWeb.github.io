﻿using FitnessProject.Models;
using FitnessProject.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace FitnessProject.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            FitnessWebDbEntities db = new FitnessWebDbEntities();

            List<Post> posts = db.Posts.ToList();

            var postWithClientFullName = (from post in db.Posts
                                          join client in db.Clients on post.client_id equals client.id
                                          where post.client_id == client.id
                                          select new
                                          {
                                              client.Fullname
                                          }).FirstOrDefault();



            PostViewModel model = new PostViewModel
            {
                Posts = posts,

            };
            return View(model);  

        }

        public ActionResult ProFile()
        {
            FitnessWebDbEntities db = new FitnessWebDbEntities();
            

            if (Session["IsLoggedIn"] != null && (bool)Session["IsLoggedIn"])
            {
                // Lấy giá trị User_id từ Session
                int userId = Convert.ToInt32(Session["User_id"]);
                var user = db.Clients
                  .Where(u => u.user_id == userId)
                  .Select(u => new { Id = u.id, Fullname = u.Fullname, Gender = u.gender, BirthDay = u.birthday, Email = u.email, Phone = u.phone, anh = u.anh,weight =u.weight,height = u.height })
                  .FirstOrDefault();



                if (user == null)
                {

                    // Handle the case where user is null
                    return RedirectToAction("createProfile");

                    // Birthday is not null, calculate age and create view model
                    
                }
                else
                {
                    DateTime? nullableDateTime = user.BirthDay;
                    int? age = CalculateAge(nullableDateTime);
                    Session["Client_id"] = user.Id;

                    ClientViewModel model = new ClientViewModel
                    {
                        Fullname = user.Fullname,
                        Gender = user.Gender,
                        Email = user.Email,
                        Phone = user.Phone,
                        Age = age.Value,
                        Anh = user.anh,
                        Weight = user.weight,
                        Height = user.height,
                        Birthday = user.BirthDay
                        
                    };
                    Session["checkUser"] = true;
                    return View(model);  
                }
            }
            else
            {
                Session["checkUser"] = false;
                // Nếu người dùng chưa đăng nhập, có thể chuyển hướng hoặc xử lý khác tùy thuộc vào yêu cầu của bạn
                return RedirectToAction("Login", "Account");
            }

        }
        public ActionResult createProfile()
        {
            return View();
        }
        [HttpPost]
        public ActionResult createProfile(Client model, HttpPostedFileBase images)
        {
            int userId = Convert.ToInt32(Session["User_id"]);
            FitnessWebDbEntities db = new FitnessWebDbEntities();

            if (images.ContentLength > 0)
            {
                string rootFolder = Server.MapPath("/Data");
                string pathImages = rootFolder + "/" + images.FileName;
                images.SaveAs(pathImages);

                model.anh = Url.Content("/Data/" + images.FileName);
            }

            model.user_id = userId;
            db.Clients.AddOrUpdate(model);
            db.SaveChanges();
            return RedirectToAction("ProFile");
        }



        public ActionResult UpdateProfile()
        {
            FitnessWebDbEntities db = new FitnessWebDbEntities();

            int userId = Convert.ToInt32(Session["User_id"]);
            var user = db.Clients
              .Where(u => u.user_id == userId)
              .FirstOrDefault();

            Client model = user;
            return View(model);
        }

        [HttpPost]
        
        public ActionResult UpdateProfile(Client model, HttpPostedFileBase images)
        {
            int userId = Convert.ToInt32(Session["User_id"]);
            FitnessWebDbEntities db = new FitnessWebDbEntities();

               if (images.ContentLength > 0)
                {
                    string rootFolder = Server.MapPath("/Data");
                    string pathImages = rootFolder + "/" + images.FileName;
                   images.SaveAs(pathImages);

                   model.anh = Url.Content("/Data/" + images.FileName);
                }
                var update = db.Clients
               .Where(u => u.user_id == userId)
               .FirstOrDefault();
                update.Fullname = model.Fullname;
                update.gender = model.gender;
                update.email = model.email;
                update.phone = model.phone;
                update.anh= model.anh;
                update.height= model.height;
                update.weight=model.weight;
                update.birthday = model.birthday;
                update.user_id= userId;
                
                db.SaveChanges();
                return RedirectToAction("ProFile");
        }





        public static int? CalculateAge(DateTime? birthDate)
        {
            if (birthDate.HasValue)
            {
                DateTime currentDate = DateTime.Now;
                int age = currentDate.Year - birthDate.Value.Year;

                // Kiểm tra xem ngày sinh đã qua ngày sinh nhật trong năm chưa
                if (currentDate.Month < birthDate.Value.Month ||
                    (currentDate.Month == birthDate.Value.Month && currentDate.Day < birthDate.Value.Day))
                {
                    age--;
                }

                return age;
            }

            // Trả về null nếu ngày sinh là null
            return null;
        }

        public ActionResult Appointments()
        {
            FitnessWebDbEntities db = new FitnessWebDbEntities();
            int userId = Convert.ToInt32(Session["User_id"]);
            var client = db.Clients
                  .Where(u => u.user_id == userId)
                  .Select(u => new { Id = u.id })
                  .FirstOrDefault();




            bool check = Session["IsLoggedIn"] as bool? ?? false;

            if (check)
            {
                ViewBag.ClientId = client.Id;

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

        }
        [HttpPost]
        public ActionResult Appointments(appointment model)
        {
            FitnessWebDbEntities db = new FitnessWebDbEntities();

            db.appointments.Add(model);
            db.SaveChanges();

            return RedirectToAction("Index");
        }


        

    }
}