using BookStore.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookStore.Areas.Admin.Controllers
{
    public class BookController : Controller
    {
        BookStoreDBEntities db = new BookStoreDBEntities();
        // GET: Admin/Book
        public ActionResult Index()
        {
            var books = db.Books.ToList(); 
            return View(books);
        }
        [HttpGet]
        public ActionResult Create()
        {
            // Lấy danh sách Category từ database
            var categories = db.Categories
                               .Select(c => new { c.ID, c.NameCate })  // Dùng đúng thuộc tính
                               .ToList();

            // Truyền vào ViewBag dưới dạng SelectList
            ViewBag.Categories = new SelectList(categories, "ID", "NameCate");

            return View();

        }

        [HttpPost]
        public ActionResult Create(Book newBook, HttpPostedFileBase HinhSachFile)
        {
            if (ModelState.IsValid) // Kiểm tra ModelState
            {
                
                   /* if (HinhSachFile != null && HinhSachFile.ContentLength > 0)
                    {
                        // Lấy tên file và extension
                        string filename = Path.GetFileNameWithoutExtension(HinhSachFile.FileName);
                        string extension = Path.GetExtension(HinhSachFile.FileName);

                        // Đặt tên file mới để tránh trùng lặp
                        filename = filename + DateTime.Now.ToString("yymmssfff") + extension;

                        // Đường dẫn lưu file
                        string path = Path.Combine(Server.MapPath("~/Content/images/"), filename);

                        // Kiểm tra thư mục có tồn tại không, nếu không thì tạo mới
                        if (!Directory.Exists(Server.MapPath("~/Content/images/")))
                        {
                            Directory.CreateDirectory(Server.MapPath("~/Content/images/"));
                        }

                        // Lưu file lên server
                        HinhSachFile.SaveAs(path);

                        // Lưu đường dẫn vào thuộc tính HinhSach của sách
                        newBook.HinhSach = "/Content/images/" + filename; // Sử dụng đường dẫn tương đối
                    }*/

                    // Lưu sách vào database
                    db.Books.Add(newBook);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                
               /* catch (Exception ex)
                {
                    // Log lỗi (nếu cần thiết) và hiển thị lại view với thông báo lỗi
                    ViewBag.ErrorMessage = "Có lỗi xảy ra: " + ex.Message;
                }*/
            }
            else
            {
                ViewBag.ErrorMessage = "Thông tin không hợp lệ, vui lòng kiểm tra lại.";
            }

            // Nếu ModelState không hợp lệ hoặc có lỗi xảy ra
            var categories = db.Categories
                               .Select(c => new { c.ID, c.NameCate })
                               .ToList();
            ViewBag.Categories = new SelectList(categories, "ID", "NameCate");

            return View(newBook);
        }

    }
}