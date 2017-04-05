using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FilmDb1.Models;
using System.IO;
using System.Drawing;
using System.Net;
namespace FilmDb1.Controllers
{
    public class HomeController : Controller
    {
        FilmContext db = new FilmContext();
        public ActionResult Index()
        {
            return View(db.Films.ToList());
        }
        public ActionResult Create()
        {

            ViewBag.Categories = db.Categories.ToList();
            var model = new FilmViewModel();
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FilmViewModel model, int[] selectedCategories)
        {
            var imageTypes = new string[]{    
                    "image/gif",
                    "image/jpeg",
                    "image/pjpeg",
                    "image/png"
                };
            if (model.Image == null || model.Image.ContentLength == 0)
            {
                ModelState.AddModelError("ImageUpload", "Додайте зображення");
            }
            else if (!imageTypes.Contains(model.Image.ContentType))
            {
                ModelState.AddModelError("ImageUpload", "Зображення повинне бути у GIF, JPG або PNG форматі.");
            }
            else if (model.CategoryId == null)
            {
                ModelState.AddModelError("CategoryId", "Додайте жанр");
            }

            if (ModelState.IsValid)
            {
                var film = new Film();
                film.Name = model.Name;
                film.Description = model.Description;
                if (model.CategoryId != null)
                {
                    foreach (var c in model.CategoryId)
                    {
                        if (db.Categories.Find(c) != null)
                        {
                            film.Categories.Add(db.Categories.Find(c));
                        }
                    }

                }
                if (model.Image != null)
                {
                    byte[] imageData = null;
                    using (var binaryReader = new BinaryReader(model.Image.InputStream))
                    {
                        imageData = binaryReader.ReadBytes(model.Image.ContentLength);
                    }
                    MemoryStream ms = new MemoryStream(imageData);
                    Image returnImage = Image.FromStream(ms);
                    ViewBag.Width = returnImage.Width;
                    ViewBag.Height = returnImage.Height;
                    film.Image = imageData;
                }
                db.Films.Add(film);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Categories = db.Categories.ToList();
            return View(model);
        }
        public ActionResult Delete(int id)
        {
            Film f = db.Films.Find(id);
            if (f == null)
            {
                return HttpNotFound();
            }
            return View(f);
        }
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
           Film f = db.Films.Find(id);
            if (f == null)
            {
                return HttpNotFound();
            }
            db.Films.Remove(f);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Film product = db.Films.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            ViewBag.Categories = db.Categories.ToList();
            return View(product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Film model, int[] selectedCategories, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                var product = db.Films.Find(model.Id);
                if (product == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                product.Name = model.Name;
                product.Description= model.Description;
                if (selectedCategories != null)
                {
                    product.Categories.Clear();
                    foreach (var c in selectedCategories)
                    {
                        if (db.Categories.Find(c) != null)
                        {
                            product.Categories.Add(db.Categories.Find(c));
                        }
                    }

                } 
                if (upload != null)
                {
                    byte[] imageData = null;
                    using (var binaryReader = new BinaryReader(upload.InputStream))
                    {
                        imageData = binaryReader.ReadBytes(upload.ContentLength);
                    }
                    MemoryStream ms = new MemoryStream(imageData);
                    Image returnImage = Image.FromStream(ms);
                    ViewBag.Width = returnImage.Width;
                    ViewBag.Height = returnImage.Height;
                    product.Image = imageData;
                }
                
               
                db.SaveChanges();
                return RedirectToAction("Index");
            }

       //     ViewBag.Categories = db.Categories.Where(x => x.IsActive == true);
            return View(model);
        }
    }
}