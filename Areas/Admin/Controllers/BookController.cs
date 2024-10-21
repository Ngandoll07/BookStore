using BookStore.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Reflection;
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
            // Lấy danh sách thể loại
            ViewBag.Categories = new SelectList(db.Categories.ToList(), "ID", "NameCate");

            // Lấy danh sách tác giả
            ViewBag.TacGias = new SelectList(db.TacGias.ToList(), "ID", "NameAuthor");

            // Lấy danh sách nhà phân phối (Org)
            ViewBag.Orgs = new SelectList(db.Users.ToList(), "ID", "NameUser");

            // Lấy danh sách mã giảm giá (Coupon)
            ViewBag.Coupons = new SelectList(db.Coupons.ToList(), "ID", "CouponCode");

            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Book newBook, HttpPostedFileBase HinhSachFile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (HinhSachFile != null && HinhSachFile.ContentLength > 0)
                    {
                        // Tạo tên file duy nhất nếu cần
                        var fileName = Path.GetFileName(HinhSachFile.FileName);
                        var path = Path.Combine(Server.MapPath("~/Content/Images/"), fileName);
                        HinhSachFile.SaveAs(path);
                        newBook.HinhSach = "/Content/Images/" + fileName; // Lưu đường dẫn hình ảnh vào model
                    }

                    // Lưu model vào cơ sở dữ liệu
                    // (Giả sử bạn có một phương thức lưu trữ nào đó)
                    db.Books.Add(newBook);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;

                // Nếu lỗi, load lại các dropdowns
                ViewBag.Categories = new SelectList(db.Categories.ToList(), "ID", "NameCate");
                ViewBag.TacGias = new SelectList(db.TacGias.ToList(), "ID", "NameAuthor");
                ViewBag.Orgs = new SelectList(db.Users.ToList(), "ID", "NameUser");
                ViewBag.Coupons = new SelectList(db.Coupons.ToList(), "ID", "CouponCode");

                return View(newBook);
            }
        }
        public ActionResult Edit(string id)
        {
            var book = db.Books.Find(id); // Lấy sản phẩm theo ID
            if (book == null)
            {
                return HttpNotFound(); // Nếu không tìm thấy sản phẩm
            }

            var model = new Book
            {
                ID = book.ID,
                NameBook = book.NameBook,
                IDCAT = book.IDCAT,
                GIA = book.GIA,
                BookDescrip = book.BookDescrip,
                HinhSach = book.HinhSach,
                IDOrg = book.IDOrg,
                IDCoupon = book.IDCoupon,
                IDTG = book.IDTG
            };

            // Tạo danh sách cho dropdown
            ViewBag.Categories = new SelectList(db.Categories.ToList(), "ID", "NameCate");
            ViewBag.TacGias = new SelectList(db.TacGias.ToList(), "ID", "NameAuthor");
            ViewBag.Orgs = new SelectList(db.Users.ToList(), "ID", "NameUser");
            ViewBag.Coupons = new SelectList(db.Coupons.ToList(), "ID", "CouponCode");

            return View(model); // Trả về View với model đã có thông tin
        }

        [HttpPost]
        public ActionResult Edit(Book newBook, HttpPostedFileBase HinhSachFile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingBook = db.Books.Find(newBook.ID); // Tìm sản phẩm cũ bằng ID

                    if (existingBook != null)
                    {
                        // Cập nhật các thuộc tính
                        existingBook.NameBook = newBook.NameBook;
                        existingBook.IDCAT = newBook.IDCAT;
                        existingBook.GIA = newBook.GIA;
                        existingBook.BookDescrip = newBook.BookDescrip;
                        existingBook.IDOrg = newBook.IDOrg;
                        existingBook.IDCoupon = newBook.IDCoupon;
                        existingBook.IDTG = newBook.IDTG;

                        // Nếu có hình mới, cập nhật đường dẫn
                        if (HinhSachFile != null && HinhSachFile.ContentLength > 0)
                        {
                            var fileName = Path.GetFileName(HinhSachFile.FileName);
                            var path = Path.Combine(Server.MapPath("~/Content/Images/"), fileName);
                            HinhSachFile.SaveAs(path);
                            existingBook.HinhSach = "/Content/Images/" + fileName; // Cập nhật đường dẫn hình ảnh
                        }

                        // Lưu thay đổi vào cơ sở dữ liệu
                        db.SaveChanges();

                        return RedirectToAction("Index");
                    }
                    ModelState.AddModelError("", "Không tìm thấy sản phẩm.");
                }
                // Nếu không hợp lệ, load lại các dropdowns
                LoadDropdowns();
                return View(newBook);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;

                // Nếu có lỗi, load lại các dropdowns
                LoadDropdowns();
                return View(newBook);
            }
        }

        // Phương thức để load các dropdown
        private void LoadDropdowns()
        {
            ViewBag.Categories = new SelectList(db.Categories.ToList(), "ID", "NameCate");
            ViewBag.TacGias = new SelectList(db.TacGias.ToList(), "ID", "NameAuthor");
            ViewBag.Orgs = new SelectList(db.Users.ToList(), "ID", "NameUser");
            ViewBag.Coupons = new SelectList(db.Coupons.ToList(), "ID", "CouponCode");
        }
        // GET: Books/Delete/5
        public ActionResult Delete(string id)
        {
            var book = db.Books.Find(id); // Tìm sản phẩm theo ID
            if (book == null)
            {
                return HttpNotFound(); // Nếu không tìm thấy sản phẩm
            }
            return View(book); // Trả về view để xác nhận xóa
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            var book = db.Books.Find(id); // Tìm sản phẩm theo ID
            if (book != null)
            {
                db.Books.Remove(book); // Xóa sản phẩm
                db.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu
            }
            return RedirectToAction("Index"); // Chuyển hướng về danh sách sản phẩm
        }


    }
}