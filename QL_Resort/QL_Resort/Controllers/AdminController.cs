using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QL_Resort.Models;
using System.Dynamic;
using System.Text.RegularExpressions;
using Microsoft.Ajax.Utilities;

namespace QL_Resort.Controllers
{
    public class AdminController : Controller
    {
        QL_ResortDataContext db = new QL_ResortDataContext();

        // GET: Admin
        public ActionResult Index()
        {
            dynamic mymodel = new ExpandoObject();
            mymodel.LoaiPhong = db.LOAIPHONGs.ToList();
            return View(mymodel);
        }

        public ActionResult ThongTinTKNV()
        {
            dynamic mymodel = new ExpandoObject();
            mymodel.Quyens = db.QUYENs.Where(t => t.TenQuyen != "Khách hàng").ToList();
            return View(mymodel);
        }

        [HttpPost]
        public ActionResult ThongTinTKNV(FormCollection f)
        {
            var username = f["username"];
            var matkhau = f["Pass"];
            var hoten = f["Hoten"];
            var ngaysinh = f["Ngaysinh"];
            var cccd = f["cccd"];
            var gioitinh = f["GioiTinh"];
            var email = f["email"];
            var dienthoai = f["sdt"];
            var diachi = f["diachi"];
            var tenquyen = f["tenquyen"];

            dynamic mymodel = new ExpandoObject();
            mymodel.Validation = new TaiKhoan();
            mymodel.Quyens = db.QUYENs.Where(t => t.TenQuyen != "Khách hàng").ToList();

            //check emty
            if (username == string.Empty)
            {
                ViewData["LoiUser"] = "Không được bỏ trống!";
            }
            if (matkhau == string.Empty)
            {
                ViewData["LoiPass"] = "Không được bỏ trống!";
            }
            if (hoten == string.Empty)
            {
                ViewData["Loihoten"] = "Không được bỏ trống!";
            }
            if (ngaysinh == string.Empty)
            {
                ViewData["Loingaysinh"] = "Không được bỏ trống!";
            }
            if (email == string.Empty)
            {
                ViewData["LoiEmail"] = "Không được bỏ trống!";
            }
            if (dienthoai == string.Empty)
            {
                ViewData["LoiSdt"] = "Không được bỏ trống!";
            }
            else if (!Regex.Match(dienthoai, @"(84|0[3|5|7|8|9])+([0-9]{8})", RegexOptions.IgnoreCase).Success)
            {
                ViewData["LoiSdt"] = "Định dạng số điện thoại không đúng";
            }
            if (diachi == string.Empty)
            {
                ViewData["Loidiachi"] = "Không được bỏ trống!";
            }

            else
            {
                var kq = db.sp_AddAcc(username, matkhau, hoten, DateTime.Parse(ngaysinh), cccd, gioitinh, email, dienthoai, diachi, tenquyen);
            }

            return View(mymodel);
        }
        public ActionResult QLTaiKhoan()
        {

            dynamic mymodel = new ExpandoObject();
            var _tk = db.TAIKHOANs.Join(db.NHANVIENs, tk => tk.Id, nv => nv.Id_tk, (tk, nv) => new TaiKhoanNV { TenTK = tk.TenTK, NgayTao = (DateTime)tk.NgayTao, ChucVu = nv.ChucVu });
            mymodel.TK = _tk;
            return View(mymodel);
        }
        public ActionResult XemChiTiet(string id)
        {
            dynamic mymodel = new ExpandoObject();
            var ha = db.HINHANH_LOAIPHONGs.Where(t => t.Id_LP == id).ToList();
            var mota = db.THONGTINLOAIPHONGs.Where(t => t.Id_LP == id).ToList().Distinct();
            var layLP = db.LOAIPHONGs.Where(t => t.Id == id);
            var _ttlp = db.LOAIPHONGs.Join(db.THONGTINLOAIPHONGs, lp => lp.Id, ttlp => ttlp.Id_LP, (lp, ttlp) => new ThongTinLP { Ten = lp.TenLoai, Motalp = lp.MoTa, Gia = (double)lp.Gia });
            mymodel.TTLP = _ttlp;
            mymodel.HA = ha;
            mymodel.MT = mota;
            mymodel.LayLP = layLP;
            return View(mymodel);
        }
        public ActionResult DatPhong(string id)
        {

            dynamic mymodel = new ExpandoObject();
            var _layP = db.PHONGs.Where(t => t.Id_LP == id).ToList();
            var _LayTTDP = db.THONGTINDATPHONGs.ToList();
            var layLP = db.LOAIPHONGs.Where(t => t.Id == id).FirstOrDefault();
            var ha = db.HINHANH_LOAIPHONGs.Where(t => t.Id_LP == id).ToList();
            var mota = db.THONGTINLOAIPHONGs.Where(t => t.Id_LP == id).ToList().Distinct();
            var _ttlp = db.LOAIPHONGs.Join(db.THONGTINLOAIPHONGs, lp => lp.Id, ttlp => ttlp.Id_LP, (lp, ttlp) => new ThongTinLP { Ten = lp.TenLoai, Motalp = lp.MoTa, Gia = (double)lp.Gia });
            mymodel.LayP = _layP;
            mymodel.LayLP = layLP;
            mymodel.HA = ha;
            mymodel.TTLP = _ttlp;
            mymodel.LTTDP = _LayTTDP;
            mymodel.MT = mota;

            Session["loaiPhong"] = layLP;

            return View(mymodel);
        }
        public ActionResult TimKiem()
        {
            return View();
        }
        [HttpPost]
        public ActionResult TimKiem(FormCollection f)
        {
            dynamic mymodel = new ExpandoObject();
            string SLNL = f["SLNL"];
            string SLTE = f["SLTE"];
            int nl = 0;
            int te = 0;
            if (String.IsNullOrEmpty(SLNL) && String.IsNullOrEmpty(SLTE))
            {
                mymodel.LayLP = db.LOAIPHONGs.ToList();

            }
            else if (String.IsNullOrEmpty(SLTE))
            {
                nl = int.Parse(SLNL);
                mymodel.LayLP = db.LOAIPHONGs.Where(t => t.SoLuongNguoiLon >= nl).ToList();
            }
            else if (String.IsNullOrEmpty(SLNL))
            {
                te = int.Parse(SLTE);
                mymodel.LayLP = db.LOAIPHONGs.Where(t => t.SoLuongTreEm >= te).ToList();
            }
            else
            {
                nl = int.Parse(SLNL);
                te = int.Parse(SLTE);
                mymodel.LayLP = db.LOAIPHONGs.Where(t => t.SoLuongNguoiLon >= nl && t.SoLuongTreEm >= te).ToList();
            }
            mymodel.HA = db.HINHANH_LOAIPHONGs.ToList();
            return View(mymodel);
        }
        public ActionResult ThanhToanThanhCong()
        {
            return View();
        }
        public ActionResult ThanhToan(FormCollection f)
        {
            var loaiPhong = Session["loaiPhong"] as LOAIPHONG;
            var phong = db.PHONGs.Where(p => p.Id_LP == loaiPhong.Id).ToList();

            var user = Session["User"] as TAIKHOAN;
            var username = user.TenTK;
            var id_KH = user.Id;
            var id_P = phong[new Random().Next(phong.Count)].Id;
            var SLNL = loaiPhong.SoLuongNguoiLon;
            var SLTE = loaiPhong.SoLuongTreEm;
            var Gia = f["sum"];/// ngày ở

            //string TrangThai = f["TrangThai"];
            //string NgayTao = f["NgayTao"];
            var ngaydat = f["ngaydat"];
            var ngaytra = f["ngaytra"];
            //var kq = db.sp_AddTTDatPhong(username, SLNL, SLTE, double.Parse(Gia), DateTime.Parse(ngaydat), DateTime.Parse(ngaytra), id_P);
            return RedirectToAction("ThanhToanThanhCong", "Admin");
        }
        public ActionResult NhapThongTinCN()
        {
            return View();
        }
        [HttpPost]
        public ActionResult NhapThongTinCN(FormCollection f)
        {
            return View();
        }
        public ActionResult DanhSachDatTruoc()
        {
            var ttdp = db.THONGTINDATPHONGs.Select(t => new ThongTinPhong
            {
                Id_DP = t.Id,
                Ten_kh = db.THONGTINCANHANs.Where(kh => kh.Id_tk == t.Id_KH).FirstOrDefault().HoTen,
                Ngaydat = t.NgayDat.ToString(),
                Ngaytra = t.NgayTra.ToString(),
                Soluong = t.CTDATPHONGs.Count().ToString()
            });
            dynamic mymodel = new ExpandoObject();
            mymodel.TTDP = ttdp;
            return View(mymodel);
        }
        public ActionResult HienThi(string id)
        {
            dynamic mymodel = new ExpandoObject();
            var _ttlp = db.LOAIPHONGs.Join(db.PHONGs, lp => lp.Id, p => p.Id_LP, (lp, p) => new ThongTinLP { Id = lp.Id, Ten = lp.TenLoai, Id_p = p.Id });
            //List<int> dsPhong = new List<int>();
            //List<string> lp1 = new List<string>();
            //string idlp;
            //dsPhong = db.CTDATPHONGs.Where(b => b.Id_DatPhong == id).Select(a => a.Id_P).ToList();
            //var lstPT = dsPhong[new Random().Next(dsPhong.Count)];
            //int idp = lstPT;
            //dsPhong.Remove(lstPT);
            //var TTP = db.PHONGs.Where(c => c.Id == idp);
            //lp1 = TTP.Select(f => f.Id_LP).Distinct().ToList();
            //var lstLP = lp1[new Random().Next(lp1.Count)];
            //idlp = lstLP;
            //lp1.Remove(lstLP);
            //mymodel.LayLP1 = db.LOAIPHONGs.Where(d => d.Id == idlp).ToList();
            //string ttlp = (db.LOAIPHONGs.Where(d => d.Id == idlp).ToList()).Select(g => g.TenLoai).ToString();
            var ttdp = db.THONGTINDATPHONGs.Select(t => new ThongTinPhong
            {
                Id_DP = t.Id,
                Ten_kh = db.THONGTINCANHANs.Where(kh => kh.Id_tk == t.Id_KH).FirstOrDefault().HoTen,
                Ngaydat = t.NgayDat.ToString(),
                Ngaytra = t.NgayTra.ToString(),
                Soluong = t.CTDATPHONGs.Where(a => a.Id_DatPhong == id).Count().ToString(),
                Id_P = t.CTDATPHONGs.Where(b => b.Id_DatPhong == id).Select(a => a.Id_P).ToList(),
            });
            //var _ttlp = db.LOAIPHONGs.Join(db.PHONGs, lp => lp.Id, p => p.Id_LP, (lp, p) => new ThongTinLP { Id = lp.Id, Ten = lp.TenLoai, Id_p = p.Id });
            List<int> dsPhong = new List<int>();
            dsPhong = db.CTDATPHONGs.Where(b => b.Id_DatPhong == id).Select(a => a.Id_P).ToList();
            var lstPT = dsPhong[new Random().Next(dsPhong.Count)];
            int idp = lstPT;

            var ttpXN = ttdp.Where(t => t.Id_DP == id).FirstOrDefault();
            ttpXN.Songayo = (1 + Math.Floor(double.Parse((DateTime.Parse(ttpXN.Ngaytra) - DateTime.Parse(ttpXN.Ngaydat)).Days.ToString()))).ToString();
            //string p1= _ttlp.Where(c => c.Id_p == idp).Select(q=>q.Id).ToString();
            //var lp1 = db.LOAIPHONGs.Where(w => w.Id == p1).ToList();
            //string tenlp = lp1.Select(z => z.TenLoai).ToString();
            ttpXN.Tenphong = _ttlp.Where(t => t.Id_p == idp).FirstOrDefault().Ten;
            mymodel.TTDP = ttpXN;
            return View(mymodel);
            //return RedirectToAction("DanhSachDatTruoc", "Admin");
        }
        [HttpPost]
        public ActionResult XacNhanDatPhong(FormCollection f)
        {
            return View();
        }
        public ActionResult Check(string id)
        {
            dynamic mymodel = new ExpandoObject();
            mymodel.CheckP = db.PHONGs.Where(t => t.Id_LP == id).ToList();
            return View(mymodel);
        }
        public ActionResult TimKiemDatTruoc()
        {
            dynamic mymodel = new ExpandoObject();
            mymodel.TTDP = db.THONGTINDATPHONGs.ToList();
            return View(mymodel);
        }
        [HttpPost]
        public ActionResult TimKiemDatTruocGo(FormCollection f)
        {
            dynamic mymodel = new ExpandoObject();
            string txtTK = f["txtTK"];
            var ttdp = db.THONGTINDATPHONGs.Select(t => new ThongTinPhong
            {
                Id_DP = t.Id,
                Ten_kh = db.THONGTINCANHANs.Where(kh => kh.Id_tk == t.Id_KH).FirstOrDefault().HoTen,
                Ngaydat = t.NgayDat.ToString(),
                Ngaytra = t.NgayTra.ToString(),
            });
            mymodel.TTDP = ttdp;
            if (!String.IsNullOrEmpty(txtTK))
            {
                mymodel.TTDP = ttdp.Where(t => t.Id_DP == txtTK).ToList();
            }
            return View(mymodel);
        }
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
            var kq = db.sp_Login(username.Trim(), password.Trim(), true).First();

            if (kq.ID != null)
            {
                TAIKHOAN tttk = db.TAIKHOANs.SingleOrDefault(c => c.TenTK == username);
                Session["User"] = tttk;
                Session["tdn"] = tttk.TenTK;
                return RedirectToAction("Index", "Admin");
            }
            return View();
        }


    }
}