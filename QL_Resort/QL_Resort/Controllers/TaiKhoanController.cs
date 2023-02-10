using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.WebPages;
using Microsoft.Ajax.Utilities;
using QL_Resort.Models;

namespace QL_Resort.Controllers
{
    public class TaiKhoanController : Controller
    {
        QL_ResortDataContext db = new QL_ResortDataContext();
        // GET: TaiKhoan
        public ActionResult DangNhap()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DangNhap(FormCollection f)
        {
            //khai báo cac biến nhận gtri tu form f
            var username = f["username"];
            var password = f["pw"];
            ViewBag.Username = username;
            if (String.IsNullOrEmpty(username))
            {
                ViewData["Loi1"] = " Vui lòng nhập tên đăng nhập ";
            }
            if (String.IsNullOrEmpty(password))
            {
                ViewData["Loi2"] = " Vui lòng nhập mật khẩu ";
            }
            var kq = db.sp_Login(username.Trim(), password.Trim(), false).First();

            if (kq.ID != null)
            {
                TAIKHOAN tttk = db.TAIKHOANs.SingleOrDefault(c => c.TenTK == username);
                Session["User"] = tttk;
                Session["tdn"] = tttk.TenTK;
                return RedirectToAction("Index", "Home");

            }
            ViewData["Loi2"] = " Vui lòng xác nhận lại thông tin đăng nhập";
            return View();
        }

