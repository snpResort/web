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
using System.Security.Policy;

namespace QL_Resort.Controllers
{
    public class AdminController : Controller
    {
        QL_ResortDataContext db = new QL_ResortDataContext();

        // GET: Admin
        public ActionResult Index()
        {
            if (Session["AdminUser"] == null)
            {
                return RedirectToAction("DangNhap");
            }
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

            List<ThongTinPhong> data = new List<ThongTinPhong>();
            var ttdp = db.THONGTINDATPHONGs;

            data = ttdp.Select(tt => new ThongTinPhong
            {
                Id_DP = tt.Id,
                Ngaydat = tt.NgayDat.ToString(),
                Ngaytra = tt.NgayTra.ToString(),
                Id_P = db.CTDATPHONGs.Where(r => r.Id_DatPhong == tt.Id).Select(t => t.Id_P).ToList()
            }).ToList();

            Session["dataPhong"] = data;

            mymodel.dataPhong = data;
            return View(mymodel);
        }
        [HttpPost]
        public JsonResult DatPhong(String ngayDat, String ngayTra)
        {
            var data = Session["dataPhong"] as List<ThongTinPhong>;
            List<String> dsPhong = new List<String>();
            for (DateTime d = DateTime.Parse(ngayDat); d.CompareTo(DateTime.Parse(ngayTra)) <= 0; d = d.AddDays(1.0))
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

            return Json(new { listPhongTrong = dsPhongTrong }, JsonRequestBehavior.AllowGet);
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
            var idtk = db.TAIKHOANs.Where(tk => tk.TenTK == emailkh).FirstOrDefault().Id;
            var layTTCN = db.THONGTINCANHANs.Where(t => t.Id_tk == idtk).FirstOrDefault();
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
            var idtk = db.TAIKHOANs.Where(tk => tk.TenTK == email).FirstOrDefault().Id;
            var layTTCN = db.THONGTINCANHANs.Where(t => t.Id_tk == idtk).FirstOrDefault();
            Session["TTCN"] = layTTCN;
            return RedirectToAction("Index", "Admin");
        }
        [HttpPost]
        public ActionResult ThanhToan(string idlp, string strURL, FormCollection f)
        {
            var TTCN = Session["TTCN"] as THONGTINCANHAN;
            var kh = db.KHACHHANGs.ToList();
            var idkh1s = db.KHACHHANGs.Where(t => t.Id_tk == TTCN.Id_tk).ToList();
            var idkh = db.KHACHHANGs.Where(t => t.Id_tk == TTCN.Id_tk).FirstOrDefault().Id;
            DateTime ND = Convert.ToDateTime(f["ngaydat"]);
            DateTime NT = Convert.ToDateTime(f["ngaytra"]);

            // todo: check empty ngay dat & ngay tra
            if (String.IsNullOrEmpty(f["ngaydat"].Trim()))
            {
                Session["EmptyNgay"] = "ngaydat";
                return RedirectToAction("DatPhong", "Phong", new { id = idlp });
            }
            if (String.IsNullOrEmpty(f["ngaytra"].Trim()))
            {
                Session["EmptyNgay"] = "ngaytra";
                return RedirectToAction("DatPhong", "Phong", new { id = idlp });
            }

            if (String.IsNullOrEmpty(f["txtsoluong"].Trim()))
            {
                Session["EmptyNgay"] = "soLuong";
                return RedirectToAction("DatPhong", "Phong", new { id = idlp });
            }

            // todo check phòng trống
            var data = Session["dataPhong"] as List<ThongTinPhong>;
            List<String> dsPhong = new List<String>();
            int soNgayO = 0;
            for (DateTime d = DateTime.Parse(f["ngaydat"].Trim()); d.CompareTo(DateTime.Parse(f["ngaytra"].Trim())) <= 0; d = d.AddDays(1.0))
            {
                soNgayO++;
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
            //if (dsPhongTrong.Count == 0)
            //{
            //    Session["TB"] = "Het phong!!!";
            //    return Redirect(strURL);
            //}
            var id_KH = idkh;
            string soluong = f["txtsoluong"];

            var phong = db.PHONGs.Where(p => p.Id_LP == loaiPhong.Id).ToList();
            var id_DP = dsPhongTrong[new Random().Next(dsPhongTrong.Count)];
            var SLNL = loaiPhong.SoLuongNguoiLon;
            var SLTE = loaiPhong.SoLuongTreEm;
            //var Gia = f["sum"].ToString().Split(' ')[0].Replace(",", "");/// ngày ở
            var Gia = Regex.Replace(f["sum"].ToString().Split(' ')[0], @"[.,]", ""); // tổng giá

            Session["sl"] = null;
            var kq = db.sp_AddTTDatPhong_Off(id_KH, SLNL, SLTE, double.Parse(Gia), ND, NT, ref id_DP);
            if (id_DP != null)
            {
                for (int i = 1; i <= int.Parse(soluong); i++)
                {
                    CTDATPHONG ct = new CTDATPHONG();

                    ct.Id_DatPhong = id_DP;

                    // get room is empty
                    var lstPT = dsPhongTrong[new Random().Next(dsPhongTrong.Count)];

                    int idp = int.Parse(lstPT);

                    var priceRoom = db.LOAIPHONGs.Select(lp => new ChiTietTTLP
                    {
                        Id = lp.Id,
                        Gia = lp.Gia ?? 0,
                        Ten = lp.TenLoai,
                        Id_p = db.PHONGs.Where(p => p.Id_LP == lp.Id).Select(p => p.Id).ToList()
                    }).ToList();


                    ct.Gia = priceRoom.Where(pp => pp.Id_p.Contains(idp)).FirstOrDefault().Gia;

                    ct.Id_P = idp;

                    dsPhongTrong.Remove(lstPT);

                    db.CTDATPHONGs.InsertOnSubmit(ct);
                }
                db.SubmitChanges();
            }

            // add hoá đơn
            var us = Session["AdminUser"] as TAIKHOAN;
            int idnv = db.NHANVIENs.Where(t => t.Id_tk == us.Id).FirstOrDefault().Id;
            Random rand = new Random();
            int num = rand.Next(100000000);
            HOADON hd = new HOADON();
            hd.Id = "h" + num.ToString();
            hd.Id_DP = id_DP;
            hd.Id_NV = idnv;
            hd.TiendatCoc = String.IsNullOrEmpty(f["tienCoc"]) ? 0 : double.Parse(f["tienCoc"].ToString().Split(' ')[1].Replace(",", "")); // get from form f["tienCoc"]
            hd.NgayTao = DateTime.Now;
            hd.NgayCapNhat = DateTime.Now;
            db.HOADONs.InsertOnSubmit(hd);
            db.SubmitChanges();
            var idctdp = db.CTDATPHONGs.Where(z => z.Id_DatPhong == id_DP).FirstOrDefault();
            Session["idctdp"] = idctdp;
            MailUtils.SendMailVerifyBookSuccess(TTCN.Email, id_DP, loaiPhong.TenLoai, double.Parse(Gia).ToString(), soNgayO.ToString(), SLNL.ToString(), SLTE.ToString());
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
                //Slnl=int.Parse(t.SoLuongNguoiTH.ToString()),
                //Slte=int.Parse(t.SoLuongTreEm.ToString()),
                Tongtien = double.Parse(db.THONGTINDATPHONGs.Where(d => d.Id == id).FirstOrDefault().DonGia.ToString()),
            });
            var CTDP = db.CTDATPHONGs.Where(t => t.Id_DatPhong == id).FirstOrDefault();
            List<int> dsPhong = new List<int>();
            dsPhong = db.CTDATPHONGs.Where(b => b.Id_DatPhong == id).Select(a => a.Id_P).ToList();
            var lstPT = dsPhong[new Random().Next(dsPhong.Count)];
            int idp = lstPT;
            var ttpXN = ttdp.Where(t => t.Id_DP == id).FirstOrDefault();
            ttpXN.Songayo = (1 + Math.Floor(double.Parse((DateTime.Parse(ttpXN.Ngaytra) - DateTime.Parse(ttpXN.Ngaydat)).Days.ToString()))).ToString();

