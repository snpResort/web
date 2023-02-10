using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QL_Resort.Models;
using System.Dynamic;
using System.Web.UI;
using System.Web.Helpers;
using System.Text.RegularExpressions;

namespace QL_Resort.Controllers
{
    public class PhongController : Controller
    {
        // GET: Phong
        QL_ResortDataContext db = new QL_ResortDataContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult XemChiTiet(string id)
        {
            dynamic mymodel = new ExpandoObject();
            var ha = db.HINHANH_LOAIPHONGs.Where(t => t.Id_LP == id).ToList();
            var mota = db.THONGTINLOAIPHONGs.Where(t => t.Id_LP == id).ToList().Distinct();
            var layLP = db.LOAIPHONGs.Where(t => t.Id == id).ToList();
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
            string SLNL = f["SLNL"];
            string SLTE = f["SLTE"];
            string GIA = f["gia"];
            GIA = Regex.Replace(f["gia"].ToString().Split(' ')[0], @"[.,]", "");

            int nl = 0;
            int te = 0;
            double g = 0;
            var lp = db.LOAIPHONGs.ToList();
            List<LoaiPhong> data = new List<LoaiPhong>();
            Session["timkiem"] = null;
            if (String.IsNullOrEmpty(SLTE) && String.IsNullOrEmpty(SLNL) && String.IsNullOrEmpty(GIA))
            {
                Session["timkiem"] = "Vui long nhap so luong!!!";
                return RedirectToAction("Index","Home");
            }    
            else if (String.IsNullOrEmpty(SLTE) && !String.IsNullOrEmpty(SLNL) && String.IsNullOrEmpty(GIA))
            {
                nl = int.Parse(SLNL);
                lp = db.LOAIPHONGs.Where(t => t.SoLuongNguoiLon >= nl).ToList();
            }
            else if (String.IsNullOrEmpty(SLNL) && !String.IsNullOrEmpty(SLTE) && String.IsNullOrEmpty(GIA))
            {
                te = int.Parse(SLTE);
                lp = db.LOAIPHONGs.Where(t => t.SoLuongTreEm >= te).ToList();
            }
            else if(String.IsNullOrEmpty(SLNL) && String.IsNullOrEmpty(SLTE) && !String.IsNullOrEmpty(GIA))
            {
                g = double.Parse(GIA);
                lp = db.LOAIPHONGs.Where(t => t.Gia <= g).ToList();
            }    
            else if (!String.IsNullOrEmpty(SLNL) && !String.IsNullOrEmpty(SLTE) && String.IsNullOrEmpty(GIA))
            {
                nl = int.Parse(SLNL);
                te = int.Parse(SLTE);
                lp = db.LOAIPHONGs.Where(t => t.SoLuongNguoiLon >= nl && t.SoLuongTreEm >= te).ToList();
            }
            else if (!String.IsNullOrEmpty(SLNL) && String.IsNullOrEmpty(SLTE) && !String.IsNullOrEmpty(GIA))
            {
                nl = int.Parse(SLNL);
                g = double.Parse(GIA);
                lp = db.LOAIPHONGs.Where(t => t.SoLuongNguoiLon >= nl && t.Gia <= g).ToList();
            }
            else if (String.IsNullOrEmpty(SLNL) && !String.IsNullOrEmpty(SLTE) && !String.IsNullOrEmpty(GIA))
            {
                te = int.Parse(SLTE);
                g = double.Parse(GIA);
                lp = db.LOAIPHONGs.Where(t => t.Gia <= g && t.SoLuongTreEm >= te).ToList();
            }
            else if (!String.IsNullOrEmpty(SLNL) && !String.IsNullOrEmpty(SLTE) && !String.IsNullOrEmpty(GIA))
            {
                nl = int.Parse(SLNL);
                te = int.Parse(SLTE);
                g = double.Parse(GIA);
                lp = db.LOAIPHONGs.Where(t => t.Gia <= g && t.SoLuongTreEm >= te && t.SoLuongNguoiLon >= nl).ToList();
            }
            foreach (var item in lp)
            {
                LoaiPhong _loaiphong = new LoaiPhong();
                _loaiphong.Id = item.Id;
                _loaiphong.Ten = item.TenLoai;
                _loaiphong.Mota = item.MoTa;
                _loaiphong.Gia = (double)item.Gia;
                _loaiphong.Hinhanh = db.HINHANH_LOAIPHONGs.Where(t => t.Id_LP == item.Id).FirstOrDefault().imagePath;
                //_loaiphong.Id_P = (Int32.Parse(db.PHONGs.Where(a => a.Id_LP == item.Id).ToString()));
                data.Add(_loaiphong);
            }
            mymodel.LayLP = data;
            //mymodel.HA = db.HINHANH_LOAIPHONGs.ToList();
            return View(mymodel);
        }
        public ActionResult ThanhToanThanhCong()
        {
            return View();
        }
        public ActionResult ThanhToan()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ThanhToan(string idlp, string strURL, FormCollection f)
        {
            // todo: check empty ngay dat & ngay tra
            if (String.IsNullOrEmpty(f["ngaydat"].Trim())) {
                Session["EmptyNgay"] = "ngaydat";
                return RedirectToAction("DatPhong", "Phong", new { id = idlp });
            }
            if (String.IsNullOrEmpty(f["ngaytra"].Trim())) {
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


            var user = Session["User"] as TAIKHOAN;
            // check is login
            if (user == null)
            {
                // return login page
                return RedirectToAction("DangNhap", "TaiKhoan");
            }
            DateTime ND = Convert.ToDateTime(f["ngaydat"]);
            DateTime NT = Convert.ToDateTime(f["ngaytra"]);

            Session["TB"] = null;
            Session["done"] = null;

            // get info for sp_AddTTDatPhong
            var username = user.TenTK;
            var id_KH = user.Id;
            string soluong = f["txtsoluong"];

            var phong = db.PHONGs.Where(p => p.Id_LP == loaiPhong.Id).ToList();
            var id_DP = dsPhongTrong[new Random().Next(dsPhongTrong.Count)];
            var SLNL = loaiPhong.SoLuongNguoiLon;
            var SLTE = loaiPhong.SoLuongTreEm;
            var Gia = Regex.Replace(f["sum"].ToString().Split(' ')[0], @"[.,]", ""); // tổng giá
            Session["TongTien"] = Gia;

            var kq = db.sp_AddTTDatPhong(username, SLNL, SLTE, double.Parse(Gia), ND, NT, ref id_DP);
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
            Session["done"] = "Code: " + id_DP;
            // send mail message

            MailUtils.SendMailBookSuccess(username, id_DP, loaiPhong.TenLoai, double.Parse(Gia).ToString(), soNgayO.ToString(), SLNL.ToString(), SLTE.ToString());

            // show message book success
            return RedirectToAction("ThanhToanThanhCong", "Phong");

        }
    }
}