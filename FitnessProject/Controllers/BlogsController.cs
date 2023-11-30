using FitnessProject.Models;
using FitnessProject.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FitnessProject.Controllers
{
    public class BlogsController : Controller
    {
        private FitnessWebDbEntities db = new FitnessWebDbEntities();

        // GET: Blogs
        public ActionResult Index()
        {
            var posts = db.Posts.ToList();

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
                FullName = postWithClientFullName?.Fullname,
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(Post model, HttpPostedFileBase images)
        {
            try
            {
                int userId = Convert.ToInt32(Session["User_id"]);
                FitnessWebDbEntities db = new FitnessWebDbEntities();

                if (images.ContentLength > 0)
                {
                    string rootFolder = Server.MapPath("/Data");
                    string pathImages = rootFolder + "/" + images.FileName;
                    images.SaveAs(pathImages);

                    model.images = Url.Content("/Data/" + images.FileName);
                }
                var author = db.Clients
                  .Where(u => u.user_id == userId)
                  .Select(u => new { Id = u.id, Fullname = u.Fullname })
                  .FirstOrDefault();

                model.client_id = author.Id;
                model.author = author.Fullname;
                db.Posts.Add(model);
                db.SaveChanges();
                Session["Post"] = true;
                return RedirectToAction("Index", "Home");
            }
            catch (DbEntityValidationException ex)
            {
                HandleValidationErrors(ex);
            }
            return RedirectToAction("Index");
        }

        public ActionResult Detail(int id)
        {
            var post = db.Posts
            .Where(u => u.id == id)
            .FirstOrDefault();    
            if (post == null)
            {  
                return RedirectToAction("Index"); 
            }
            return View(post);
        }


        public ActionResult MyBlogs()
        {
            if (Session["IsLoggedIn"] != null && (bool)Session["IsLoggedIn"])
            {
                int userId = (int)Session["User_id"];
                var userPosts = db.Posts.Where(p => p.client_id == userId).ToList();
                string userFullName = db.Clients.Where(c => c.id == userId).Select(c => c.Fullname).FirstOrDefault();
                PostViewModel model = new PostViewModel
                {
                    Posts = userPosts,
                    FullName = userFullName,
                };

                return View("MyBlogs", model);
            }
            return RedirectToAction("Login", "Account");
        }

        public ActionResult Edit(int id)
        {
            Post post = db.Posts.Find(id);

            int userId = (int)Session["User_id"];
            if (post != null && post.client_id == userId)
            {
                var categories = db.Categories.ToList();
                PostViewModel model = new PostViewModel
                {
                    Post = post,
                    Categories = categories
                };

                return View(model);
            }
            else
            {
                return RedirectToAction("MyBlogs");
            }
        }

        [HttpPost]
        public ActionResult Edit(PostViewModel editedPost)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Post existingPost = db.Posts.Find(editedPost.Post.id);
                    Console.WriteLine($"Existing post found: {existingPost != null}");

                    if (existingPost != null)
                    {
                        existingPost.title = editedPost.Post.title;
                        existingPost.content = editedPost.Post.content;
                        existingPost.category_id = editedPost.Post.category_id;
                        db.Entry(existingPost).State = EntityState.Modified;
                        db.SaveChanges();

                        return RedirectToAction("MyBlogs");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Bài đăng không tồn tại.");
                    }
                }
            }
            catch (DbEntityValidationException ex)
            {
                HandleValidationErrors(ex);
            }

            var categories = db.Categories.ToList();
            editedPost.Categories = categories;

            return View(editedPost);
        }



        private void HandleValidationErrors(DbEntityValidationException ex)
        {
            foreach (var validationErrors in ex.EntityValidationErrors)
            {
                foreach (var validationError in validationErrors.ValidationErrors)
                {
                    Console.WriteLine($"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                }
            }
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                var post = db.Posts.Find(id);
                if (post != null)
                {
                    db.Posts.Remove(post);
                    db.SaveChanges();
                }
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }
        public ActionResult Appointments()
        {
            FitnessWebDbEntities db = new FitnessWebDbEntities();

            bool check = Session["IsLoggedIn"] as bool? ?? false;

            if (check)
            {
                
                List<appointment> appointments = db.appointments.ToList();

                return View(appointments);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

    }
}