            //lấy list tên phòng theo list idp
            ttpXN.CactenP = db.PHONGs.Where(p => dsPhong.Contains(p.Id)).Select(p => p.TenPhong).ToList();
            //lấy list IDLP theo list tên phòng
            List<string> dsLoaiP = db.PHONGs.Where(p => dsPhong.Contains(p.Id)).Select(p => p.Id_LP).ToList();
            //lấy list tên loại phòng theo list idLP
            ttpXN.Tenphong = db.LOAIPHONGs.Where(v => dsLoaiP.Contains(v.Id)).Select(v => v.TenLoai).ToList();
            //lấy list giá theo id_DP
            ttpXN.GiaLP = db.CTDATPHONGs.Where(x => x.Id_DatPhong == id).Select(z => double.Parse(z.Gia.Value.ToString())).Distinct().ToList();

            ttpXN.Slnl = db.LOAIPHONGs.Where(k => dsLoaiP.Contains(k.Id)).Select(k => k.SoLuongNguoiLon.ToString()).ToList();
            ttpXN.Slte = db.LOAIPHONGs.Where(k => dsLoaiP.Contains(k.Id)).Select(k => k.SoLuongTreEm.ToString()).ToList();

            //CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");   // try with "en-US"
            //string g = double.Parse(ttpXN.Tongtien).ToString("#,###", cul.NumberFormat);
            //ttpXN.Tongtien = g;
            mymodel.TTDP = ttpXN;

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
            var ttdp = db.THONGTINDATPHONGs
                .Where(dp => dp.Id == id)
                .Select(dp => new ThongTinDP
                {
                    Gia = dp.DonGia ?? 0,
                    NgDat = dp.NgayDat.ToString(),
                    NgTra = dp.NgayTra.ToString(),
                    SoLuongNgLon = dp.SoLuongNguoiTH.ToString(),
                    SoLuongTreEm = dp.SoLuongTreEm.ToString(),
                    Id = dp.Id
                }).FirstOrDefault();

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

