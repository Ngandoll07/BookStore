using BookStore.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookStore.Areas.Admin.Controllers
{
    public class TacGiaController : Controller
    {
        BookStoreDBEntities db = new BookStoreDBEntities();
        // GET: Admin/TacGia
        public ActionResult Index()
        {
            var tacGiaList = db.TacGias.ToList(); // Lấy danh sách tác giả từ database
            return View(tacGiaList);
        }
        public ActionResult Create()
        {
            var tg = new BookStore.Models.TacGia();  // Một đối tượng book mới
            return View(tg);
        }
        [HttpPost]
        public ActionResult Create(TacGia newTacGia, HttpPostedFileBase HinhAnhTG)
        {
            try
            {
                if (HinhAnhTG != null && HinhAnhTG.ContentLength > 0)
                {
                    // Lấy tên file và extension
                    string filename = Path.GetFileNameWithoutExtension(HinhAnhTG.FileName);
                    string extension = Path.GetExtension(HinhAnhTG.FileName);

                    // Đặt tên file mới để tránh trùng lặp
                    filename = filename + DateTime.Now.ToString("yymmssfff") + extension;

                    // Đường dẫn lưu file
                    string path = Path.Combine(Server.MapPath("~/Content/images/"), filename);

                    // Lưu file lên server
                    HinhAnhTG.SaveAs(path);

                    // Lưu đường dẫn vào thuộc tính HinhSach của sách
                    newTacGia.HinhTG = "~/Content/images/" + filename;
                }

                // Lưu sách vào database
                db.TacGias.Add(newTacGia);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log lỗi (nếu cần thiết) và hiển thị lại view với thông báo lỗi
                ViewBag.ErrorMessage = ex.Message;
                return View(newTacGia);
            }
        }
        public ActionResult Edit(string id)
        {
            var existingTacGia = db.TacGias.Find(id);  // Tìm tác giả theo ID
            if (existingTacGia == null)
            {
                return HttpNotFound(); // Nếu không tìm thấy, trả về lỗi 404
            }
            return View(existingTacGia); // Hiển thị form chỉnh sửa với thông tin tác giả
        }


        [HttpPost]
        public ActionResult Edit(TacGia tacGia, HttpPostedFileBase HinhTG)
        {
            if (ModelState.IsValid)
            {
                var existingTacGia = db.TacGias.Find(tacGia.ID); // Tìm tác giả theo ID
                if (existingTacGia != null)
                {
                    // Cập nhật các thuộc tính của tác giả
                    existingTacGia.NameAuthor = tacGia.NameAuthor;
                    existingTacGia.NgaySinh = tacGia.NgaySinh;
                    existingTacGia.TGDescription = tacGia.TGDescription;

                    // Nếu có ảnh mới, lưu và cập nhật
                    if (HinhTG != null && HinhTG.ContentLength > 0)
                    {
                        string filename = Path.GetFileNameWithoutExtension(HinhTG.FileName);
                        string extension = Path.GetExtension(HinhTG.FileName);
                        filename = filename + DateTime.Now.ToString("yymmssfff") + extension;
                        string path = Path.Combine(Server.MapPath("~/Content/images/"), filename);

                        HinhTG.SaveAs(path);
                        existingTacGia.HinhTG = "~/Content/images/" + filename;
                    }

                    db.SaveChanges(); // Lưu thay đổi vào DB
                    return RedirectToAction("Index"); // Quay lại trang danh sách
                }
            }
            return View(tacGia); // Nếu không hợp lệ, hiển thị lại form với dữ liệu hiện tại
        }
        public ActionResult Delete(string id)
        {
            var category = db.TacGias.Find(id);
            if (category == null)
            {
                return HttpNotFound(); // Return a 404 error if the category is not found
            }
            return View(category); // Pass the category to the confirmation view
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            try
            {
                // Find the category by ID
                var tg = db.TacGias.Find(id);

                if (tg == null)
                {
                    return HttpNotFound(); // Return a 404 if the category is not found
                }

                // Remove the found category from the database
                db.TacGias.Remove(tg);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log the exception message (optional, for debugging purposes)
                return Content("Lỗi: " + ex.Message);
            }
        }
    }
}