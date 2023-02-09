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
        public ActionResult LichSuDP()
        {
            var user = Session["User"] as TAIKHOAN;

            var ttdp = db.THONGTINDATPHONGs
                .Where(dp => dp.Id_KH == user.Id)
                .Select(dp => new ThongTinDP
                {
                    Gia = dp.DonGia ?? 0,
                    NgDat = dp.NgayDat.ToString(),
                    NgTra = dp.NgayTra.ToString(),
                    SoLuongNgLon = dp.SoLuongNguoiTH.ToString(),
                    SoLuongTreEm = dp.SoLuongTreEm.ToString(),
                    Id = dp.Id
                }).ToList();

            /*
             * select Id_DatPhong, CTDATPHONG.Gia, TenLoai, count(*) SoLuong 
                from CTDATPHONG join Phong
                    on CTDATPHONG.Id_P = Phong.Id join LoaiPhong
                    on LoaiPhong.Id = Phong.Id_LP
                Group by Id_DatPhong, CTDATPHONG.Gia, TenLoai
             */
            var ctttlp = db.LOAIPHONGs.Select(lp => new ChiTietTTLP
            {
                Id = lp.Id,
                Gia = lp.Gia ?? 0,
                Ten = lp.TenLoai,
                Id_p = db.PHONGs.Where(p => p.Id_LP == lp.Id).Select(p => p.Id).ToList()
            }).ToList();

            var ctdpgr = db.CTDATPHONGs.ToList().Select(ct => new CtDP
            {
                Gia = ct.Gia.ToString(),
                Id_DatPhong = ct.Id_DatPhong.ToString(),
                TenLoai = ctttlp.Where(pp => pp.Id_p.Contains(ct.Id_P)).FirstOrDefault().Ten,
            }).ToList()
            .GroupBy(tt => new
            {
                tt.Id_DatPhong,
                tt.TenLoai,
                tt.Gia
            }).ToList().Select(tt => new CtDP
            {
                Gia = tt.Key.Gia,
                Id_DatPhong = tt.Key.Id_DatPhong,
                SoLuong = tt.Count().ToString(),
                TenLoai = tt.Key.TenLoai,
            }).ToList();



            //foreach (var line in )
            //{
            //    var a = line.Count();
            //}

            dynamic mymodel = new ExpandoObject();
            mymodel.ttdp = ttdp.Select(dp => new ThongTinDP
            {
                Gia = dp.Gia,
                NgDat = dp.NgDat,
                NgTra = dp.NgTra,
                SoLuongNgLon = dp.SoLuongNgLon,
                SoLuongTreEm = dp.SoLuongTreEm,
                Id = dp.Id,
                Ctdp = ctdpgr.Where(ct => ct.Id_DatPhong == dp.Id).ToList(),
            }).ToList();
            //mymodel.ctdp = ctdpgr;
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
            return RedirectToAction("ThongTinTK", "Home");
        }
    }
}