            // thông tin dp
            mymodel.ttdp = new ThongTinDP
            {
                Gia = ttdp.Gia,
                NgDat = ttdp.NgDat,
                NgTra = ttdp.NgTra,
                SoLuongNgLon = ttdp.SoLuongNgLon,
                SoLuongTreEm = ttdp.SoLuongTreEm,
                Id = ttdp.Id,
                Ctdp = ctdpgr
                    .Where(ct => ct.Id_DatPhong == ttdp.Id)
                    .Select(ct => new CtDP
                    {
                        Gia = (double.Parse(ct.Gia) * int.Parse(ct.SoLuong) * (1 + Math.Floor(double.Parse((DateTime.Parse(ttdp.NgTra) - DateTime.Parse(ttdp.NgDat)).Days.ToString())))).ToString(),
                        Id_DatPhong = ct.Id_DatPhong,
                        SoLuong = ct.SoLuong,
                        TenLoai = ct.TenLoai,
                    })
                    .ToList(),
            };

            var dsPhong = db.CTDATPHONGs.Where(b => b.Id_DatPhong == id).Select(a => a.Id_P).ToList();

            List<string> dsLoaiP = db.PHONGs.Where(p => dsPhong.Contains(p.Id)).Select(p => p.Id_LP).ToList();

            // rooms list booked
            mymodel.dsP = db.PHONGs.Where(p => dsPhong.Contains(p.Id)).Select(p => p.TenPhong).ToList();

            // info user
            var user_id = db.THONGTINDATPHONGs.Where(tt => tt.Id == id).FirstOrDefault().Id_KH;
            mymodel.userInfo = db.THONGTINCANHANs.Where(ttcn => ttcn.Id_tk == user_id);


            Session["ctdp"] = mymodel.ttdp as ThongTinDP;

