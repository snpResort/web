using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QL_Resort.Models;
using System.Dynamic;
using System.Text.RegularExpressions;
using Microsoft.Ajax.Utilities;
using System.Globalization;

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
            Session["Taotk"] = null;

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
                Session["Taotk"] = "Tạo tài khoản thành công";
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
            string emailkh = f["emailkh"];
            Session["emptyEmail"] = null;
            if (!String.IsNullOrEmpty(emailkh))
            {
                mymodel.LayTTKH = db.THONGTINCANHANs.Where(t=>t.Email==emailkh).FirstOrDefault();
            }
            else
            {
                Session["emptyEmail"] = "Chưa có thông tin của khách hàng này. Vui lòng nhập thông tin khách hàng";
                return RedirectToAction("ThongTinTKKH", "Admin");
            }
            var layTTCN = db.THONGTINCANHANs.Where(t => t.Email == emailkh).FirstOrDefault();
            Session["TTCN"] = layTTCN;
            return View(mymodel);
        }
        public ActionResult ThongTinTKKH()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ThongTinTKKH(FormCollection f)
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
            Session["XN"] = null;

            dynamic mymodel = new ExpandoObject();
            mymodel.Validation = new TaiKhoan();

            //check emty
            if (username == string.Empty && matkhau == string.Empty)
            {
                username = null;
                matkhau = null;
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
                var kq = db.sp_AddAccKH(username, matkhau, hoten, DateTime.Parse(ngaysinh), cccd, gioitinh, email, dienthoai, diachi);
                Session["XN"] = "Cập nhật thông tin khách hàng thành công";
            }
            var layTTCN = db.THONGTINCANHANs.Where(t => t.Email == email).FirstOrDefault();
            Session["TTCN"] = layTTCN;
            return RedirectToAction("Index", "Admin");
        }
        [HttpPost]
        public ActionResult ThanhToan(string idlp, string strURL, FormCollection f)
        {
            var TTCN = Session["TTCN"] as THONGTINCANHAN;
            var idkh = db.KHACHHANGs.Where(t => t.Id_tk == TTCN.Id_tk).FirstOrDefault().Id;
            DateTime ND = Convert.ToDateTime(f["ngaydat"]);
            DateTime NT = Convert.ToDateTime(f["ngaytra"]);

            var ttdp = db.THONGTINDATPHONGs;
            var ctdp = db.CTDATPHONGs;
            List<ThongTinPhong> data = new List<ThongTinPhong>();
            foreach (var item in ttdp)
            {
                ThongTinPhong _thongtinphong = new ThongTinPhong();
                _thongtinphong.Id_DP = item.Id;
                _thongtinphong.Ngaydat = item.NgayDat.ToString();
                _thongtinphong.Ngaytra = item.NgayTra.ToString();
                _thongtinphong.Id_P = db.CTDATPHONGs.Where(r => r.Id_DatPhong == item.Id).Select(t => t.Id_P).ToList();
                data.Add(_thongtinphong);
            }
            List<String> dsPhong = new List<String>();
            for (DateTime d = ND; d.CompareTo(NT) <= 0; d = d.AddDays(1.0))
            {
                var booked = data.Where(t => d.CompareTo(DateTime.Parse(t.Ngaydat)) >= 0 && d.CompareTo(DateTime.Parse(t.Ngaytra)) <= 0).Select(c => c.Id_P).ToList();

                foreach (var dataCtDP in booked)
                {
                    dsPhong.AddRange(dataCtDP.ToHashSet().ToList().ConvertAll<string>(x => x.ToString()).ToList());
                }

            }
            dsPhong = dsPhong.ToHashSet().ToList();
            var loaiPhong = Session["loaiPhong"] as LOAIPHONG;
            var _p = db.PHONGs.Where(p => p.Id_LP == loaiPhong.Id).ToList();

            List<String> dsPhongTrong = _p.Where(t => !dsPhong.Contains(t.Id.ToString())).Select(t => t.Id.ToString()).ToList();
            Session["TB"] = null;
            Session["done"] = null;
            if (dsPhongTrong.Count == 0)
            {
                Session["TB"] = "Het phong!!!";
                return Redirect(strURL);
            }
            var id_KH = idkh;
            string soluong = f["txtsoluong"];

            var phong = db.PHONGs.Where(p => p.Id_LP == loaiPhong.Id).ToList();
            var id_DP = dsPhongTrong[new Random().Next(dsPhongTrong.Count)];
            var SLNL = loaiPhong.SoLuongNguoiLon;
            var SLTE = loaiPhong.SoLuongTreEm;
            var Gia = f["sum"];/// ngày ở   

            Session["sl"] = null;
            var kq = db.sp_AddTTDatPhong_Off(id_KH, SLNL, SLTE, double.Parse(Gia), ND, NT, ref id_DP);
            if (id_DP != null)
            {
                if (int.Parse(soluong) > dsPhongTrong.Count)
                {
                    Session["sl"] = "Qua so luong!!!";
                    return Redirect(strURL);
                }
                else
                {
                    for (int i = 1; i <= int.Parse(soluong); i++)
                    {
                        CTDATPHONG ct = new CTDATPHONG();
                        ct.Id_DatPhong = id_DP;
                        ct.Gia = double.Parse(Gia);
                        var lstPT = dsPhongTrong[new Random().Next(dsPhongTrong.Count)];
                        int idp = int.Parse(lstPT);
                        ct.Id_P = idp;
                        dsPhongTrong.Remove(lstPT);
                        db.CTDATPHONGs.InsertOnSubmit(ct);
                    }
                    db.SubmitChanges();
                }
            }
            var us = Session["User"] as TAIKHOAN;
            int idnv = db.NHANVIENs.Where(t => t.Id_tk == us.Id).FirstOrDefault().Id;
            Random rand = new Random();
            int num = rand.Next(100000000);
            HOADON hd = new HOADON();
            hd.Id = "h" + num.ToString();
            hd.Id_DP = id_DP;
            hd.Id_NV = idnv;
            hd.TiendatCoc = 0;
            hd.NgayTao = DateTime.Now;
            hd.NgayCapNhat = DateTime.Now;
            db.HOADONs.InsertOnSubmit(hd);
            db.SubmitChanges();
            var idctdp = db.CTDATPHONGs.Where(z => z.Id_DatPhong == id_DP).FirstOrDefault();
            Session["idctdp"] = idctdp;
            return RedirectToAction("ThanhToanThanhCong", "Admin");

        }
        public ActionResult ThanhToanThanhCong()
        {
            var idCTDP = Session["idctdp"] as CTDATPHONG;
            string id = idCTDP.Id_DatPhong;
            dynamic mymodel = new ExpandoObject();
            var _ttlp = db.LOAIPHONGs.Join(db.PHONGs, lp => lp.Id, p => p.Id_LP, (lp, p) => new ThongTinLP { Id = lp.Id, Ten = lp.TenLoai, Id_p = p.Id });
            var ttdp = db.THONGTINDATPHONGs.Select(t => new ThongTinPhong
            {
                Id_DP = t.Id,
                Ten_kh = db.THONGTINCANHANs.Where(kh => kh.Id_tk == t.Id_KH).FirstOrDefault().HoTen,
                Ngaydat = t.NgayDat.ToString(),
                Ngaytra = t.NgayTra.ToString(),
                Soluong = t.CTDATPHONGs.Where(a => a.Id_DatPhong == id).Count().ToString(),
                Id_P = t.CTDATPHONGs.Where(b => b.Id_DatPhong == id).Select(a => a.Id_P).ToList(),
                Slnl = int.Parse(t.SoLuongNguoiTH.ToString()),
                Slte = int.Parse(t.SoLuongTreEm.ToString()),
            });
            var CTDP = db.CTDATPHONGs.Where(t => t.Id_DatPhong == id).FirstOrDefault();
            List<int> dsPhong = new List<int>();
            dsPhong = db.CTDATPHONGs.Where(b => b.Id_DatPhong == id).Select(a => a.Id_P).ToList();
            var lstPT = dsPhong[new Random().Next(dsPhong.Count)];
            int idp = lstPT;

            var ttpXN = ttdp.Where(t => t.Id_DP == id).FirstOrDefault();
            ttpXN.Songayo = (1 + Math.Floor(double.Parse((DateTime.Parse(ttpXN.Ngaytra) - DateTime.Parse(ttpXN.Ngaydat)).Days.ToString()))).ToString();
            ttpXN.Tenphong = _ttlp.Where(t => t.Id_p == idp).FirstOrDefault().Ten;
            double tt = double.Parse(db.CTDATPHONGs.Where(d => d.Id_DatPhong == id).FirstOrDefault().Gia.ToString());
            ttpXN.Tongtien = tt.ToString();
            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");   // try with "en-US"
            string g = double.Parse(ttpXN.Tongtien).ToString("#,###", cul.NumberFormat);
            ttpXN.Tongtien = g;
            mymodel.TTDP = ttpXN;
            Session["ThongTinPhong"] = CTDP;
            return View(mymodel);
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
            var ttdp = db.THONGTINDATPHONGs.Select(t => new ThongTinPhong
            {
                Id_DP = t.Id,
                Ten_kh = db.THONGTINCANHANs.Where(kh => kh.Id_tk == t.Id_KH).FirstOrDefault().HoTen,
                Ngaydat = t.NgayDat.ToString(),
                Ngaytra = t.NgayTra.ToString(),
                Soluong = t.CTDATPHONGs.Where(a => a.Id_DatPhong == id).Count().ToString(),
                Id_P = t.CTDATPHONGs.Where(b => b.Id_DatPhong == id).Select(a => a.Id_P).ToList(),
                Slnl=int.Parse(t.SoLuongNguoiTH.ToString()),
                Slte=int.Parse(t.SoLuongTreEm.ToString()),
            });
            var CTDP = db.CTDATPHONGs.Where(t => t.Id_DatPhong == id).FirstOrDefault();
            List<int> dsPhong = new List<int>();
            dsPhong = db.CTDATPHONGs.Where(b => b.Id_DatPhong == id).Select(a => a.Id_P).ToList();
            var lstPT = dsPhong[new Random().Next(dsPhong.Count)];
            int idp = lstPT;

            var ttpXN = ttdp.Where(t => t.Id_DP == id).FirstOrDefault();
            ttpXN.Songayo = (1 + Math.Floor(double.Parse((DateTime.Parse(ttpXN.Ngaytra) - DateTime.Parse(ttpXN.Ngaydat)).Days.ToString()))).ToString();
            ttpXN.Tenphong = _ttlp.Where(t => t.Id_p == idp).FirstOrDefault().Ten;
            double tt = double.Parse(db.CTDATPHONGs.Where(d => d.Id_DatPhong == id).FirstOrDefault().Gia.ToString());
            ttpXN.Tongtien = tt.ToString();
            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");   // try with "en-US"
            string g = double.Parse(ttpXN.Tongtien).ToString("#,###", cul.NumberFormat);
            ttpXN.Tongtien = g;
            mymodel.TTDP = ttpXN;
            Session["ThongTinPhong"] = CTDP;
            return View(mymodel);
        }
        [HttpPost]
        public ActionResult XacNhanDatPhong(FormCollection f)
        {
            var us = Session["User"] as TAIKHOAN;
            int idnv = db.NHANVIENs.Where(t => t.Id_tk == us.Id).FirstOrDefault().Id;
            var ctdp = Session["ThongTinPhong"] as CTDATPHONG;
            List<string> iddp_tt = db.HOADONs.Select(t => t.Id_DP).ToList();
            Session["datt"] = null;
            for(int i=0;i<=iddp_tt.Count;i++)
            {
                var lstPT = iddp_tt[new Random().Next(iddp_tt.Count)];
                string iddp = lstPT;
                iddp_tt.Remove(lstPT);
                if (ctdp.Id_DatPhong==iddp)
                {
                    Session["datt"] = "Id_DatPhong này đã thanh toán";
                    return RedirectToAction("DanhSachDatTruoc", "Admin");
                }
            }    
            Random rand = new Random();
            int num = rand.Next(100000000);
            HOADON hd = new HOADON();
            hd.Id = "h" + num.ToString();
            hd.Id_DP = ctdp.Id_DatPhong;
            hd.Id_NV = idnv;
            hd.TiendatCoc = 0;
            hd.NgayTao = DateTime.Now;
            hd.NgayCapNhat = DateTime.Now;
            db.HOADONs.InsertOnSubmit(hd);
            db.SubmitChanges();
            Session["XacNhan"] = null;
            Session["XacNhan"] = "Xác nhận đặt phòng thành công";
            return RedirectToAction("DanhSachDatTruoc","Admin");
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