        public ActionResult DangXuat()
        {
            Session["User"] = null;
            Session["tdn"] = null;
            return RedirectToAction("Index", "Home");
        }
        public ActionResult DangKy()
        {
            return View();
        }
        public bool IsValidEmail(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
        public ActionResult XacThucEmail(int code)
        {
            return View(code);
        }
        [HttpPost]
        public ActionResult XacThucEmail(FormCollection f, int code)
        {
            var maxacnhan = f["MaXacNhan"];
            if (String.Compare(maxacnhan, code.ToString()) != 0)
            {
                ViewData["LoiXacNhan"] = "Mã xác nhận không đúng";
                return View(code);
                //Response.Write("<script>alert('Mật khẩu nhập lại không đúng!!!')</script>");
            }
            //System.Console.WriteLine(code);
            //return RedirectToAction("NhapThongTinKH");
            if ((Session["verify"] ?? "").ToString() != "")
            {
                return RedirectToAction("DoiMatKhau");
            }
            else
            {
                return RedirectToAction("NhapThongTinTK");
            }
        }

        [HttpPost]
        public ActionResult DangKy(FormCollection f)
        {

            //khai báo cac biến nhận gtri tu form f
            var email = f["email"];
            var password = f["pw"];
            var repassword = f["repw"];
            ViewBag.email = email;
            bool isError = false;

            ViewData["Loi1"] = string.Empty;

            // check empty)
            if (String.IsNullOrEmpty(email))
            {
                ViewData["Loi1"] = " Vui lòng nhập email ";
                isError = true;

            }

            else if (!IsValidEmail(email))
            {
                ViewData["Loi1"] = "Email không hợp lệ ";
                isError = true;

            }
            else if ((db.TAIKHOANs?.Where(tk => tk.TenTK == email)?.Count() ?? 0) != 0)
            {
                ViewData["Loi1"] = "Email đã tồn tại";
                isError = true;
            }
            if (String.IsNullOrEmpty(password))
            {
                ViewData["Loi2"] = " Vui lòng nhập mật khẩu ";
                isError = true;

            }
            if (!Regex.Match(password, @"(?=.*?[A - Z])(?=.*?[a - z])(?=.*?[0 - 9])(?=.*?[#?!@$%^&*-]).{8,}", RegexOptions.IgnoreCase).Success)
            {
                ViewData["Loi2"] = " Mật khẩu phải tối thiểu tám ký tự, ít nhất có:\n- Một chữ hoa\n- Một chữ thường\n- Một số và một ký tự đặc biệt ";
                isError = true;

            }
            if (String.IsNullOrEmpty(repassword))
            {
                ViewData["Loi3"] = " Vui lòng xác nhận lại mật khẩu ";
                isError = true;

            }

            if (String.Compare(password, repassword) != 0)
            {
                ViewData["Loi3"] = "Nhập lại mật khẩu không đúng";
                isError = true;

                //Response.Write("<script>alert('Mật khẩu nhập lại không đúng!!!')</script>");
            }

            if (!isError)
            {
                Session["email"] = email;
                Session["password"] = password;
                int code = MailUtils.SendMailVerifyCode(email);
                return RedirectToAction("XacThucEmail", new { code = code });
            }
            return View();
        }

        [HttpPost]
        public ActionResult NhapThongTinTK(FormCollection f)
        {
            //khai báo cac biến nhận gtri tu form f
            string email = Session["email"].ToString();
            string password = Session["password"].ToString();
            var hoten = f["HoTen"];
            var ngaysinh = f["NgaySinh"];
            var cccd = f["Cccd"];
            var gioitinh = f["GioiTinh"];
            var sdt = f["sdt"];
            var diachi = f["diachi"];
            bool isError = false;

            //check emty
            if (String.IsNullOrEmpty(hoten))
            {
                ViewData["LoiHoTen"] = " Vui lòng vào họ tên ";
                isError = true;
            }
            if (String.IsNullOrEmpty(ngaysinh))
            {
                ViewData["LoiNgaySinh"] = " Vui lòng nhập vào ngày sinh ";
                isError = true;
            }
            if (String.IsNullOrEmpty(cccd))
            {
                ViewData["LoiCccd"] = "Vui lòng nhập vào căn cước công dân ";
                isError = true;
            }
            if (String.IsNullOrEmpty(sdt))
            {
                ViewData["LoiSdt"] = " Vui lòng nhập vào số điện thoại ";
                isError = true;
            }
            else if (!Regex.Match(sdt, @"(03|05|07|08|09|01[2|6|8|9])+([0-9]{8})", RegexOptions.IgnoreCase).Success)
            {
                ViewData["LoiSdt"] = "Định dạng số điện thoại không đúng";
                isError = true;
            }
            if (String.IsNullOrEmpty(diachi))
            {
                ViewData["LoiDiaChi"] = " Vui lòng vào địa chỉ ";
                isError = true;
            }
            else
            {
                var kq = db.sp_AddAccKH(email, password, hoten, DateTime.Parse(ngaysinh), cccd, gioitinh, email, sdt, diachi);
            }
            if (isError)
            {
                return View();
            }
            return RedirectToAction("DangNhap");
        }
        public ActionResult NhapThongTinTK()
        {

            return View();
        }
        public ActionResult QuenMatKhau()
        {

            return View();
        }
        [HttpPost]
        public ActionResult QuenMatKhau(FormCollection f)
        {
            string email = f["email"];
            bool isError = false;
            if (String.IsNullOrEmpty(email))
            {
                ViewData["Loi1"] = "Vui lòng nhập email";
                isError = true;
            }
            else if ((db.TAIKHOANs?.Where(tk => tk.TenTK == email)?.Count() ?? 0) == 0)
            {
                // todo: show alert message
                ViewData["Loi1"] = "Email không tồn tại";
                isError = true;

            }
            // todo: create flag
            if (!isError)
            {
                Session["verify"] = "qmk";
                Session["emailChangePw"] = email;
                int code = MailUtils.SendMailVerifyCode(email);
                return RedirectToAction("XacThucEmail", new { code = code });
            }
            return View(); 
        }
        public ActionResult DoiMatKhau()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DoiMatKhau(FormCollection f)
        {
            var user = Session["User"] as TAIKHOAN;
            var password = f["pw"];
            var repassword = f["repw"];
            bool isError = false;
            Session["dmktc"] = null;

            if (String.IsNullOrEmpty(password))
            {
                ViewData["Loi1"] = "Không được bỏ trống Email";
                isError = true;

            }
            if (String.IsNullOrEmpty(password))
            {
                ViewData["Loi2"] = " Vui lòng nhập mật khẩu ";
                isError = true;

            }
            if (!Regex.Match(password, @"(?=.*?[A - Z])(?=.*?[a - z])(?=.*?[0 - 9])(?=.*?[#?!@$%^&*-]).{8,}", RegexOptions.IgnoreCase).Success)
            {
                ViewData["Loi2"] = " Mật khẩu phải tối thiểu tám ký tự, ít nhất có:\n- Một chữ hoa\n- Một chữ thường\n- Một số và một ký tự đặc biệt ";
                isError = true;

            }
            if (String.IsNullOrEmpty(repassword))
            {
                ViewData["Loi3"] = " Vui lòng xác nhận lại mật khẩu ";
                isError = true;

            }

            if (String.Compare(password, repassword) != 0)
            {
                ViewData["Loi3"] = "Nhập lại mật khẩu không đúng";
                isError = true;

                //Response.Write("<script>alert('Mật khẩu nhập lại không đúng!!!')</script>");
            }
            else
            {
                Session["emailChangePw"] = user.TenTK;
                var result = db.sp_changePassword(Session["emailChangePw"]?.ToString() ?? "", password);
            }
            if (!isError)
            {
                Session["dmktc"] = "Đổi mật khẩu thành công";
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
    }
}