            /*
            var _ttlp = db.LOAIPHONGs.Join(db.PHONGs, lp => lp.Id, p => p.Id_LP, (lp, p) => new ThongTinLP { Id = lp.Id, Ten = lp.TenLoai, Id_p = p.Id });
            var ttdp = db.THONGTINDATPHONGs.Select(t => new ThongTinPhong
            {
                Id_DP = t.Id,
                Ten_kh = db.THONGTINCANHANs.Where(kh => kh.Id_tk == t.Id_KH).FirstOrDefault().HoTen,
                Ngaydat = t.NgayDat.ToString(), 
                Ngaytra = t.NgayTra.ToString(),
                Soluong = t.CTDATPHONGs.Where(a => a.Id_DatPhong == id).Count().ToString(),
                Id_P = t.CTDATPHONGs.Where(b => b.Id_DatPhong == id).Select(a => a.Id_P).ToList(),
                //Slnl=int.Parse(t.SoLuongNguoiTH.ToString()),
                //Slte=int.Parse(t.SoLuongTreEm.ToString()),
                Tongtien=double.Parse(db.THONGTINDATPHONGs.Where(d=>d.Id==id).FirstOrDefault().DonGia.ToString()),
            });
            var CTDP = db.CTDATPHONGs.Where(t => t.Id_DatPhong == id).FirstOrDefault();
            List<int> dsPhong = new List<int>();
            dsPhong = db.CTDATPHONGs.Where(b => b.Id_DatPhong == id).Select(a => a.Id_P).ToList();
            var lstPT = dsPhong[new Random().Next(dsPhong.Count)];
            int idp = lstPT;
            var ttpXN = ttdp.Where(t => t.Id_DP == id).FirstOrDefault();
            ttpXN.Songayo = (1 + Math.Floor(double.Parse((DateTime.Parse(ttpXN.Ngaytra) - DateTime.Parse(ttpXN.Ngaydat)).Days.ToString()))).ToString();


            // err /*
            //lấy list tên phòng theo list idp
            ttpXN.CactenP = db.PHONGs.Where(p => dsPhong.Contains(p.Id)).Select(p => p.TenPhong).ToList();
            //lấy list IDLP theo list tên phòng
            List<string> dsLoaiP=db.PHONGs.Where(p => dsPhong.Contains(p.Id)).Select(p => p.Id_LP).ToList();
            //lấy list tên loại phòng theo list idLP
            ttpXN.Tenphong = db.LOAIPHONGs.Where(v => dsLoaiP.Contains(v.Id)).Select(v => v.TenLoai).ToList();
            //lấy list giá theo id_DP
            ttpXN.GiaLP = db.CTDATPHONGs.Where(x => x.Id_DatPhong == id).Select(z =>double.Parse((z.Gia).Value.ToString())).Distinct().ToList();

            ttpXN.SoLuongLP = db.CTDATPHONGs.Where(x => x.Id_DatPhong == id).Select(z => z.)).ToList();

            ttpXN.Slnl = db.LOAIPHONGs.Where(k => dsLoaiP.Contains(k.Id)).Select(k => k.SoLuongNguoiLon.ToString()).ToList();
            ttpXN.Slte = db.LOAIPHONGs.Where(k => dsLoaiP.Contains(k.Id)).Select(k => k.SoLuongTreEm.ToString()).ToList();
            

            //CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");   // try with "en-US"
            //string g = double.Parse(ttpXN.Tongtien).ToString("#,###", cul.NumberFormat);
            //ttpXN.Tongtien = g;
            mymodel.TTDP = ttpXN;
            Session["ThongTinPhong"] = CTDP;
            Session["ThongTinDP"] = ttpXN;
            */
            return View(mymodel);
        }
        [HttpPost]
        public ActionResult XacNhanDatPhong(FormCollection f, THONGTINCANHAN ttcn)
        {
            var us = Session["AdminUser"] as TAIKHOAN;
            int idnv = db.NHANVIENs.Where(t => t.Id_tk == us.Id).FirstOrDefault().Id;
            var ctdp = Session["ctdp"] as ThongTinDP;
            
            Random rand = new Random();
            int num = rand.Next(100000000);
            HOADON hd = new HOADON();
            hd.Id = "h" + num.ToString();
            hd.Id_DP = ctdp.Id;
            hd.Id_NV = idnv;
            hd.TiendatCoc = 0;
            hd.NgayTao = DateTime.Now;
            hd.NgayCapNhat = DateTime.Now;
            db.HOADONs.InsertOnSubmit(hd);
            db.SubmitChanges();
            Session["XacNhan"] = null;
            Session["XacNhan"] = "Xác nhận đặt phòng thành công";

            //MailUtils.SendMailVerifyBookSuccess(ttcn.Email, ctdp.Id, loaiPhong.TenLoai, double.Parse(Gia).ToString(), soNgayO.ToString(), SLNL.ToString(), SLTE.ToString());
            return RedirectToAction("DanhSachDatTruoc","Admin");
        }
        public ActionResult HoaDon()
        {
            var ttcn = db.NHANVIENs.Join(db.THONGTINCANHANs, nv => nv.Id_tk, cn => cn.Id_tk, (nv, cn) => new ThongTinCN { Idnv=nv.Id,Id_ttcn=cn.Id,Id_tk=nv.Id_tk,Hoten=cn.HoTen });
            dynamic mymodel = new ExpandoObject();
            var dshd = db.HOADONs.Select(t => new ThongTinHoaDon
            {
               Id_HD=t.Id,
               IdDP=t.Id_DP,
               Id_NV=t.Id_NV.ToString(),
               Id_tknv=db.NHANVIENs.Where(nv=>nv.Id==t.Id_NV).FirstOrDefault().Id_tk,
               TienCoc=double.Parse(t.TiendatCoc.ToString()),
               NgayTaoHD=t.NgayTao.ToString(),
               TenNV=ttcn.Where(c=>c.Idnv==t.Id_NV).FirstOrDefault().Hoten,
            });
            //CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");   // try with "en-US"
            //string g = double.Parse().ToString("#,###", cul.NumberFormat);
            //ttpXN.Tongtien = g;
            mymodel.DSHD = dshd;
            return View(mymodel);
        }
        [HttpPost]
        public ActionResult HoaDon(FormCollection f)
        {
            dynamic mymodel = new ExpandoObject();
            string txtTK = f["txtTK"];
            var ttcn = db.NHANVIENs.Join(db.THONGTINCANHANs, nv => nv.Id_tk, cn => cn.Id_tk, (nv, cn) => new ThongTinCN { Idnv = nv.Id, Id_ttcn = cn.Id, Id_tk = nv.Id_tk, Hoten = cn.HoTen });
            var dshd = db.HOADONs.Select(t => new ThongTinHoaDon
            {
                Id_HD = t.Id,
                IdDP = t.Id_DP,
                Id_NV = t.Id_NV.ToString(),
                Id_tknv = db.NHANVIENs.Where(nv => nv.Id == t.Id_NV).FirstOrDefault().Id_tk,
                TienCoc = double.Parse(t.TiendatCoc.ToString()),
                NgayTaoHD = t.NgayTao.ToString(),
                TenNV = ttcn.Where(c => c.Idnv == t.Id_NV).FirstOrDefault().Hoten,
            });
            mymodel.DSHD = dshd;
            if (!String.IsNullOrEmpty(txtTK))
            {
                mymodel.DSHD = dshd.Where(t => t.Id_HD == txtTK || t.IdDP==txtTK).ToList();
            }
            return View(mymodel);
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
                Session["AdminUser"] = tttk;
                Session["tdn"] = tttk.TenTK;
                return RedirectToAction("Index", "Admin");
            }
            return View();
        }
        public ActionResult ThongKe()
        {
            return View();
        }
        public ActionResult ThongKeDoanhThu(int year)
        {
            dynamic mymodel = new ExpandoObject();
            var thongke = db.HOADONs.Join(db.THONGTINDATPHONGs, hd => hd.Id_DP, tt => tt.Id, (hd, tt) => new ThongKe { Id_hd = hd.Id, Id_dp = hd.Id_DP, Ngaytao = DateTime.Parse(hd.NgayTao.ToString()),Gia=double.Parse(tt.DonGia.ToString()) });
            mymodel.TK = thongke;
            return Json(thongke, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ThongTinTK()
        {
            var user = Session["AdminUser"] as TAIKHOAN;

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
            var user = Session["AdminUser"] as TAIKHOAN;
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
            //lỗi nếu đánh sai ngày tháng sinh
            else
            {
                var kq = db.sp_EditInfoUser(user.TenTK, hoten, DateTime.Parse(ngaysinh), Cccd, gioitinh, sdt, diachi);
                Session["Taotk"] = "Tạo tài khoản thành công";
            }
            mymodel.Info = infoAccount;
            return RedirectToAction("ThongTinTK", "Admin");
        }


    }
}