using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using QL_Resort.Models;


namespace QL_Resort.Controllers
{
    public class HomeController : Controller
    {
        QL_ResortDataContext db = new QL_ResortDataContext();
        public ActionResult Index()
        {
            var lp = db.LOAIPHONGs;
            List<LoaiPhong> data=new List<LoaiPhong>();
            foreach(var item in lp)
            {
                LoaiPhong _loaiphong = new LoaiPhong();
                _loaiphong.Id = item.Id;
                _loaiphong.Ten = item.TenLoai;
                _loaiphong.Mota = item.MoTa;
                _loaiphong.Gia = (double)item.Gia;
                _loaiphong.Hinhanh = db.HINHANH_LOAIPHONGs.Where(t => t.Id_LP == item.Id).FirstOrDefault().imagePath;
                //_loaiphong.Id_P = (Int32.Parse(db.PHONGs.Where(a => a.Id_LP == item.Id).ToString()));
                _loaiphong.Ha = db.HINHANH_LOAIPHONGs.Where(t => t.Id_LP == item.Id).ToList();
                data.Add(_loaiphong);
            }
            dynamic mymodel = new ExpandoObject();
            mymodel.LoaiPhong = data;
            return View(mymodel);
        }
        public ActionResult ThongTinTK()
        {
            var user = Session["User"] as TAIKHOAN;
            
            var infoAccount = db.THONGTINCANHANs.Where(tk => tk.Id_tk == user.Id).FirstOrDefault();
            dynamic mymodel = new ExpandoObject();
            mymodel.Info = infoAccount;
            return View(mymodel);
        }
        public ActionResult CapNhatThongTinTK(String id_tk)
        {
            var infoAccount = db.THONGTINCANHANs.Where(tk => tk.Id_tk == id_tk).FirstOrDefault();
            dynamic mymodel = new ExpandoObject();
            ViewData["LoiHoTen"] = "";
            ViewData["LoiCccd"] = "";
            ViewData["LoiNgaySinh"] = "";
            ViewData["LoiDiaChi"] = "";
            ViewData["LoiSdt"] = "";
            mymodel.Info = infoAccount;
            return View(mymodel);
        }
        [HttpPost]
        public ActionResult CapNhatThongTinTK(FormCollection f)
        {
            String hoten = f["HoTen"];
            var ngaysinh = f["NgaySinh"];
            var Cccd = f["Cccd"];
            var sdt = f["sdt"];
            var diachi = f["diachi"];
            var gioitinh = f["GioiTinh"];
            var user = Session["User"] as TAIKHOAN;
            var infoAccount = db.THONGTINCANHANs.Where(tk => tk.Id_tk == user.Id).FirstOrDefault();
            dynamic mymodel = new ExpandoObject();


            Session["Taotk"] = null;

            if (hoten == string.Empty)
            {
                ViewData["LoiHoTen"] = "Không được bỏ trống!";
            }
            if (ngaysinh == string.Empty)
            {
                ViewData["LoiNgaySinh"] = "Không được bỏ trống!";
            }
            if (Cccd == string.Empty)
            {
                ViewData["LoiCccd"] = "Không được bỏ trống!";
            }
            if (sdt == string.Empty)
            {
                ViewData["LoiSdt"] = "Không được bỏ trống!";
            }
            else if (!Regex.Match(sdt, @"(84|0[3|5|7|8|9])+([0-9]{8})", RegexOptions.IgnoreCase).Success)
            {
                ViewData["LoiSdt"] = "Định dạng số điện thoại không đúng";
            }
            if (diachi == string.Empty)
            {
                ViewData["LoiDiaChi"] = "Không được bỏ trống!";
            }

            else
            {
                var kq = db.sp_EditInfoUser(user.TenTK, hoten, DateTime.Parse(ngaysinh), Cccd, gioitinh, sdt, diachi);
                Session["Taotk"] = "Tạo tài khoản thành công";
            }
            mymodel.Info = infoAccount;
            return View(mymodel);
        }
    }
}